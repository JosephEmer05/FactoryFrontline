using UnityEngine;

/// <summary>
/// Rare body: more HP and damage resistance.
/// </summary>
public class ReinforcedShell : MonoBehaviour
{
    [Header("Body Stats (Rare)")]
    [Tooltip("Hit points provided by this body")]
    public int hp = 150;

    [Tooltip("Fractional damage reduction (0.15 = 15% damage reduced)")]
    public float damageReductionPercent = 0.15f;

    /// <summary>
    /// Convenience multiplier to apply to incoming damage: damageTaken = incoming * DamageMultiplier
    /// </summary>
    public float DamageMultiplier => 1f - Mathf.Clamp01(damageReductionPercent);
}
