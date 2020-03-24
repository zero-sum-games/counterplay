using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UnitMove : MonoBehaviour
{   
    protected enum MoveState
    {
        Idle        = 0,
        Selected    = 1,
        Moving      = 2
    }

    protected MoveState State = MoveState.Idle;

    private readonly List<Tile> _selectableTiles = new List<Tile>();
    private GameObject[] _tiles;

    private readonly Stack<Tile> _path = new Stack<Tile>();
    private Tile _currentTile;

    public int range = 5;
    public float speed = 2.0f;

    private Vector3 _velocity;
    private Vector3 _heading;

    private float _halfUnitHeight;

    public void SetRange(int elementalState)
    {
        switch (elementalState)
        {
            // Grass
            default:
            case 0:
                range = 5;
                break;
            
            // Water
            case 1:
                range = 3;
                break;

            // Fire
            case 2:
                range = 4;
                break;
            
        }
    }

    protected void Init()
    {
        _tiles = GameObject.FindGameObjectsWithTag("Tile");
        _halfUnitHeight = GetComponent<Collider>().bounds.extents.y;
    }

    protected void FindSelectableTiles()
    {
        ComputeAdjacencyLists();

        var process = new Queue<Tile>();

        _currentTile = GetCurrentTile();
        _currentTile.visited = true;
        process.Enqueue(_currentTile);

        while (process.Count > 0)
        {
            var t = process.Dequeue();

            // TODO: rename the SELECTED state to SELECTABLE (?)
            _selectableTiles.Add(t);
            if (t != _currentTile)
                t.state = Tile.TileState.Selected;

            if (t.distance >= range) 
                continue;

            foreach (var tile in t.adjacencyList.Where(tile => !tile.visited))
            {
                tile.parent = t;
                tile.visited = true;
                tile.distance = t.distance + 1;

                process.Enqueue(tile);
            }
        }
    }

    protected void Move()
    {
        if (_path.Count > 0)
        {
            var t = _path.Peek();

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

    protected void MoveToTile(Tile tile)
    {
        _path.Clear();

        tile.state = Tile.TileState.Targeted;
        State = MoveState.Moving;
        
        var next = tile;
        while (next != null)
        {
            _path.Push(next);
            next = next.parent;
        }
    }

    private void ComputeAdjacencyLists()
    {
        // for dynamically added / deleted tiles, make sure to reset tiles list object here
        foreach (var tile in _tiles)
        {
            var t = tile.GetComponent<Tile>();
            t.FindNeighbors(this);
        }
    }

    private Tile GetCurrentTile()
    {
        var tile = GetTargetTile(gameObject);
        tile.state = Tile.TileState.Current;

        return tile;
    }

    private static Tile GetTargetTile(GameObject target)
    {
        Tile tile = null;

        if (Physics.Raycast(target.transform.position, Vector3.down, out var hit, 1))
            tile = hit.collider.GetComponent<Tile>();

        return tile;
    }

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

    private void SetHeading(Vector3 target)
    {
        _heading = target - transform.position;
        _heading.Normalize();
    }

    private void SetHorizontalVelocity()
    {
        _velocity = _heading * speed;
    }
}
