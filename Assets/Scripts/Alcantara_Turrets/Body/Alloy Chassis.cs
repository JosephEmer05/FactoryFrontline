using UnityEngine;

/// <summary>
/// Unique body: highest HP, stronger armor and debuff resistance.
/// </summary>
public class AlloyChassis : MonoBehaviour
{
    [Header("Body Stats (Unique)")]
    [Tooltip("Hit points provided by this body")]
    public int hp = 200;

    [Tooltip("Fractional damage reduction (0.25 = 25% damage reduced)")]
    public float damageReductionPercent = 0.25f;

    [Tooltip("If true, this body reduces or resists status debuffs")]
    public bool debuffResistance = true;

    public float DamageMultiplier => 1f - Mathf.Clamp01(damageReductionPercent);
}
