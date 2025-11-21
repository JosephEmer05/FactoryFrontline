using UnityEngine;

public class PowerCore : MonoBehaviour
{
    [Header("Power Core Stats (Rare)")]
    [Tooltip("Shots per second")]
    public float fireRate = 1.2f;

    [Tooltip("Damage applied per hit")]
    public float damage = 15f;

    [Tooltip("Effective range units")]
    public float range = 6f;

    void Awake()
    {
        // Ensure runtime values inherited turrets read are set
        // (keeps inspector-visible defaults on this component)
        // Also keep BaseCore.FireInterval behavior intact via inheritance.
    }
}
