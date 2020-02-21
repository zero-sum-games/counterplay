using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//==============================================================================

public class UnitMove : MonoBehaviour
{
    //==========================================================================

    protected enum MoveState
    {
        Idle        = 0,
        Selecting   = 1,
        Moving      = 2
    }

    protected MoveState State = MoveState.Idle;

    //==========================================================================

    List<Tile> selectableTiles = new List<Tile>();
    GameObject[] _tiles;

    Stack<Tile> path = new Stack<Tile>();
    Tile _currentTile;

    public int range = 5;
    public float speed = 2.0f;

    Vector3 _velocity = new Vector3();
    Vector3 _heading = new Vector3();

    private float _halfUnitHeight;
    //==========================================================================

    // booleans for if the player state allows terrain to be walkable
    /*public bool moveToGrassland = true;
    public bool moveToLake = false;
    public bool moveToForest = false;
    public bool moveToMountain = false;*/
    
    //==========================================================================

    // Each state's parameters
    // Now also determines which tiles are walkable for each state
    public void SetRange(int elementalState)
    {
        switch (elementalState)
        {
            // Fire
            default:
            case 0:
                range = 2;
                /*moveToGrassland = true;
                moveToForest = true;
                moveToLake = false;
                moveToMountain = true;*/
                break;

            // Water
            case 1:
                range = 3;
                /*moveToGrassland = true;
                moveToForest = true;
                moveToLake = true;
                moveToMountain = false;*/
                break;

            // Grass
            case 2:
                range = 4;
                /*moveToGrassland = true;
                moveToForest = true;
                moveToLake = true;
                moveToMountain = false;*/
                break;
        }
    }

    //==========================================================================

    // initialize tile array and halfUnitHeight
    protected void Init()
    {
        _tiles = GameObject.FindGameObjectsWithTag("Tile");

        _halfUnitHeight = GetComponent<Collider>().bounds.extents.y;
    }

    // find all tiles within movable range (implements breadth-first search (bfs) algorithm)
    protected void FindSelectableTiles()
    {
        ComputeAdjacencyLists();

        Queue<Tile> process = new Queue<Tile>();

        _currentTile = GetCurrentTile();
        _currentTile.visited = true;
        process.Enqueue(_currentTile);

        while (process.Count > 0)
        {
            // get next tile in queue
            Tile t = process.Dequeue();

            // add the tile to selectable tiles list (and change tile state only if it's not the current tile)
            // we should rename the SELECTED state to SELECTABLE
            selectableTiles.Add(t);
            if (t != _currentTile)
                t.state = Tile.TileState.Selected;

            // if tile is still within range of unit
            if (t.distance < range)
            {
                // then for every unvisited tile in the current tile's adjacency list, add it to the queue
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
            target.y += _halfUnitHeight + t.GetComponent<Collider>().bounds.extents.y;

            if (Vector3.Distance(transform.position, target) >= 0.05f)
            {
                SetHeading(target);
                SetHorizontalVelocity();

                Transform transform1 = transform;
                transform1.forward = _heading;
                transform1.position += _velocity * Time.deltaTime;
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
            State = MoveState.Idle;
        }
    }

    // move to the given tile
    protected void MoveToTile(Tile tile)
    {
        // clear any existing path
        path.Clear();

        // update tile and move states
        tile.state = Tile.TileState.Targeted;
        State = MoveState.Moving;
        
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

        foreach (GameObject tile in _tiles)
        {
            Tile t = tile.GetComponent<Tile>();
            t.FindNeighbors(this);
        }
    }

    // get tile that is currently under this gameObject
    private Tile GetCurrentTile()
    {
        Tile tile = GetTargetTile(gameObject);
        tile.state = Tile.TileState.Current;

        return tile;
    }

    // get tile that is current under target gameObject
    private static Tile GetTargetTile(GameObject target)
    {
        Tile tile = null;

        if (Physics.Raycast(target.transform.position, Vector3.down, out RaycastHit hit, 1))
            tile = hit.collider.GetComponent<Tile>();

        return tile;
    }

    // deselect all currently selected tiles
    private void RemoveSelectableTiles()
    {
        if (_currentTile != null)
        {
            _currentTile.state = Tile.TileState.Default;
            _currentTile = null;
        }

        foreach (Tile tile in selectableTiles)
            tile.Reset();

        selectableTiles.Clear();
    }

    // set heading based off of target vector and current position
    private void SetHeading(Vector3 target)
    {
        _heading = target - transform.position;
        _heading.Normalize();
    }

    // set velocity with speed and updated heading vector
    private void SetHorizontalVelocity()
    {
        _velocity = _heading * speed;
    }
}
