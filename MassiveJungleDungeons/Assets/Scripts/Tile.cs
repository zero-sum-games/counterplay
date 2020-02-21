using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//==============================================================================

public class Tile : MonoBehaviour
{
    //==========================================================================

    public enum TileState
    {
        Default     = 0,
        Selected    = 1,
        Targeted    = 2,
        Current     = 3
    }

    // indicates the state of a tile using the enum defined above ^^
    public TileState state = TileState.Default;

    public enum TileType
    {
        Grassland   = 0,
        Lake       = 1,
        Forest      = 2,
        Mountain    = 3
    }

    public TileType type = TileType.Grassland;

    Material _material;

    //==========================================================================

    // represents the tiles that are adjacent to a certain tile
    public List<Tile> adjacencyList = new List<Tile>();

    // breadth-first search variables
    public bool visited = false;
    public Tile parent = null;
    public int distance = 0;
    private Renderer _selectableRangeColor;

    //==========================================================================

    // check all four neighbors and add to adjacency list if appropriate
    public void FindNeighbors(UnitMove unit)
    {
        Reset();

        CheckTile(Vector3.forward, unit);
        CheckTile(Vector3.back, unit);
        CheckTile(Vector3.right, unit);
        CheckTile(Vector3.left, unit);
    }

    // reset all bfs variables, adjacency list, and tile state
    public void Reset()
    {
        state = TileState.Default;

        adjacencyList.Clear();
        visited = false;
        parent = null;
        distance = 0;
    }

    //  Need to create a singleton so we can reference the player instance in UnitMove.SetRange(int)
    //     see https://answers.unity.com/questions/1298691/best-way-to-reference-player-class-instance.html
    
        // same booleans as lines 37-41 in UnitMove.cs [only here for testing]
            public bool moveToGrassland = true;
            public bool moveToLake = false;
            public bool moveToForest = true;
            public bool moveToMountain = false;
    
    //==========================================================================
    
    // adds neighbor tile to current tile's adjacency list if there is nothing on top and it is walkable
    private void CheckTile(Vector3 direction, UnitMove unitMove)
    {
        Vector3 halfExtents = new Vector3(0.25f, 0.5f, 0.25f);
        Collider[] colliders = Physics.OverlapBox(transform.position + direction, halfExtents);

        foreach (Collider item in colliders)
        {
            Tile tile = item.GetComponent<Tile>();
            
            // I was thinking we use if statements with these booleans
            // then every state's parameters set each bool as true/false
            // therefore every elementalState is just changing which tiles
            // get included in the adjacency list
            
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
            }
            
        }
        
    }

    //==========================================================================

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
        //changes selectable tiles' color to show if they are walkable/target/current
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
    
    //==========================================================================
    
    // In edit mode - change material whenever Tile Type is changed in the inspector
    public void OnValidate()
    {
        switch (type)
        {
            default:
            case Tile.TileType.Grassland:
                _material = Resources.Load<Material>("Tiles/Grassland");
                break;

            case Tile.TileType.Lake:
                _material = Resources.Load<Material>("Tiles/Lake");
                break;

            case Tile.TileType.Forest:
                _material = Resources.Load<Material>("Tiles/Forest");
                break;

            case Tile.TileType.Mountain:
                _material = Resources.Load<Material>("Tiles/Mountain");
                break;
        }
        GetComponent<Renderer>().material = _material;
    }
}