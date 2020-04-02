using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UnitCombat : MonoBehaviour
{
    public enum CombatState
    {
        Idle        = 0,
        Selected    = 1,
        Attacking   = 2,
        Attacked    = 3,
        Dead        = 4
    }

    public CombatState state = CombatState.Idle;

    protected int _teamID;

    public int GetTeamID() { return _teamID; }

    private readonly List<Tile> _tilesInRange = new List<Tile>();
    private GameObject[] _tiles;

    private Tile _currentTile;

    protected GameObject _target;

    public int attackRange = 1;

    public int health;
    public int maxHealth;
    public Transform healthBar;
    public Slider healthFill;
    protected float _healthBarYOffset = 1.0f;

    protected void Init()
    {
        _tiles = GameObject.FindGameObjectsWithTag("Tile");

        _teamID = transform.parent.gameObject.GetComponent<TeamManager>().teamID;

        SetAttackRange((int) this.GetComponent<PlayerState>().GetElementalState());
    }

    protected GameObject GetTarget()
    {
        if (_tilesInRange != null)
            foreach(var tile in _tilesInRange)
            {
                var t = tile.transform;
                if (Physics.Raycast(t.position, Vector3.up, out var hit, 1))
                    if (hit.collider.gameObject.GetComponent<PlayerCombat>().GetTeamID() != _teamID)
                        return hit.collider.gameObject;
            }
        return null;
    }

    protected void FindTilesInRange()
    {
        ComputeAdjacencyLists();

        if (Physics.Raycast(transform.position, Vector3.down, out var hit, 1))
            _currentTile = hit.collider.GetComponent<Tile>();
        _currentTile.visited = true;
        _currentTile.state = Tile.TileState.Current;

        var process = new Queue<Tile>();
        process.Enqueue(_currentTile);

        while (process.Count > 0)
        {
            var t = process.Dequeue();

            _tilesInRange.Add(t);

            if(t != _currentTile)
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
        {
            _currentTile.state = Tile.TileState.Default;
            _currentTile = null;
        }

        foreach (var tile in _tilesInRange)
            tile.Reset(false, true);

        _tilesInRange.Clear();
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
