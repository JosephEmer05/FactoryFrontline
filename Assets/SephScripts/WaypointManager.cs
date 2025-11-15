using UnityEngine;

public class WaypointManager : MonoBehaviour
{
    public static WaypointManager Instance;

    [Header("Low-Ground Paths")]
    public Transform[] lowPaths;
    private Transform[][] lowGround;

    [Header("High-Ground Paths")]
    public Transform[] highPaths;
    private Transform[][] highGround;

    void Awake()
    {
        Instance = this;

        lowGround = new Transform[lowPaths.Length][];
        for (int i = 0; i < lowPaths.Length; i++)
        {
            lowGround[i] = new Transform[lowPaths[i].childCount];
            for (int j = 0; j < lowPaths[i].childCount; j++)
                lowGround[i][j] = lowPaths[i].GetChild(j);
        }

        highGround = new Transform[highPaths.Length][];
        for (int i = 0; i < highPaths.Length; i++)
        {
            highGround[i] = new Transform[highPaths[i].childCount];
            for (int j = 0; j < highPaths[i].childCount; j++)
                highGround[i][j] = highPaths[i].GetChild(j);
        }
    }

    public Transform[] GetGroundPath(int spawnIndex)
    {
        return lowGround[Mathf.Clamp(spawnIndex, 0, lowGround.Length - 1)];
    }

    public Transform[] GetHighPathRandom()
    {
        int randomIndex = Random.Range(0, highGround.Length);
        return highGround[randomIndex];
    }

    public Transform[] GetHighPathByIndex(int index)
    {
        return highGround[Mathf.Clamp(index, 0, highGround.Length - 1)];
    }
}
