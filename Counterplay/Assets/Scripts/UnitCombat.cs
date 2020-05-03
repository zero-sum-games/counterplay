using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//==============================================================================
public class UnitCombat : MonoBehaviour
{
    //==========================================================================
    public enum CombatState
    {
        Idle            = 0,
        Selected        = 1,
        Attacking       = 2,
        HasAttacked     = 3,
        Dead            = 4
    }

    public CombatState state = CombatState.Idle;

    protected int _teamID;
    public int GetTeamID() { return _teamID; }

    protected readonly List<Tile> _tilesInRange = new List<Tile>();
    private GameObject[] _tiles;

    protected Tile _currentTile;
    protected Tile _targetTile;
    public TypeModifier atkMod;

    protected GameObject _target;

    private int _attackBudget = 2;
    private int[] _attackBudgetsPerTileType;

    public int health;
    public int maxHealth;
    public int previousHealth;

    public Text deathText;

    public Transform healthBar;
    public Transform healthBarRotation;
    public Slider healthFill;
    protected float _healthBarXOffset = 0.0f;
    protected float _healthBarYOffset = 1.2f;

    protected bool _displayForCombatSelection = false;
    public void SetDisplayForCombatSelection(bool shouldDisplayForCombatSelection) { _displayForCombatSelection = shouldDisplayForCombatSelection; }

    //==========================================================================
    protected void Init()
    {
        _tiles = GameObject.FindGameObjectsWithTag("Tile");

        _teamID = transform.parent.gameObject.GetComponent<TeamManager>().teamID;

        atkMod = GameObject.Find("GameManager").GetComponent<TypeModifier>();
    }

    //==========================================================================
    protected void FindTilesInRange()
    {
        ComputeAdjacencyLists();

        if (Physics.Raycast(transform.position, Vector3.down, out var hit, 1))
            _currentTile = hit.collider.GetComponent<Tile>();
        _currentTile.visited = true;
        _currentTile.state = Tile.TileState.Current;
        _currentTile.SetActiveSelectors(false, false, true);

        SetAttackBudgetsPerTileType(_currentTile.type);
        SetAttackBudget(this.GetComponent<PlayerState>().GetElementalState());

        var process = new Queue<Tile>();
        process.Enqueue(_currentTile);

        while (process.Count > 0)
        {
            var t = process.Dequeue();

            if (t.GetAttackCost() > _attackBudget)
                continue;

            _tilesInRange.Add(t);

            if(t != _currentTile)
            {
                t.SetActiveSelectors(false, true, false);
                t.state = Tile.TileState.Selected;
            }

            foreach (var tile in t.adjAttackList)
            {
                if (tile.visited)
                    continue;

                tile.parent = t;
                tile.visited = true;

                tile.SetAttackCost(t.GetAttackCost());

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

    public void SetAttackBudget(UnitState.ElementalState elementalState)
    {
        switch (elementalState)
        {
            default:
            case UnitState.ElementalState.Grass:
                _attackBudget = _attackBudgetsPerTileType[0];
                break;

            case UnitState.ElementalState.Water:
                _attackBudget = _attackBudgetsPerTileType[1];
                break;

            case UnitState.ElementalState.Fire:
                _attackBudget = _attackBudgetsPerTileType[2];
                break;
        }
    }

    private void SetAttackBudgetsPerTileType(Tile.TileType tileType)
    {
        // _attackBudgetsPerTileType = [0 = Grass, 1 = Water, 2 = Fire]
        // Use this ^^ when inputting values below for each elemental state

        int tileTypeIndex = (int) tileType;
        _attackBudgetsPerTileType = new int[] { atkMod.types[0].atkRange[tileTypeIndex], atkMod.types[1].atkRange[tileTypeIndex], atkMod.types[2].atkRange[tileTypeIndex] };
    }

    //==========================================================================
    protected void RemoveSelectedTiles()
    {
        foreach (var tile in _tilesInRange)
            if (tile != _currentTile)
            {
                tile.SetActiveSelectors(false, false, false);
                tile.Reset(false, true);
            }

        _tilesInRange.Clear();
    }
}
