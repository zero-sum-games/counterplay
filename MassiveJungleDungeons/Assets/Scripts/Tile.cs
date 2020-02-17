using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//==============================================================================

public class Tile : MonoBehaviour
{
    //==========================================================================

    public enum TileState
    {
        DEFAULT     = 0,
        SELECTED    = 1,
        TARGETED    = 2,
        CURRENT     = 3
    }

    // indicates the state of a tile using the enum defined above ^^
    public TileState state = TileState.DEFAULT;

    public enum TileType
    {
        GRASS       = 0,
        WATER       = 1,
        FOREST      = 2,
        MOUNTAIN    = 3
    }

    public TileType type = TileType.GRASS;

    Material material;

    //==========================================================================

    // represents the tiles that are adjacent to a certain tile
    public List<Tile> adjacencyList = new List<Tile>();

    // breadth-first search variables
    public bool visited = false;
    public Tile parent = null;
    public int distance = 0;

    //==========================================================================

    // check all four neighbors and add to adjacency list if appropriate
    public void FindNeighbors()
    {
        Reset();

        CheckTile(Vector3.forward);
        CheckTile(Vector3.back);
        CheckTile(Vector3.right);
        CheckTile(Vector3.left);
    }

    // reset all bfs variables, adjacency list, and tile state
    public void Reset()
    {
        state = TileState.DEFAULT;

        adjacencyList.Clear();
        visited = false;
        parent = null;
        distance = 0;
    }

    //==========================================================================

    // adds neighbor tile to current tile's adjacency list if there is nothing on top and it is walkable
    private void CheckTile(Vector3 direction)
    {
        var halfExtents = new Vector3(0.25f, 0.5f, 0.25f);
        Collider[] colliders = Physics.OverlapBox(transform.position + direction, halfExtents);

        foreach (Collider item in colliders)
        {
            Tile tile = item.GetComponent<Tile>();
            if (tile != null && tile.type == TileType.GRASS)
            {
                RaycastHit hit;
                if (!Physics.Raycast(tile.transform.position, Vector3.up, out hit, 1))
                    adjacencyList.Add(tile);
            }
        }
    }

    //==========================================================================

    private void Start()
    {
        switch(type)
        {
            default:
            case TileType.GRASS:
                material = Resources.Load<Material>("Tiles/Grass");
                break;

            case TileType.WATER:
                material = Resources.Load<Material>("Tiles/Water");
                break;

            case TileType.FOREST:
                material = Resources.Load<Material>("Tiles/Forest");
                break;

            case TileType.MOUNTAIN:
                material = Resources.Load<Material>("Tiles/Mountain");
                break;
        }

        GetComponent<Renderer>().material = material;
    }

    private void Update()
    {
        var renderer = GetComponent<Renderer>();
        switch (state)
        {
            default:
            case TileState.DEFAULT:
                renderer.material = material;
                break;

            case TileState.SELECTED:
                renderer.material.color = Color.red;
                break;

            case TileState.TARGETED:
                renderer.material.color = Color.green;
                break;

            case TileState.CURRENT:
                renderer.material.color = Color.magenta;
                break;
        }
    }
}