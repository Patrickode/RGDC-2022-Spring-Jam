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
    [Tooltip("The dimensions of the grid; y = z, since the grid is flat along the XZ plane.")]
    [SerializeField] [Min(1)] private Vector3Int gridSize;
    [Tooltip("The distance between each tile's center in the grid.")]
    [SerializeField] private Vector3 tileSpacing;
    [Tooltip("The distance between each tile's edge in the grid. If 0, a sphere centered on a tile would touch " +
        "its orthogonal neighbors.")]
    [SerializeField] [Min(0)] private Vector3 tilePadding;

    public Vector3Int GridSize { get => gridSize; }
    public Vector2 TileSpacing { get => tileSpacing; }

    private GridTile[,,] tiles;

    void Start()
    {
        if (!tilePrefab)
        {
            Debug.LogError($"{name}'s GridManager needs a tile prefab to instantiate!");
            return;
        }

        tiles = new GridTile[gridSize.x, gridSize.y, gridSize.z];

        //Instantiate all the tiles
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
                        () => { tile.InitTileAdjacency(tiles, ind); },
                        () => tiles[gridSize.x - 1, gridSize.y - 1, gridSize.z - 1]
                    );
                }
            }
        }
    }

    public GridTile GetTile(int xIndex, int yIndex, int zIndex) => tiles[xIndex, yIndex, zIndex];
    public GridTile GetTile(Vector3Int indices) => tiles[indices.x, indices.y, indices.z];

    public Vector3 GetGridLinePos(Vector3Int tileIndex, Vector3Int dir)
    {
        Vector3 halfSpaceInDir = Vector3.Scale(dir, tileSpacing / 2);
        return tiles[tileIndex.x, tileIndex.y, tileIndex.z].transform.position + new Vector3(halfSpaceInDir.x, halfSpaceInDir.y, halfSpaceInDir.z);
    }
}