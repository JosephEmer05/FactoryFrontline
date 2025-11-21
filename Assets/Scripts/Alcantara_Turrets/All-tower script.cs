using UnityEngine;
using System.Linq;

/// <summary>
/// Unified turret script combining body, core and weapon behaviors with
/// per-feature enable/disable toggles. Can optionally load settings from a
/// TurretConfig ScriptableObject asset for easy reuse.
/// Projectiles now query damage from this script (no per-projectile damage set).
/// </summary>
public class Alltowerscript : MonoBehaviour
{
    public enum BodyType { CardboardFrame, ReinforcedShell, AlloyChassis }
    public enum CoreType { BaseCore, PowerCore, FusionCore }
    public enum WeaponType { Single_turret, Spread_turret, Arc_Cannon }

    [Header("Config Asset (Optional)")] public TurretConfig config; public bool continuousConfigSync = false;
    [Header("Mode Selection")] public BodyType bodyType = BodyType.CardboardFrame; public CoreType coreType = CoreType.BaseCore; public WeaponType weaponType = WeaponType.Single_turret;
    [Header("Global Toggles")] public bool turretEnabled = true; public bool autoTargeting = true; public bool debugDraw = false;
    [Header("Body Feature Toggles")] public bool enableDamageReduction = true; public bool enableDebuffResistance = true;
    [Header("Single Shot Features")] public bool enableSingleShot = true; public bool singleHoming = true; public bool singleIgnoreTurretCollision = true;
    [Header("Spread Shot Features")] public bool enableSpreadShot = false; public bool spreadRandomize = true; public bool spreadHoming = false;
    [Header("Arc Cannon Features")] public bool enableArcCannon = false; public bool arcHoming = true;

    [Header("Prefabs (Assign / Override)")] public GameObject singleProjectilePrefab; public GameObject spreadProjectilePrefab; public GameObject arcProjectilePrefab;
    [Header("Weapon Generic Stats")] public float spawnOffset = 1f;
    [Header("Single Shot Stats")] public float singleCooldown = 0f; public float singleProjectileSpeed = 15f;
    [Header("Spread Shot Stats")] public float spreadCooldown = 1f; public float spreadProjectileSpeed = 15f; public int spreadPelletCount = 8; public float spreadAngle = 20f;
    [Header("Arc Cannon Stats")] public float arcCooldown = 2f; public float arcProjectileSpeed = 15f;

    [Header("Derived Body Stats (Read Only)")] [SerializeField] private int bodyHp; [SerializeField] private float damageReductionPercent; [SerializeField] private bool debuffResistance;
    [Header("Derived Core Stats (Read Only)")] [SerializeField] private float coreFireRate; [SerializeField] private float coreDamage; [SerializeField] private float coreRange;

    private float _lastFireTime = 0f; private Transform _currentTarget;

    void Awake() { PullFromConfig(); RecomputeDerivedStats(); }
    void Start() { PullFromConfig(); RecomputeDerivedStats(); }
#if UNITY_EDITOR
    void OnValidate() { PullFromConfig(); ClampInputs(); RecomputeDerivedStats(); }
#endif

    void Update()
    {
        if (continuousConfigSync) { PullFromConfig(); RecomputeDerivedStats(); }
        if (!turretEnabled) return;
        if (autoTargeting) _currentTarget = AcquireTarget();
        if (_currentTarget == null) return;
        Vector3 dir = (_currentTarget.position - transform.position).normalized; if (dir != Vector3.zero) transform.rotation = Quaternion.LookRotation(dir);
        if (enableSingleShot && weaponType == WeaponType.Single_turret) TryFireSingle();
        else if (enableSpreadShot && weaponType == WeaponType.Spread_turret) TryFireSpread();
        else if (enableArcCannon && weaponType == WeaponType.Arc_Cannon) TryFireArc();
    }

    private void PullFromConfig()
    {
        if (config == null) return;
        bodyType = config.bodyType; coreType = config.coreType; weaponType = config.weaponType;
        turretEnabled = config.turretEnabled; autoTargeting = config.autoTargeting; debugDraw = config.debugDraw;
        enableDamageReduction = config.enableDamageReduction; enableDebuffResistance = config.enableDebuffResistance;
        enableSingleShot = config.enableSingleShot; singleHoming = config.singleHoming; singleIgnoreTurretCollision = config.singleIgnoreTurretCollision;
        enableSpreadShot = config.enableSpreadShot; spreadRandomize = config.spreadRandomize;
        enableArcCannon = config.enableArcCannon; arcHoming = config.arcHoming;
        if (config.singleProjectilePrefab) singleProjectilePrefab = config.singleProjectilePrefab;
        if (config.spreadProjectilePrefab) spreadProjectilePrefab = config.spreadProjectilePrefab;
        if (config.arcProjectilePrefab) arcProjectilePrefab = config.arcProjectilePrefab;
        spawnOffset = config.spawnOffset; singleCooldown = config.singleCooldown; singleProjectileSpeed = config.singleProjectileSpeed;
        spreadCooldown = config.spreadCooldown; spreadProjectileSpeed = config.spreadProjectileSpeed; spreadPelletCount = config.spreadPelletCount; spreadAngle = config.spreadAngle;
        arcCooldown = config.arcCooldown; arcProjectileSpeed = config.arcProjectileSpeed;
        if (config.overrideCoreFireRate > 0f) coreFireRate = config.overrideCoreFireRate;
        if (config.overrideCoreDamage > 0f) coreDamage = config.overrideCoreDamage;
        if (config.overrideCoreRange > 0f) coreRange = config.overrideCoreRange;
    }

    private void ClampInputs()
    {
        spawnOffset = Mathf.Max(0f, spawnOffset); singleCooldown = Mathf.Max(0f, singleCooldown); singleProjectileSpeed = Mathf.Max(0f, singleProjectileSpeed);
        spreadCooldown = Mathf.Max(0f, spreadCooldown); spreadProjectileSpeed = Mathf.Max(0f, spreadProjectileSpeed); spreadPelletCount = Mathf.Max(1, spreadPelletCount);
        spreadAngle = Mathf.Max(0f, spreadAngle); arcCooldown = Mathf.Max(0f, arcCooldown); arcProjectileSpeed = Mathf.Max(0f, arcProjectileSpeed);
    }

    private void RecomputeDerivedStats()
    {
        switch (bodyType)
        {
            case BodyType.CardboardFrame: bodyHp = 100; damageReductionPercent = 0f; debuffResistance = false; break;
            case BodyType.ReinforcedShell: bodyHp = 150; damageReductionPercent = enableDamageReduction ? 0.15f : 0f; debuffResistance = false; break;
            case BodyType.AlloyChassis: bodyHp = 200; damageReductionPercent = enableDamageReduction ? 0.25f : 0f; debuffResistance = enableDebuffResistance; break;
        }
        if (coreFireRate <= 0f || coreDamage <= 0f || coreRange <= 0f)
        {
            switch (coreType)
            {
                case CoreType.BaseCore: coreFireRate = 1f; coreDamage = 10f; coreRange = 5f; break;
                case CoreType.PowerCore: coreFireRate = 1.2f; coreDamage = 15f; coreRange = 6f; break;
                case CoreType.FusionCore: coreFireRate = 1.5f; coreDamage = 20f; coreRange = 7f; break;
            }
        }
    }

    private Transform AcquireTarget()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy"); if (enemies == null || enemies.Length == 0) return null;
        return enemies.Where(e => Vector3.Distance(transform.position, e.transform.position) <= coreRange)
                      .OrderBy(e => Vector3.Distance(transform.position, e.transform.position))
                      .Select(e => e.transform).FirstOrDefault();
    }

    private bool Ready(float cooldown) => Time.time >= _lastFireTime + cooldown; private void AfterFire() => _lastFireTime = Time.time;

    private void TryFireSingle()
    { float interval = singleCooldown > 0f ? singleCooldown : (coreFireRate > 0f ? 1f / coreFireRate : Mathf.Infinity); if (!Ready(interval)) return; FireSingleProjectile(_currentTarget); AfterFire(); }

    private void FireSingleProjectile(Transform target)
    {
        if (singleProjectilePrefab == null) { singleProjectilePrefab = Resources.Load<GameObject>("Prefabs/SinglePrefab"); if (!singleProjectilePrefab) { Debug.LogError("UnifiedTurret: Missing single projectile prefab."); return; } }
        Vector3 dir = (target.position - transform.position).normalized; Vector3 spawnPos = transform.position + dir * spawnOffset;
        GameObject bullet = Instantiate(singleProjectilePrefab, spawnPos, Quaternion.LookRotation(dir)); Rigidbody rb = bullet.GetComponent<Rigidbody>(); if (rb) rb.linearVelocity = dir * singleProjectileSpeed;
        if (singleIgnoreTurretCollision)
        {
            Collider bCol = bullet.GetComponent<Collider>(); if (bCol)
            {
                foreach (var t in GameObject.FindGameObjectsWithTag("Turret")) foreach (var tc in t.GetComponentsInChildren<Collider>()) Physics.IgnoreCollision(bCol, tc, true);
                foreach (var tc in GetComponentsInChildren<Collider>()) Physics.IgnoreCollision(bCol, tc, true);
            }
        }
        var proj = bullet.GetComponent<Single_Projectile>();
        if (proj)
        {
            proj.SetTarget(target);
            proj.homing = singleHoming;
            proj.owner = this; // supply damage source
        }
    }

    private void TryFireSpread() { if (!Ready(spreadCooldown)) return; FireSpreadVolley(_currentTarget); AfterFire(); }

    private void FireSpreadVolley(Transform target)
    {
        if (spreadProjectilePrefab == null) { spreadProjectilePrefab = Resources.Load<GameObject>("Prefabs/SpreadPrefab"); if (!spreadProjectilePrefab) { Debug.LogError("UnifiedTurret: Missing spread projectile prefab."); return; } }
        float halfAngle = spreadAngle * 0.5f; Vector3 baseDir = (target.position - transform.position).normalized; Quaternion lookRot = Quaternion.LookRotation(baseDir);
        for (int i = 0; i < spreadPelletCount; i++)
        {
            float yaw = spreadRandomize ? Random.Range(-halfAngle, halfAngle) : Mathf.Lerp(-halfAngle, halfAngle, spreadPelletCount > 1 ? (float)i / (spreadPelletCount - 1) : 0f);
            float pitch = spreadRandomize ? Random.Range(-halfAngle, halfAngle) : 0f;
            Quaternion offsetRot = Quaternion.Euler(pitch, yaw, 0f); Vector3 pelletDir = lookRot * offsetRot * Vector3.forward; Vector3 spawnPos = transform.position + pelletDir * spawnOffset;
            GameObject pellet = Instantiate(spreadProjectilePrefab, spawnPos, Quaternion.LookRotation(pelletDir)); Rigidbody rb = pellet.GetComponent<Rigidbody>(); if (rb) rb.linearVelocity = pelletDir * spreadProjectileSpeed;
            var spreadProj = pellet.GetComponent<Spread_Projectile>();
            if (spreadProj)
            {
                spreadProj.owner = this;
                if (spreadHoming) spreadProj.SetTarget(target);
                spreadProj.homing = spreadHoming;
            }
            else
            {
                // fallback reuse single projectile
                var singleProj = pellet.GetComponent<Single_Projectile>();
                if (singleProj)
                {
                    singleProj.owner = this;
                    singleProj.homing = spreadHoming || singleHoming;
                    singleProj.SetTarget(target);
                }
            }
        }
    }

    private void TryFireArc() { if (!Ready(arcCooldown)) return; FireArcProjectile(_currentTarget); AfterFire(); }

    private void FireArcProjectile(Transform target)
    {
        if (arcProjectilePrefab == null) { arcProjectilePrefab = Resources.Load<GameObject>("Prefabs/ArcPrefab"); if (!arcProjectilePrefab) { Debug.LogError("UnifiedTurret: Missing arc projectile prefab."); return; } }
        Vector3 dir = (target.position - transform.position).normalized; GameObject arc = Instantiate(arcProjectilePrefab, transform.position, Quaternion.LookRotation(dir)); Rigidbody rb = arc.GetComponent<Rigidbody>(); if (rb) rb.linearVelocity = dir * arcProjectileSpeed; var proj = arc.GetComponent<Arc_Projectile>(); if (proj) { proj.SetTarget(target); proj.homing = arcHoming; proj.owner = this; }
    }

    // Public damage accessor used by projectiles
    public float CurrentDamage => coreDamage;

    public int CurrentHP => bodyHp; public float DamageMultiplier => 1f - Mathf.Clamp01(enableDamageReduction ? damageReductionPercent : 0f); public bool HasDebuffResistance => debuffResistance && enableDebuffResistance;
    public float CurrentRange => coreRange; public float FireInterval => coreFireRate > 0f ? 1f / coreFireRate : Mathf.Infinity;
    public void ForceRecompute() { PullFromConfig(); RecomputeDerivedStats(); }
    public void SetWeapon(WeaponType type) { weaponType = type; enableSingleShot = type == WeaponType.Single_turret; enableSpreadShot = type == WeaponType.Spread_turret; enableArcCannon = type == WeaponType.Arc_Cannon; }
    public void DisableAllWeapons() { enableSingleShot = enableSpreadShot = enableArcCannon = false; }
    public void DisableAllFeatures() { turretEnabled = false; DisableAllWeapons(); enableDamageReduction = false; enableDebuffResistance = false; }

    void OnDrawGizmosSelected()
    {
        if (!debugDraw) return; Gizmos.color = Color.yellow; Gizmos.DrawWireSphere(transform.position, coreRange);
        if (weaponType == WeaponType.Spread_turret && enableSpreadShot) { Vector3 forward = transform.forward; float half = spreadAngle * 0.5f; Quaternion left = Quaternion.Euler(0f, -half, 0f); Quaternion right = Quaternion.Euler(0f, half, 0f); Gizmos.color = Color.cyan; Gizmos.DrawLine(transform.position, transform.position + (left * forward) * coreRange); Gizmos.DrawLine(transform.position, transform.position + (right * forward) * coreRange); }
    }
}
