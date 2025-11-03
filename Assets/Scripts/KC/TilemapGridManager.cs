using UnityEngine;
using UnityEngine.Tilemaps;

public class TilemapGridManager : MonoBehaviour
{
    public Tilemap tilemap;
    public TileBase blockedTile;
    public TileBase availableTile;

    private Camera mainCamera;

    private void Start()
    {
        mainCamera = Camera.main;
    }

    //Check if a tile is blocked
    public bool IsTileBlocked(Vector3 worldPosition)
    {
        Vector3Int gridPosition = tilemap.WorldToCell(worldPosition);
        TileBase tile = tilemap.GetTile(gridPosition);  //Get the tile at this position

        return tile == blockedTile;  //If the tile is blocked = return true
    }

    //Convert world position to grid position
    public Vector3Int GetGridPosition(Vector3 worldPosition)
    {
        return tilemap.WorldToCell(worldPosition);
    }

    //Get the world position of the tile
    public Vector3 GetTileWorldPosition(Vector3Int gridPosition)
    {
        return tilemap.GetCellCenterWorld(gridPosition);
    }
}
