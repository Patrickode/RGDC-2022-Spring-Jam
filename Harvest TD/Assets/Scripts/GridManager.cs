using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Originally made by Mason Hayes and adjusted by Patrick Mitchell/Kesavan Shanmugasundaram for 603 Game 3
/// </summary>
public class GridManager : MonoBehaviour
{
    [SerializeField] private GridTile tilePrefab;
    [Tooltip("The dimensions of the grid.")]
    [SerializeField] [Min(1)] private Vector3Int gridSize;
    [Tooltip("The distance between each tile's center in the grid.")]
    [SerializeField] private Vector3 tileSpacing;
    [Tooltip("The distance between each tile's edge in the grid. If 0, a sphere centered on a tile would touch " +
        "its orthogonal neighbors.")]
    [SerializeField] [Min(0)] private Vector3 tilePadding;

    public Vector3Int GridSize { get => gridSize; }
    public Vector2 TileSpacing { get => tileSpacing; }

    private GridTile[,,] tiles = { };

    private void OnValidate() => ValidationUtility.SafeOnValidate(() => { if (this) InitGrid(); });
    private void Start() => InitGrid();

    private void InitGrid()
    {
        if (!tilePrefab)
        {
            Debug.LogError($"{name}'s GridManager needs a tile prefab to instantiate!");
            return;
        }

#if UNITY_EDITOR
        //Since we're initializing, the grid shouldn't have tile children. So, go through any that it does have...
        foreach (Transform child in transform)
            if (child.CompareTag("Tile"))
                //...and after all inspectors have updated, destroy them (assuming it's still valid to do so).
                UnityEditor.EditorApplication.delayCall += () =>
                {
                    if (this && child)
                        DestroyImmediate(child.gameObject);
                };
#endif
        tiles = new GridTile[gridSize.x, gridSize.y, gridSize.z];

        //Start from the bottom left; go forward, then go up, then go right.
        for (int x = 0; x < gridSize.x; x++)
        {
            for (int y = 0; y < gridSize.y; y++)
            {
                for (int z = 0; z < gridSize.z; z++)
                {
                    //Get the position of the tile, based on spacing and position in the array
                    Vector3 pos = new Vector3(tileSpacing.x * x, tileSpacing.y * y, tileSpacing.z * z);
                    GridTile tile = Instantiate(tilePrefab, pos + transform.position, Quaternion.identity, transform);

                    Vector3 paddedSpacing = tileSpacing - tilePadding;
                    tile.transform.localScale = Vector3.Scale(tile.transform.localScale, paddedSpacing);

                    tiles[x, y, z] = tile;
                    tile.name = $"Tile [{x}, {y}, {z}]";

                    //Once the final tile has been initialized, connect this tile (all the other ones will do this at the same time).
                    //  This declaration is important; the lambda will not cache xyz as they are now, it will use xyz as they are when
                    //  the final tile is init'd.
                    Vector3Int ind = new Vector3Int(x, y, z);
                    Coroutilities.DoWhen(this,
                        () => tile.InitTileAdjacency(tiles, ind),
                        () => tiles[gridSize.x - 1, gridSize.y - 1, gridSize.z - 1]);
                }
            }
        }
    }

    public GridTile GetTile(int xIndex, int yIndex, int zIndex) => tiles[xIndex, yIndex, zIndex];
    public GridTile GetTile(Vector3Int indices) => tiles[indices.x, indices.y, indices.z];

    public Vector3 GetGridLinePos(Vector3Int tileIndex, Vector3Int dir)
    {
        Vector3 halfSpaceInDir = Vector3.Scale(dir, tileSpacing / 2);

        return tiles[tileIndex.x, tileIndex.y, tileIndex.z].transform.position + Vector3.Scale(Vector3.one, halfSpaceInDir);
    }
}