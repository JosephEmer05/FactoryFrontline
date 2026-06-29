using UnityEngine;

/// <summary>
/// ScriptableObject container for unified turret configuration.
/// Create assets via: Create -> Scriptable Objects -> TurretConfig.
/// Assign the asset to an Alltowerscript component to apply these settings.
/// </summary>
[CreateAssetMenu(fileName = "TurretConfig", menuName = "Scriptable Objects/TurretConfig", order = 0)]
public class TurretConfig : ScriptableObject
{
    // Enums mirroring runtime selections
    public Alltowerscript.BodyType bodyType = Alltowerscript.BodyType.CardboardFrame;
    public Alltowerscript.CoreType coreType = Alltowerscript.CoreType.BaseCore;
    public Alltowerscript.WeaponType weaponType = Alltowerscript.WeaponType.Single_turret;

    [Header("Global Toggles")]
    public bool turretEnabled = true;
    public bool autoTargeting = true;
    public bool debugDraw = false;

    [Header("Body Feature Toggles")]
    public bool enableDamageReduction = true;
    public bool enableDebuffResistance = true;

    [Header("Single Shot Features")]
    public bool enableSingleShot = true;
    public bool singleHoming = true;
    public bool singleIgnoreTurretCollision = true;

    [Header("Spread Shot Features")]
    public bool enableSpreadShot = false;
    public bool spreadRandomize = true;

    [Header("Arc Cannon Features")]
    public bool enableArcCannon = false;
    public bool arcHoming = true;

    [Header("Prefabs (Optional Overrides)")]
    public GameObject singleProjectilePrefab;
    public GameObject spreadProjectilePrefab;
    public GameObject arcProjectilePrefab;

    [Header("Weapon Generic Stats")]
    public float spawnOffset = 1f;

    [Header("Single Shot Stats")]
    public float singleCooldown = 0f;
    public float singleProjectileSpeed = 15f;

    [Header("Spread Shot Stats")]
    public float spreadCooldown = 1f;
    public float spreadProjectileSpeed = 15f;
    public int spreadPelletCount = 8;
    public float spreadAngle = 20f;

    [Header("Arc Cannon Stats")]
    public float arcCooldown = 2f;
    public float arcProjectileSpeed = 15f;

    [Header("Optional Direct Core Overrides (leave 0 to use defaults by type)")]
    public float overrideCoreFireRate = 0f;
    public float overrideCoreDamage = 0f;
    public float overrideCoreRange = 0f;

#if UNITY_EDITOR
    void OnValidate()
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
#endif
}
