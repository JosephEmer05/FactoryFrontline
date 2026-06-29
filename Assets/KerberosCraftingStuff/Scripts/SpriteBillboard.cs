using UnityEngine;

public class SpriteBillboard : MonoBehaviour
{
    private Transform cam;

    void Start()
    {
        cam = Camera.main.transform;
    }

    void LateUpdate()
    {
        transform.rotation = Quaternion.LookRotation(cam.forward, Vector3.up);
    }
}
