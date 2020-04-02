using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitCombat : MonoBehaviour
{
    public enum CombatState
    {
        Idle        = 0,
        Selected    = 1,
        Attacking   = 2,
        Attacked    = 3
    }

    public CombatState state = CombatState.Idle;

    protected int _teamID;

    private readonly List<Tile> _selectedTiles = new List<Tile>();
    private GameObject[] _tiles;

    private readonly Stack<Tile> _path = new Stack<Tile>();
    private Tile _currentTile;

    public int attackRange = 1;

    protected void Init()
    {
        _tiles = GameObject.FindGameObjectsWithTag("Tile");

        _teamID = transform.parent.gameObject.GetComponent<TeamManager>().teamID;

        SetAttackRange((int)this.GetComponent<PlayerState>().GetElementalState());
    }

    protected void FindAndSelectTiles()
    {
        ComputeAdjacencyLists();

        var process = new Queue<Tile>();

        if (Physics.Raycast(transform.position, Vector3.down, out var hit, 1))
            _currentTile = hit.collider.GetComponent<Tile>();
        _currentTile.visited = true;
        process.Enqueue(_currentTile);

        while (process.Count > 0)
        {
            var t = process.Dequeue();

            _selectedTiles.Add(t);

            if (t != _currentTile)
                t.state = Tile.TileState.Selected;

            if (t.distance >= attackRange)
                continue;

            foreach (var tile in t.adjAttackList)
            {
                if (tile.visited)
                    continue;

                tile.parent = t;
                tile.visited = true;
                tile.distance = t.distance + 1;

                process.Enqueue(tile);
            }
        }
    }

    private void ComputeAdjacencyLists()
    {
        foreach (var tile in _tiles)
        {
            var t = tile.GetComponent<Tile>();
            t.FindNeighbors();
        }
    }

    protected void RemoveSelectedTiles()
    {
        if (_currentTile != null)
            _currentTile = null;

        foreach (var tile in _selectedTiles)
            tile.Reset(false, true);

        _selectedTiles.Clear();
    }

    public void SetAttackRange(int elementalState)
    {
        switch (elementalState)
        {
            // Grass
            default:
            case 0:
                attackRange = 2;
                break;

            // Water
            case 1:
                attackRange = 3;
                break;

            // Fire
            case 2:
                attackRange = 1;
                break;
        }
    }
}
