using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UnitMove : MonoBehaviour
{   
    public enum MoveState
    {
        Idle        = 0,
        Selected    = 1,
        Moving      = 2,
        Moved       = 3
    }

    public MoveState state = MoveState.Idle;

    protected int _teamID;

    protected readonly List<Tile> _selectedTiles = new List<Tile>();
    private GameObject[] _tiles;

    protected readonly Stack<Tile> _path = new Stack<Tile>();
    protected Tile _currentTile;

    private float _movementBudget = 1.0f;
    public float speed = 2.0f;

    private Vector3 _velocity;
    private Vector3 _heading;

    private float _halfUnitHeight;

    protected void Init()
    {
        _tiles = GameObject.FindGameObjectsWithTag("Tile");
        _halfUnitHeight = GetComponent<Collider>().bounds.extents.y;

        _teamID = transform.parent.gameObject.GetComponent<TeamManager>().teamID;
    }

    protected void FindAndSelectTiles()
    {
        ComputeAdjacencyLists();

        var process = new Queue<Tile>();

        _currentTile = GetCurrentTile();
        if (_currentTile == null) return; 
        
        _currentTile.visited = true;
        _currentTile.SetActiveSelectors(false, false, true);
        
        process.Enqueue(_currentTile);

        while (process.Count > 0)
        {
            var t = process.Dequeue();

            _selectedTiles.Add(t);

            if (t != _currentTile)
            {
                t.state = Tile.TileState.Selected;
                t.SetActiveSelectors(true, false, false);
            }

            if (t.GetMovementCost() >= _movementBudget) 
                continue;

            foreach (var tile in t.adjMovementList.Where(tile => !tile.visited))
            {
                tile.parent = t;
                tile.visited = true;

                tile.CalculateMovementCostsPerTileType(this.GetComponent<PlayerState>().GetElementalState());
                tile.SetMovementCost(t.GetMovementCost());

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

                transform.rotation = Quaternion.Euler(0.0f, 0.0f, 0.0f);
            }
            else
            {
                transform.position = target;
                _path.Pop();
            }
        }
        else
        {
            state = MoveState.Moved;
        }
    }

    protected void MoveToTile(Tile tile)
    {
        _path.Clear();

        tile.state = Tile.TileState.Targeted;
        state = MoveState.Moving;

        ResetTiles();

        var next = tile;
        while (next != null)
        {
            _path.Push(next);
            next = next.parent;
        }
    }

    private void ResetTiles()
    {
        foreach(var tile in _tiles)
        {
            var t = tile.GetComponent<Tile>();
            t.state = Tile.TileState.Default;

            if (t == _currentTile)
                t.SetActiveSelectors(true, false, false);
            else
                t.SetActiveSelectors(false, false, false);
        }
    }

    private void ComputeAdjacencyLists()
    {
        // for dynamically added / deleted tiles, make sure to reset tiles list object here
        foreach (var tile in _tiles)
        {
            var t = tile.GetComponent<Tile>();
            t.FindNeighbors(this.gameObject.GetComponent<PlayerState>().GetElementalState());
        }
    }

    public Tile GetCurrentTile()
    {
        var tile = GetTargetTile(gameObject);

        if (tile != null)
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

    protected void RemoveSelectedTiles()
    {
        foreach (var tile in _selectedTiles)
            if (tile != _currentTile)
            {
                tile.SetActiveSelectors(false, false, false);
                tile.Reset(true, false);
            }

        _selectedTiles.Clear();
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
