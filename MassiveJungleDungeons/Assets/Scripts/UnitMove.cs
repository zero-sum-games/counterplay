using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

    private readonly List<Tile> _selectableTiles = new List<Tile>();
    private GameObject[] _tiles;

    private readonly Stack<Tile> _path = new Stack<Tile>();
    private Tile _currentTile;

    public int range = 5;
    public float speed = 2.0f;

    private Vector3 _velocity;
    private Vector3 _heading;

    private float _halfUnitHeight;
    //==========================================================================

    // Each state's parameters
    public void SetRange(int elementalState)
    {
        switch (elementalState)
        {
            // Grass
            default:
            case 0:
                range = 3;
                break;
            
            // Water
            case 1:
                range = 2;
                break;

            // Fire
            case 2:
                range = 2;
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

        var process = new Queue<Tile>();

        _currentTile = GetCurrentTile();
        _currentTile.visited = true;
        process.Enqueue(_currentTile);

        while (process.Count > 0)
        {
            // get next tile in queue
            var t = process.Dequeue();

            // add the tile to selectable tiles list (and change tile state only if it's not the current tile)
            // we should rename the SELECTED state to SELECTABLE
            _selectableTiles.Add(t);
            if (t != _currentTile)
                t.state = Tile.TileState.Selected;

            // if tile is still within range of unit
            if (t.distance >= range) continue;
            // then for every unvisited tile in the current tile's adjacency list, add it to the queue
            foreach (var tile in t.adjacencyList.Where(tile => !tile.visited))
            {
                tile.parent = t;
                tile.visited = true;
                tile.distance = t.distance + 1;

                process.Enqueue(tile);
            }
        }
    }

    // move from tile to next tile in path
    protected void Move()
    {
        if (_path.Count > 0)
        {
            // get next tile from path
            var t = _path.Peek();

            // calculate unit's position on top of target tile
            var target = t.transform.position;
            target.y += _halfUnitHeight + t.GetComponent<Collider>().bounds.extents.y;

            if (Vector3.Distance(transform.position, target) >= 0.05f)
            {
                SetHeading(target);
                SetHorizontalVelocity();

                var transform1 = transform;
                transform1.forward = _heading;
                transform1.position += _velocity * Time.deltaTime;
            }
            else
            {
                // the tile has been reached
                transform.position = target;
                _path.Pop();
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
        _path.Clear();

        // update tile and move states
        tile.state = Tile.TileState.Targeted;
        State = MoveState.Moving;
        
        // add specific sequence of tiles to path
        var next = tile;
        while (next != null)
        {
            _path.Push(next);
            next = next.parent;
        }
    }

    //==========================================================================

    // update adjacency lists of all current map tiles
    private void ComputeAdjacencyLists()
    {
        // **NOTE: For dynamically added / deleted tiles, make sure to reset tiles list object here**

        foreach (var tile in _tiles)
        {
            var t = tile.GetComponent<Tile>();
            t.FindNeighbors(this);
        }
    }

    // get tile that is currently under this gameObject
    private Tile GetCurrentTile()
    {
        var tile = GetTargetTile(gameObject);
        tile.state = Tile.TileState.Current;

        return tile;
    }

    // get tile that is current under target gameObject
    private static Tile GetTargetTile(GameObject target)
    {
        Tile tile = null;

        if (Physics.Raycast(target.transform.position, Vector3.down, out var hit, 1))
            tile = hit.collider.GetComponent<Tile>();

        return tile;
    }

    // deselect all currently selected tiles
    protected void RemoveSelectableTiles()
    {
        if (_currentTile != null)
        {
            _currentTile.state = Tile.TileState.Default;
            _currentTile = null;
        }

        foreach (var tile in _selectableTiles)
            tile.Reset();

        _selectableTiles.Clear();
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
