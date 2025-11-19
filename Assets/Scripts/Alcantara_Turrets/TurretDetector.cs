using UnityEngine;

public class TurretDeathDetector : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        Debug.LogError(
            $"TURRET COLLISION -> Turret hit by: {collision.gameObject.name}, " +
            $"Tag: {collision.gameObject.tag}, " +
            $"Layer: {LayerMask.LayerToName(collision.gameObject.layer)}, " +
            $"Relative Velocity: {collision.relativeVelocity}"
        );
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.LogError(
            $"TURRET TRIGGER -> Triggered by: {other.gameObject.name}, " +
            $"Tag: {other.gameObject.tag}, " +
            $"Layer: {LayerMask.LayerToName(other.gameObject.layer)}"
        );
    }

    private void OnDestroy()
    {
        Debug.LogError(
            " TURRET DESTROYED \n" +
            "If this wasn't done manually or by game logic, something called Destroy() on this object.\n" +
            "Check the logs ABOVE this message for the last collision/trigger."
        );
    }
}
