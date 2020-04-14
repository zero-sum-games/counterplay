using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    public enum TileState
    {
        Default     = 0,
        Selected    = 1,
        Targeted    = 2,
        Current     = 3
    }

    public TileState state = TileState.Default;

    public enum TileType
    {
        Grassland   = 0,
        Forest      = 1,
        Lake        = 2,
        Mountain    = 3
    }

    public TileType type;

    private Material _material;

    public List<Tile> adjMovementList = new List<Tile>();
    public List<Tile> adjAttackList = new List<Tile>();

    public bool visited = false;
    public Tile parent = null;

    private int _attackCost = 0;

    private float _movementCost = 0.0f;
    private float[] _movementCostsPerTileType;

    private Renderer _renderer;

    private GameObject _unitSelector;
    private GameObject _movementSelector;
    private GameObject _combatSelector;

    private void Awake()
    {
        LoadSelectors();
    }

    private void Start()
    {
        _renderer = GetComponent<Renderer>();
    }

    public void Reset(bool resetMovement, bool resetCombat)
    {
        if (resetMovement)
            adjMovementList.Clear();

        if (resetCombat)
            adjAttackList.Clear();

        state = TileState.Default;

        visited = false;
        parent = null;

        _attackCost = 0;

        _movementCost = 0.0f;
        _movementCostsPerTileType = new float[] {};
    }

    /// <summary>
    /// Use for computing tiles' adjacency lists for combat.
    /// </summary>
    public void FindNeighbors()
    {
        Reset(false, true);

        CheckTile(Vector3.forward);
        CheckTile(Vector3.back);
        CheckTile(Vector3.right);
        CheckTile(Vector3.left);
    }

    /// <summary>
    /// Use for computing tiles' adjacency lists for movement.
    /// </summary>
    /// <param name="elementalState">Used to inform a tile to add adjacent tiles only if they are traversible by this elemental state.</param>
    public void FindNeighbors(UnitState.ElementalState elementalState)
    {
        Reset(true, false);

        CheckTile(Vector3.forward, elementalState);
        CheckTile(Vector3.back, elementalState);
        CheckTile(Vector3.right, elementalState);
        CheckTile(Vector3.left, elementalState);
    }

    private void CheckTile(Vector3 direction)
    {
        var halfExtents = new Vector3(0.25f, 0.5f, 0.25f);
        var colliders = Physics.OverlapBox(transform.position + direction, halfExtents);

        foreach (var item in colliders)
        {
            var tile = item.GetComponent<Tile>();
            if (tile != null)
                adjAttackList.Add(tile);
        }
    }

    // TODO: remove the double functions at some point since movement is calculated on cost basis... combine the functions
    // somehow but keep the notion of adding to movement and attack adjacency lists separate
    private void CheckTile(Vector3 direction, UnitState.ElementalState elementalState)
    {
        var halfExtents = new Vector3(0.25f, 0.5f, 0.25f);
        var colliders = Physics.OverlapBox(transform.position + direction, halfExtents);

        foreach (var item in colliders)
        {
            var tile = item.GetComponent<Tile>();
            if (tile != null)
                if (!Physics.Raycast(tile.transform.position, Vector3.up, out _, 1))
                    adjMovementList.Add(tile);
        }
    }

    public int GetAttackCost() { return _attackCost; }

    public void SetAttackCost(int parentAttackCost)
    {
        _attackCost = parentAttackCost + 1;
    }

    public float GetMovementCost() { return _movementCost; }

    public void SetMovementCost(float parentMovementCost)
    {
        if (_movementCostsPerTileType == null) return;

        // ** Make sure the tile type enum values correspond with the values set in costs and shit **
        float costToNextTile = _movementCostsPerTileType[(int) type];

        _movementCost = parentMovementCost + costToNextTile;
    }

    public void CalculateMovementCostsPerTileType(UnitState.ElementalState elementalState)
    {
        // ** [0 = Grassland, 1 = Forest, 2 = Lake, 3 = Mountain] **
        // ** Use this ^^ when inputting values below for each elemental state **

        switch(elementalState)
        {
            default:
            case UnitState.ElementalState.Grass:
                _movementCostsPerTileType = new float[] { 0.5f, 1.0f, 0.5f, 100.0f };
                break;

            case UnitState.ElementalState.Water:
                _movementCostsPerTileType = new float[] { 0.5f, 1.0f, 0.25f, 100.0f };
                break;

            case UnitState.ElementalState.Fire:
                _movementCostsPerTileType = new float[] { 0.5f, 1.0f, 100.0f, 0.25f };
                break;
        }
    }

    public void SetMaterial(Material material)
    {
        _material = material;
        _renderer.material = _material;
    }

    public void SetActiveSelectors(bool setMovement, bool setCombat, bool setUnit)
    {
        _movementSelector.SetActive(setMovement);
        _combatSelector.SetActive(setCombat);
        _unitSelector.SetActive(setUnit);
    }

    public void LoadSelectors()
    {
        GameObject GetSelector(int selectorID)
        {
            var selectorVector3 = new Vector3(this.transform.position.x, 0.55f, this.transform.position.z);

            GameObject selector;

            switch(selectorID)
            {
                // MOVEMENT
                default:
                case 0:
                    selector = Instantiate(Resources.Load("MovementSelector"), selectorVector3, new Quaternion()) as GameObject;
                    break;

                // COMBAT
                case 1:
                    selector = Instantiate(Resources.Load("CombatSelector"), selectorVector3, new Quaternion()) as GameObject;
                    break;

                // UNIT
                case 2:
                    selector = Instantiate(Resources.Load("UnitSelector"), selectorVector3, new Quaternion()) as GameObject;
                    break;

            }
            selector.SetActive(false);
            selector.transform.parent = this.transform;

            return selector;
        }

        _movementSelector   = GetSelector(0);
        _combatSelector     = GetSelector(1);
        _unitSelector       = GetSelector(2);
    }
        
    public void OnValidate()
    {
        LoadMaterial();
    }

    public void LoadMaterial()
    {
        switch (type)
        {
            default:
            case TileType.Grassland:
                _material = Resources.Load<Material>("Tiles/Materials/Grassland");
                break;

            case TileType.Lake:
                _material = Resources.Load<Material>("Tiles/Materials/Lake");
                break;

            case TileType.Forest:
                _material = Resources.Load<Material>("Tiles/Materials/Forest");
                break;

            case TileType.Mountain:
                _material = Resources.Load<Material>("Tiles/Materials/Mountain");
                break;
        }

        if (_renderer == null)
            _renderer = this.GetComponent<Renderer>();
        _renderer.material = _material;
    }
}