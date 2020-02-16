using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//==============================================================================

public class UnitMove : MonoBehaviour
{
    //==========================================================================

    public enum MoveState
    {
        IDLE    = 0,
        MOVING  = 1
    }

    protected MoveState state = MoveState.IDLE;

    //==========================================================================

    List<Tile> selectableTiles = new List<Tile>();
    GameObject[] tiles;

    Stack<Tile> path = new Stack<Tile>();
    Tile currentTile;

    public int range = 5;
    public float speed = 2.0f;

    Vector3 velocity = new Vector3();
    Vector3 heading = new Vector3();

    private float halfUnitHeight;

    //==========================================================================

    // initialize tile array and halfUnitHeight
    protected void Init()
    {
        tiles = GameObject.FindGameObjectsWithTag("Tile");

        halfUnitHeight = GetComponent<Collider>().bounds.extents.y;
    }

    // find all tiles within movable range (implements breadth-first search (bfs) algorithm)
    protected void FindSelectableTiles()
    {
        ComputeAdjacencyLists();

        Queue<Tile> process = new Queue<Tile>();

        currentTile = GetCurrentTile();
        currentTile.visited = true;
        process.Enqueue(currentTile);

        while (process.Count > 0)
        {
            // get next tile in queue
            Tile t = process.Dequeue();

            // add the tile to selectable tiles list (and change tile state only if it's not the current tile)
            selectableTiles.Add(t);
            if (t != currentTile)
                t.state = Tile.TileState.SELECTED;

            // if tile is still within range of unit
            if (t.distance < range)
            {
                // then for every unvisited title in the current tile's adjacency list, add it to the queue
                foreach (Tile tile in t.adjacencyList)
                {
                    if (!tile.visited)
                    {
                        tile.parent = t;
                        tile.visited = true;
                        tile.distance = t.distance + 1;

                        process.Enqueue(tile);
                    }
                }
            }
        }
    }

    // move from tile to next tile in path
    protected void Move()
    {
        if (path.Count > 0)
        {
            // get next tile from path
            Tile t = path.Peek();

            // calculate unit's position on top of target tile
            Vector3 target = t.transform.position;
            target.y += halfUnitHeight + t.GetComponent<Collider>().bounds.extents.y;

            if (Vector3.Distance(transform.position, target) >= 0.05f)
            {
                SetHeading(target);
                SetHorizontalVelocity();

                transform.forward = heading;
                transform.position += velocity * Time.deltaTime;
            }
            else
            {
                // the tile has been reached
                transform.position = target;
                path.Pop();
            }
        }
        else
        {
            RemoveSelectableTiles();
            state = MoveState.IDLE;
        }
    }

    // move to the given tile
    protected void MoveToTile(Tile tile)
    {
        // clear any existing path
        path.Clear();

        // update tile and move states
        tile.state = Tile.TileState.TARGETED;
        state = MoveState.MOVING;

        // add specific sequence of tiles to path
        Tile next = tile;
        while (next != null)
        {
            path.Push(next);
            next = next.parent;
        }
    }

    //==========================================================================

    // update adjacency lists of all current map tiles
    private void ComputeAdjacencyLists()
    {
        // **NOTE: For dynamically added / deleted tiles, make sure to reset tiles list object here**

        foreach (GameObject tile in tiles)
        {
            Tile t = tile.GetComponent<Tile>();
            t.FindNeighbors();
        }
    }

    // get tile that is currently under this gameObject
    private Tile GetCurrentTile()
    {
        var tile = GetTargetTile(gameObject);
        tile.state = Tile.TileState.CURRENT;

        return tile;
    }

    // get tile that is current under target gameObject
    private Tile GetTargetTile(GameObject target)
    {
        Tile tile = null;

        RaycastHit hit;
        if (Physics.Raycast(target.transform.position, Vector3.down, out hit, 1))
            tile = hit.collider.GetComponent<Tile>();

        return tile;
    }

    // deselect all currently selected tiles
    private void RemoveSelectableTiles()
    {
        if (currentTile != null)
        {
            currentTile.state = Tile.TileState.DEFAULT;
            currentTile = null;
        }

        foreach (Tile tile in selectableTiles)
            tile.Reset();

        selectableTiles.Clear();
    }

    // set heading based off of target vector and current position
    private void SetHeading(Vector3 target)
    {
        heading = target - transform.position;
        heading.Normalize();
    }

    // set velocity with speed and updated heading vector
    private void SetHorizontalVelocity()
    {
        velocity = heading * speed;
    }
}
