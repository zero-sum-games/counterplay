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

    public TileType type = TileType.Grassland;

    private Material _material;

    public List<Tile> adjacencyList = new List<Tile>();

    public bool visited = false;
    public Tile parent = null;
    public int distance = 0;
    private Renderer _selectableRangeColor;

    public void FindNeighbors(UnitMove unit)
    {
        Reset();

        CheckTile(Vector3.forward, unit);
        CheckTile(Vector3.back, unit);
        CheckTile(Vector3.right, unit);
        CheckTile(Vector3.left, unit);
    }

    public void Reset()
    {
        state = TileState.Default;

        adjacencyList.Clear();
        visited = false;
        parent = null;
        distance = 0;
    }

    public bool moveToGrassland = true;
    public bool moveToLake = false;
    public bool moveToForest = true;
    public bool moveToMountain = false;

    private void CheckTile(Vector3 direction, UnitMove unitMove)
    {
        var halfExtents = new Vector3(0.25f, 0.5f, 0.25f);
        var colliders = Physics.OverlapBox(transform.position + direction, halfExtents);

        foreach (var item in colliders)
        {
            var tile = item.GetComponent<Tile>();
            
            if (moveToGrassland)
            {
                if (tile != null && tile.type == TileType.Grassland)
                {
                    if (!Physics.Raycast(tile.transform.position, Vector3.up, out _, 1))
                        adjacencyList.Add(tile);
                }
            }
            
            if (moveToForest)
            {
                if (tile != null && tile.type == TileType.Forest)
                {
                    if (!Physics.Raycast(tile.transform.position, Vector3.up, out _, 1))
                        adjacencyList.Add(tile);
                }
            }

            if (moveToLake)
            {
                if (tile != null && tile.type == TileType.Lake)
                {
                    if (!Physics.Raycast(tile.transform.position, Vector3.up, out _, 1))
                        adjacencyList.Add(tile);
                }
            }

            if (moveToMountain)
            {
                if (tile != null && tile.type == TileType.Mountain)
                {
                    if (!Physics.Raycast(tile.transform.position, Vector3.up, out _, 1))
                        adjacencyList.Add(tile);
                }
            }
        }
    }

    private void Start()
    {
        _selectableRangeColor = GetComponent<Renderer>();
        switch(type)
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

    //  COMBAT ADJACENY COMPUTING 

    public void FindTarget(UnitCombat unit)
    {
        Reset();

        CheckAttackTile(Vector3.forward, unit);
        CheckAttackTile(Vector3.back, unit);
        CheckAttackTile(Vector3.right, unit);
        CheckAttackTile(Vector3.left, unit);
    }


    private void CheckAttackTile(Vector3 direction, UnitCombat unitCombat)
    {
        Vector3 halfExtents = new Vector3(0.25f, 0.5f, 0.25f);
        Collider[] colliders = Physics.OverlapBox(transform.position + direction, halfExtents);

        foreach (Collider item in colliders)
        {
            Tile tile = item.GetComponent<Tile>();
            if (tile != null)
            {
                RaycastHit hit;
                if (!Physics.Raycast(tile.transform.position, Vector3.up, out hit, 1))
                    adjacencyList.Add(tile);
            }
            /*

            // Set Grassland as walkable
            if (moveToGrassland)
            {
                if (tile != null && tile.type == TileType.Grassland)
                {
                    RaycastHit hit;
                    if (!Physics.Raycast(tile.transform.position, Vector3.up, out hit, 1))
                        adjacencyList.Add(tile);
                }
            }


            // Set Forest as walkable
            if (moveToForest)
            {
                if (tile != null && tile.type == TileType.Forest)
                {
                    RaycastHit hit;
                    if (!Physics.Raycast(tile.transform.position, Vector3.up, out hit, 1))
                        adjacencyList.Add(tile);
                }
            }

            // Set Lake as walkable
            if (moveToLake)
            {
                if (tile != null && tile.type == TileType.Lake)
                {
                    RaycastHit hit;
                    if (!Physics.Raycast(tile.transform.position, Vector3.up, out hit, 1))
                        adjacencyList.Add(tile);
                }
            }

            // Set Mountain as walkable
            if (moveToMountain)
            {
                if (tile != null && tile.type == TileType.Mountain)
                {
                    RaycastHit hit;
                    if (!Physics.Raycast(tile.transform.position, Vector3.up, out hit, 1))
                        adjacencyList.Add(tile);
                }
            } */
        }
    }

    public void ResetTargetMarkers()
    {
        GameObject[] TargetMarks = GameObject.FindGameObjectsWithTag("TargetMark");
        foreach(var Mark in TargetMarks)
        {
            Destroy(gameObject);
        }
    }
}