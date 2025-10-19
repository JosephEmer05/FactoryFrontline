using UnityEngine;

public class ObjFollowMouse : MonoBehaviour
{
    private DragGrid dragGrid;

    void Start()
    {
        dragGrid = FindFirstObjectByType<DragGrid>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
