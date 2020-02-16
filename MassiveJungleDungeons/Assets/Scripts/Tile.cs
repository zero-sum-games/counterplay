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

    // indicates the state of a tile
    public TileState state = TileState.DEFAULT;

    // true if a tile can be traversed on by a unit
    public bool isWalkable = true;

    //==========================================================================

    // represents the tiles that are adjacent to a certain tile
    public List<Tile> adjacencyList = new List<Tile>();

    // breadth-first search variables
    public bool visited = false;
    public Tile parent = null;
    public int distance = 0;

    //==========================================================================
    void Start()
    {
        
    }

    void Update()
    {
        switch(state)
        {
            default:
            case TileState.DEFAULT:
                GetComponent<Renderer>().material.color = Color.white;
                break;
            case TileState.SELECTED:
                GetComponent<Renderer>().material.color = Color.red;
                break;
            case TileState.TARGETED:
                GetComponent<Renderer>().material.color = Color.green;
                break;
            case TileState.CURRENT:
                GetComponent<Renderer>().material.color = Color.magenta;
                break;
        }
    }

    //==========================================================================

    // Adds neighbor tile to current tile's adjacency list if there is nothing on top and it is walkable
    public void CheckTile(Vector3 direction)
    {
        var halfExtents = new Vector3(0.25f, 0.5f, 0.25f);
        Collider[] colliders = Physics.OverlapBox(transform.position + direction, halfExtents);

        foreach (Collider item in colliders)
        {
            Tile tile = item.GetComponent<Tile>();
            if (tile != null && tile.isWalkable)
            {
                RaycastHit hit;
                if (!Physics.Raycast(tile.transform.position, Vector3.up, out hit, 1))
                    adjacencyList.Add(tile);
            }
        }
    }

    // Check all four neighbors and add to adjacency list if appropriate
    public void FindNeighbors()
    {
        Reset();

        CheckTile(Vector3.forward);
        CheckTile(Vector3.back);
        CheckTile(Vector3.right);
        CheckTile(Vector3.left);
    }

    // Reset all bfs variables, adjacency list, and tile state
    public void Reset()
    {
        state = TileState.DEFAULT;

        adjacencyList.Clear();
        visited = false;
        parent = null;
        distance = 0;
    }
}
