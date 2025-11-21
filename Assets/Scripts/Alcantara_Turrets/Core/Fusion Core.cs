using UnityEngine;

public class FusionCore : MonoBehaviour
{
    [Header("Fusion Core Stats (Unique)")]
    [Tooltip("Shots per second")]
    public float fireRate = 1.5f;

    [Tooltip("Damage applied per hit")]
    public float damage = 20f;

    [Tooltip("Effective range units")]
    public float range = 7f;

    void Awake()
    {
        // Inspector-visible defaults for this unique core.
    }
}
