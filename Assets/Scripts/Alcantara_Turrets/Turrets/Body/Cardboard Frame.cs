using UnityEngine;

/// <summary>
/// Common body: basic HP and armor (inspector-editable defaults).
/// </summary>
public class CardboardFrame : MonoBehaviour
{
    [Header("Body Stats (Common)")]
    [Tooltip("Hit points provided by this body")]
    public int hp = 100;
}
