using UnityEngine;

public class FusionCore : BaseCore
{
    [Header("Fusion Core Stats (Unique)")]
    [Tooltip("Shots per second")]
    public new float fireRate = 1.5f;

    [Tooltip("Damage applied per hit")]
    public new float damage = 20f;

    [Tooltip("Effective range units")]
    public new float range = 7f;

    void Awake()
    {
        // Inspector-visible defaults for this unique core.
    }
}
