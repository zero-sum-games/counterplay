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
        Lake        = 1,
        Forest      = 2,
        Mountain    = 3
    }

    public TileType type;

    private Material _material;

    public List<Tile> adjMovementList = new List<Tile>();
    public List<Tile> adjAttackList = new List<Tile>();

    public bool visited = false;
    public Tile parent = null;
    public int distance = 0;

    private Renderer _selectableRangeColor;

    public void Reset(bool resetMovement, bool resetCombat)
    {
        if (resetMovement)
            adjMovementList.Clear();

        if (resetCombat)
            adjAttackList.Clear();

        state = TileState.Default;

        visited = false;
        parent = null;
        distance = 0;
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
            {
                adjAttackList.Add(tile);
            }
        }
    }

    private void CheckTile(Vector3 direction, UnitState.ElementalState elementalState)
    {
        var halfExtents = new Vector3(0.25f, 0.5f, 0.25f);
        var colliders = Physics.OverlapBox(transform.position + direction, halfExtents);

        foreach (var item in colliders)
        {
            var tile = item.GetComponent<Tile>();
            if (tile != null)
            {
                if (!Physics.Raycast(tile.transform.position, Vector3.up, out _, 1))
                {
                    switch (elementalState)
                    {
                        default:
                        case UnitState.ElementalState.Grass:
                            if (tile.type == TileType.Grassland || tile.type == TileType.Forest)
                                adjMovementList.Add(tile);
                            break;

                        case UnitState.ElementalState.Water:
                            if (tile.type == TileType.Lake || tile.type == TileType.Grassland)
                                adjMovementList.Add(tile);
                            break;

                        case UnitState.ElementalState.Fire:
                            if (tile.type == TileType.Forest || tile.type == TileType.Grassland || tile.type == TileType.Mountain)
                                adjMovementList.Add(tile);
                            break;
                    }
                }
            }
        }
    }

    private void Start()
    {
        _selectableRangeColor = GetComponent<Renderer>();
    }

    public void SetMaterial(Material material)
    {
        _material = material;
        GetComponent<Renderer>().material = _material;
    }

    private void Update()
    {
        switch (state)
        {
            default:
            case TileState.Default:
                _selectableRangeColor.material = _material;
                break;

            case TileState.Selected:
                _selectableRangeColor.material.color = Color.red;
                break;

            case TileState.Targeted:
                _selectableRangeColor.material.color = Color.green;
                break;

            case TileState.Current:
                _selectableRangeColor.material.color = Color.magenta;
                break;
        }
    }
        
    public void OnValidate()
    {
        switch (type)
        {
            default:
            case TileType.Grassland:
                _material = Resources.Load<Material>("Tiles/Grassland");
                break;

            case TileType.Lake:
                _material = Resources.Load<Material>("Tiles/Lake");
                break;

            case TileType.Forest:
                _material = Resources.Load<Material>("Tiles/Forest");
                break;

            case TileType.Mountain:
                _material = Resources.Load<Material>("Tiles/Mountain");
                break;
        }

        GetComponent<Renderer>().material = _material;
    }
}