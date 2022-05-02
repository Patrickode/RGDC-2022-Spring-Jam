using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Originally made by Mason Hayes and adjusted by Patrick Mitchell/Kesavan Shanmugasundaram for 603 Game 3
/// </summary>
public class GridTile : MonoBehaviour
{
#if UNITY_EDITOR
    [Serializable]
    public struct DirTilePair
    {
        public Vector3Int dir;
        public GridTile tile;
        public DirTilePair(Vector3Int index, GridTile tile) { this.dir = index; this.tile = tile; }
    }

    [Header("Editor Only Fields")]
    [Tooltip("List of the Dict's values so they can be seen in the inspector; clockwise, starting from LeftForward.")]
    [SerializeField] private List<DirTilePair> adjacentTiles = new List<DirTilePair>();
#endif
    [Header("Normal Fields")]
    [SerializeField] private Collider tileCollider;
    [SerializeField] private Renderer tileRenderer;
    [SerializeField] private Material unselectedMat;
    [SerializeField] private Material selectedMat;

    private Dictionary<Vector3Int, GridTile> tileAdj = new Dictionary<Vector3Int, GridTile>();
    private GameObject occupant;

    //--- Methods ---//
    private void OnEnable()
    {
        ConstructionManager.cursorStateChange += OnPlacingStateChange;
        ConstructionManager.tileHover += OnTileHover;
        ConstructionManager.buildOnTile += OnBuildOnTile;
        ConstructionManager.destroyTileBuilding += OnDestroyTileBuilding;
    }
    private void OnDisable()
    {
        ConstructionManager.cursorStateChange -= OnPlacingStateChange;
        ConstructionManager.tileHover -= OnTileHover;
        ConstructionManager.buildOnTile -= OnBuildOnTile;
        ConstructionManager.destroyTileBuilding -= OnDestroyTileBuilding;
    }

    private void OnPlacingStateChange(CursorState newState)
    {
        switch (newState)
        {
            case CursorState.Building:
                //Enable this tile's rend/coll if not occupied.
                ToggleTilePresence(!occupant);
                break;
            case CursorState.Harvesting:
                //Enable this tile's rend/coll if this tile has an occupant.
                ToggleTilePresence(occupant);
                break;
            case CursorState.Neutral:
            default:
                tileRenderer.enabled = false;
                break;
        }
    }

    private void ToggleTilePresence(bool enabled)
    {
        //tileCollider.enabled = enabled;
        tileCollider.enabled = enabled;

        tileRenderer.enabled = enabled;
    }

    private void OnTileHover(GameObject tileHovered, bool hovered)
    {
        if (tileHovered == gameObject)
        {
            tileRenderer.material = hovered ? selectedMat : unselectedMat;
        }
    }

    private void OnBuildOnTile(GameObject tile, GameObject buildPrefab, Vector3 buildOffset)
    {
        //Only build if this tile is the one being built on *AND* is not already occupied
        if (tile == gameObject && !occupant)
        {
            occupant = Instantiate(buildPrefab, transform);
            occupant.transform.NegateParentScale();
            occupant.transform.position += buildOffset;
        }
    }

    private void OnDestroyTileBuilding(GameObject tile)
    {
        if (tile == gameObject && occupant)
        {
            Destroy(occupant);
            occupant = null;
        }
    }

    public void InitTileAdjacency(GridTile[,,] tileMatrix, Vector3Int thisIndex)
    {
        Vector3Int dir = Vector3Int.zero;
        for (int x = -1; x < 2; x++)
        {
            dir.x = x;
            for (int y = -1; y < 2; y++)
            {
                dir.y = y;
                for (int z = -1; z < 2; z++)
                {
                    dir.z = z;

                    if (dir == Vector3Int.zero)
                        continue;

                    tileAdj.Add(dir, null);
                    TrySetConnection(tileMatrix, thisIndex, dir);

#if UNITY_EDITOR
                    adjacentTiles.Add(new DirTilePair(dir, tileAdj[dir]));
#endif
                }
            }
        }
    }

    public void TrySetConnection(GridTile[,,] tileMatrix, Vector3Int index, Vector3Int connectDir)
    {
        //Add connect dir to this tile's index to get the index in the desired direction.
        index += connectDir;

        //Now check if that index is a valid one.
        if (index.x >= 0 && index.x < tileMatrix.GetLength(0))
        {
            if (index.y >= 0 && index.y < tileMatrix.GetLength(1))
            {
                if (index.z >= 0 && index.z < tileMatrix.GetLength(2))
                {
                    tileAdj[connectDir] = tileMatrix[index.x, index.y, index.z];
                }
            }
        }
    }

    public GridTile GetAdjTile(Vector3Int direction)
    {
        return tileAdj[direction];
    }
}