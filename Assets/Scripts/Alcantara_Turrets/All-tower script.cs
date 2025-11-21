using UnityEngine;
using System.Linq;

/// <summary>
/// Unified turret script combining body, core and weapon behaviors with
/// per-feature enable/disable toggles. Replaces separate Body/Core/Gun scripts.
/// Attach to a turret GameObject; assign desired prefabs. Only one weapon mode
/// should generally be active at a time (Single / Spread / Arc).
/// </summary>
public class Alltowerscript : MonoBehaviour
{
    #region Enumerations
    // Renamed enum members to match actual component class names provided by user.
    public enum BodyType { CardboardFrame, ReinforcedShell, AlloyChassis }
    public enum CoreType { BaseCore, PowerCore, FusionCore }
    public enum WeaponType { Single_turret, Spread_turret, Arc_Cannon }
    #endregion

    [Header("Mode Selection")]
    public BodyType bodyType = BodyType.CardboardFrame;
    public CoreType coreType = CoreType.BaseCore;
    public WeaponType weaponType = WeaponType.Single_turret;

    [Header("Global Toggles")] 
    [Tooltip("Disable all runtime firing logic.")] public bool turretEnabled = true;
    [Tooltip("Acquire targets automatically within range.")] public bool autoTargeting = true;
    [Tooltip("Debug draw gizmos (range, spread cone etc.).")] public bool debugDraw = false;

    [Header("Body Feature Toggles")] 
    [Tooltip("Enable damage reduction (for ReinforcedShell / AlloyChassis).")]
    public bool enableDamageReduction = true;
    [Tooltip("Enable debuff resistance (AlloyChassis only).")]
    public bool enableDebuffResistance = true;

    [Header("Single Shot Features")] 
    [Tooltip("Enable Single_turret weapon mode.")] public bool enableSingleShot = true;
    [Tooltip("Enable homing: projectile receives target via SetTarget.")] public bool singleHoming = true;
    [Tooltip("Ignore collisions with turrets on spawn.")] public bool singleIgnoreTurretCollision = true;

    [Header("Spread Shot Features")] 
    [Tooltip("Enable Spread_turret weapon mode.")] public bool enableSpreadShot = false;
    [Tooltip("Randomize pellet yaw/pitch inside cone each shot.")] public bool spreadRandomize = true;

    [Header("Arc Cannon Features")] 
    [Tooltip("Enable Arc_Cannon weapon mode.")] public bool enableArcCannon = false;
    [Tooltip("Arc projectile receives target (if homing logic exists).")]
    public bool arcHoming = true;

    [Header("Prefabs (Assign / Override)")] 
    public GameObject singleProjectilePrefab; // expects Single_Projectile optionally
    public GameObject spreadProjectilePrefab; // pellet prefab (can also use Single)
    public GameObject arcProjectilePrefab;    // expects Arc_Projectile optionally

    [Header("Weapon Generic Stats")] 
    [Tooltip("Spawn offset distance forward from turret for projectiles/pellets.")] public float spawnOffset = 1f;

    [Header("Single Shot Stats")] 
    [Tooltip("Cooldown seconds between single shots (0 = continuous each frame).")]
    public float singleCooldown = 0f;
    [Tooltip("Projectile speed for single shots.")] public float singleProjectileSpeed = 15f;

    [Header("Spread Shot Stats")] 
    [Tooltip("Cooldown seconds between spread volleys.")] public float spreadCooldown = 1f;
    [Tooltip("Projectile speed for spread pellets.")] public float spreadProjectileSpeed = 15f;
    [Tooltip("Number of pellets per volley.")] public int spreadPelletCount = 8;
    [Tooltip("Total horizontal+vertical cone angle in degrees.")] public float spreadAngle = 20f;

    [Header("Arc Cannon Stats")] 
    [Tooltip("Cooldown seconds between arc shots.")] public float arcCooldown = 2f;
    [Tooltip("Projectile speed for arc shots.")] public float arcProjectileSpeed = 15f;

    [Header("Derived Body Stats (Read Only)")] 
    [SerializeField] private int bodyHp;
    [SerializeField] private float damageReductionPercent; // 0..1
    [SerializeField] private bool debuffResistance;

    [Header("Derived Core Stats (Read Only)")] 
    [SerializeField] private float coreFireRate; // shots/sec base
    [SerializeField] private float coreDamage;   // damage per hit
    [SerializeField] private float coreRange;    // targeting range

    // Timing
    private float _lastFireTime = 0f;

    // Cached current target
    private Transform _currentTarget;

    // Initialization & Validation
    void Awake() => RecomputeDerivedStats();
    void Start() => RecomputeDerivedStats();
#if UNITY_EDITOR
    void OnValidate()
    {
        ClampInputs();
        RecomputeDerivedStats();
    }
#endif

    private void ClampInputs()
    {
        spawnOffset = Mathf.Max(0f, spawnOffset);
        singleCooldown = Mathf.Max(0f, singleCooldown);
        singleProjectileSpeed = Mathf.Max(0f, singleProjectileSpeed);
        spreadCooldown = Mathf.Max(0f, spreadCooldown);
        spreadProjectileSpeed = Mathf.Max(0f, spreadProjectileSpeed);
        spreadPelletCount = Mathf.Max(1, spreadPelletCount);
        spreadAngle = Mathf.Max(0f, spreadAngle);
        arcCooldown = Mathf.Max(0f, arcCooldown);
        arcProjectileSpeed = Mathf.Max(0f, arcProjectileSpeed);
    }

    /// <summary>
    /// Recompute body/core derived stats based on selected types & feature toggles.
    /// </summary>
    private void RecomputeDerivedStats()
    {
        // Body
        switch (bodyType)
        {
            case BodyType.CardboardFrame:
                bodyHp = 100;
                damageReductionPercent = 0f;
                debuffResistance = false;
                break;
            case BodyType.ReinforcedShell:
                bodyHp = 150;
                damageReductionPercent = enableDamageReduction ? 0.15f : 0f;
                debuffResistance = false;
                break;
            case BodyType.AlloyChassis:
                bodyHp = 200;
                damageReductionPercent = enableDamageReduction ? 0.25f : 0f;
                debuffResistance = enableDebuffResistance;
                break;
        }

        // Core
        switch (coreType)
        {
            case CoreType.BaseCore:
                coreFireRate = 1f; coreDamage = 10f; coreRange = 5f; break;
            case CoreType.PowerCore:
                coreFireRate = 1.2f; coreDamage = 15f; coreRange = 6f; break;
            case CoreType.FusionCore:
                coreFireRate = 1.5f; coreDamage = 20f; coreRange = 7f; break;
        }
    }

    void Update()
    {
        if (!turretEnabled) return;

        if (autoTargeting)
            _currentTarget = AcquireTarget();

        if (_currentTarget == null)
            return;

        // Face target
        Vector3 dir = (_currentTarget.position - transform.position).normalized;
        if (dir != Vector3.zero)
            transform.rotation = Quaternion.LookRotation(dir);

        // Determine weapon and its cooldown
        if (enableSingleShot && weaponType == WeaponType.Single_turret)
            TryFireSingle();
        else if (enableSpreadShot && weaponType == WeaponType.Spread_turret)
            TryFireSpread();
        else if (enableArcCannon && weaponType == WeaponType.Arc_Cannon)
            TryFireArc();
    }

    private Transform AcquireTarget()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        if (enemies == null || enemies.Length == 0) return null;
        return enemies
            .Where(e => Vector3.Distance(transform.position, e.transform.position) <= coreRange)
            .OrderBy(e => Vector3.Distance(transform.position, e.transform.position))
            .Select(e => e.transform)
            .FirstOrDefault();
    }

    #region Fire Logic
    private bool Ready(float cooldown) => Time.time >= _lastFireTime + cooldown;

    private void AfterFire() => _lastFireTime = Time.time;

    private void TryFireSingle()
    {
        // Use either singleCooldown if provided else fallback to core fire interval.
        float interval = singleCooldown > 0f ? singleCooldown : (coreFireRate > 0f ? 1f / coreFireRate : Mathf.Infinity);
        if (!Ready(interval)) return;
        FireSingleProjectile(_currentTarget);
        AfterFire();
    }

    private void FireSingleProjectile(Transform target)
    {
        if (singleProjectilePrefab == null)
        {
            singleProjectilePrefab = Resources.Load<GameObject>("Prefabs/SinglePrefab");
            if (singleProjectilePrefab == null)
            {
                Debug.LogError("UnifiedTurret: Missing single projectile prefab.");
                return;
            }
        }
        Vector3 dir = (target.position - transform.position).normalized;
        Vector3 spawnPos = transform.position + dir * spawnOffset;
        GameObject bullet = Instantiate(singleProjectilePrefab, spawnPos, Quaternion.LookRotation(dir));
        Rigidbody rb = bullet.GetComponent<Rigidbody>();
        if (rb != null) rb.linearVelocity = dir * singleProjectileSpeed;

        if (singleIgnoreTurretCollision)
        {
            Collider bCol = bullet.GetComponent<Collider>();
            if (bCol != null)
            {
                GameObject[] turrets = GameObject.FindGameObjectsWithTag("Turret");
                foreach (var t in turrets)
                {
                    foreach (var tc in t.GetComponentsInChildren<Collider>())
                        Physics.IgnoreCollision(bCol, tc, true);
                }
                foreach (var tc in GetComponentsInChildren<Collider>())
                    Physics.IgnoreCollision(bCol, tc, true);
            }
        }

        if (singleHoming)
        {
            var proj = bullet.GetComponent<Single_Projectile>();
            if (proj != null) proj.SetTarget(target);
        }
    }

    private void TryFireSpread()
    {
        if (!Ready(spreadCooldown)) return;
        FireSpreadVolley(_currentTarget);
        AfterFire();
    }

    private void FireSpreadVolley(Transform target)
    {
        if (spreadProjectilePrefab == null)
        {
            spreadProjectilePrefab = Resources.Load<GameObject>("Prefabs/SpreadPrefab");
            if (spreadProjectilePrefab == null)
            {
                Debug.LogError("UnifiedTurret: Missing spread projectile prefab.");
                return;
            }
        }

        float halfAngle = spreadAngle * 0.5f;
        Vector3 baseDir = (target.position - transform.position).normalized;
        Quaternion lookRot = Quaternion.LookRotation(baseDir);

        for (int i = 0; i < spreadPelletCount; i++)
        {
            float yaw = spreadRandomize ? Random.Range(-halfAngle, halfAngle) : Mathf.Lerp(-halfAngle, halfAngle, (float)i / (spreadPelletCount - 1));
            float pitch = spreadRandomize ? Random.Range(-halfAngle, halfAngle) : 0f;
            Quaternion offsetRot = Quaternion.Euler(pitch, yaw, 0f);
            Vector3 pelletDir = lookRot * offsetRot * Vector3.forward;
            Vector3 spawnPos = transform.position + pelletDir * spawnOffset;
            GameObject pellet = Instantiate(spreadProjectilePrefab, spawnPos, Quaternion.LookRotation(pelletDir));
            Rigidbody rb = pellet.GetComponent<Rigidbody>();
            if (rb != null) rb.linearVelocity = pelletDir * spreadProjectileSpeed;

            // Optional homing reuse Single_Projectile if present
            if (singleHoming)
            {
                var proj = pellet.GetComponent<Single_Projectile>();
                if (proj != null) proj.SetTarget(target);
            }
        }
    }

    private void TryFireArc()
    {
        if (!Ready(arcCooldown)) return;
        FireArcProjectile(_currentTarget);
        AfterFire();
    }

    private void FireArcProjectile(Transform target)
    {
        if (arcProjectilePrefab == null)
        {
            arcProjectilePrefab = Resources.Load<GameObject>("Prefabs/ArcPrefab");
            if (arcProjectilePrefab == null)
            {
                Debug.LogError("UnifiedTurret: Missing arc projectile prefab.");
                return;
            }
        }
        Vector3 dir = (target.position - transform.position).normalized;
        GameObject arc = Instantiate(arcProjectilePrefab, transform.position, Quaternion.LookRotation(dir));
        Rigidbody rb = arc.GetComponent<Rigidbody>();
        if (rb != null) rb.linearVelocity = dir * arcProjectileSpeed;
        if (arcHoming)
        {
            var proj = arc.GetComponent<Arc_Projectile>();
            if (proj != null) proj.SetTarget(target);
        }
    }
    #endregion

    #region Public API
    public int CurrentHP => bodyHp;
    public float DamageMultiplier => 1f - Mathf.Clamp01(enableDamageReduction ? damageReductionPercent : 0f);
    public bool HasDebuffResistance => debuffResistance && enableDebuffResistance;
    public float CurrentRange => coreRange;
    public float CurrentDamage => coreDamage;
    public float FireInterval => coreFireRate > 0f ? 1f / coreFireRate : Mathf.Infinity;

    public void ForceRecompute() => RecomputeDerivedStats();

    public void SetWeapon(WeaponType type)
    {
        weaponType = type;
        enableSingleShot = type == WeaponType.Single_turret;
        enableSpreadShot = type == WeaponType.Spread_turret;
        enableArcCannon = type == WeaponType.Arc_Cannon;
    }

    public void DisableAllWeapons()
    {
        enableSingleShot = enableSpreadShot = enableArcCannon = false;
    }

    public void DisableAllFeatures()
    {
        turretEnabled = false;
        DisableAllWeapons();
        enableDamageReduction = false;
        enableDebuffResistance = false;
    }
    #endregion

    #region Gizmos
    void OnDrawGizmosSelected()
    {
        if (!debugDraw) return;
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, coreRange);
        if (weaponType == WeaponType.Spread_turret && enableSpreadShot)
        {
            // Approximate forward cone lines
            Vector3 forward = transform.forward;
            float half = spreadAngle * 0.5f;
            Quaternion left = Quaternion.Euler(0f, -half, 0f);
            Quaternion right = Quaternion.Euler(0f, half, 0f);
            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(transform.position, transform.position + (left * forward) * coreRange);
            Gizmos.DrawLine(transform.position, transform.position + (right * forward) * coreRange);
        }
    }
    #endregion
}
