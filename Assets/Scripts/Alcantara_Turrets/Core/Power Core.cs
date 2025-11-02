using UnityEngine;

public class PowerCore : BaseCore
{
    [Header("Power Core Stats (Rare)")]
    [Tooltip("Shots per second")]
    public new float fireRate = 1.2f;

    [Tooltip("Damage applied per hit")]
    public new float damage = 15f;

    [Tooltip("Effective range units")]
    public new float range = 6f;

    void Awake()
    {
        // Ensure runtime values inherited turrets read are set
        // (keeps inspector-visible defaults on this component)
        // Also keep BaseCore.FireInterval behavior intact via inheritance.
    }
}
