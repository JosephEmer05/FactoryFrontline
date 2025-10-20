using UnityEngine;

[ExecuteInEditMode]
public class IsometricCameraSetup : MonoBehaviour
{
    [Header("Camera Settings")]
    public bool useOrthographic = true;
    [Range(1f, 20f)] public float orthographicSize = 6f;

    [Header("Angle Settings")]
    [Range(0f, 90f)] public float tiltAngle = 40f; // X-axis rotation
    [Range(0f, 90f)] public float panAngle = 35f;  // Y-axis rotation

    [Header("Position Settings")]
    public Vector3 cameraOffset = new Vector3(0f, 10f, -10f);
    public Transform lookTarget; // optional - where camera looks at

    void Update()
    {
        Camera cam = GetComponent<Camera>();
        if (cam == null) return;

        // Switch projection
        cam.orthographic = useOrthographic;
        cam.orthographicSize = orthographicSize;

        // Apply rotation
        transform.rotation = Quaternion.Euler(tiltAngle, panAngle, 0f);

        // Apply position offset relative to target if assigned
        if (lookTarget != null)
        {
            transform.position = lookTarget.position + cameraOffset;
            transform.LookAt(lookTarget);
        }
        else
        {
            transform.position = cameraOffset;
        }
    }
}
