using UnityEngine;

/// <summary>
/// Simple data container for base core stats.
/// Turrets can read these public fields directly until
/// the crafting/system integration is implemented.
/// </summary>
public class BaseCore : MonoBehaviour
{
    [Header("Base Stats (Common)")]
    [Tooltip("Shots per second")]
    public float fireRate = 1f;

    [Tooltip("Damage applied per hit")]
    public float damage = 10f;

    [Tooltip("Effective range units")]
    public float range = 5f;

    /// <summary>
    /// Seconds between shots (derived from fireRate).
    /// </summary>
    public float FireInterval => fireRate > 0f ? 1f / fireRate : Mathf.Infinity;
}
