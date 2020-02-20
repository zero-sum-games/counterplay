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
        DEFAULT     = 0,
        SELECTED    = 1,
        TARGETED    = 2,
        CURRENT     = 3
    }

    // indicates the state of a tile using the enum defined above ^^
    public TileState state = TileState.DEFAULT;

    public enum TileType
    {
        GRASS       = 0,
        WATER       = 1,
        FOREST      = 2,
        MOUNTAIN    = 3
    }

    public TileType type = TileType.GRASS;

    Material material;

    //==========================================================================

    // represents the tiles that are adjacent to a certain tile
    public List<Tile> adjacencyList = new List<Tile>();

    // breadth-first search variables
    public bool visited = false;
    public Tile parent = null;
    public int distance = 0;

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
        state = TileState.DEFAULT;

        adjacencyList.Clear();
        visited = false;
        parent = null;
        distance = 0;
    }

    //  Need to create a singleton so we can reference the player instance in UnitMove.SetRange(int)
    //     see https://answers.unity.com/questions/1298691/best-way-to-reference-player-class-instance.html
    
        // same booleans as lines 37-41 in UnitMove.cs [only here for testing]
        /*public bool moveToGrass = true;
        public bool moveToWater = false;
        public bool moveToForest = true;
        public bool moveToMountain = false;*/
    
    //==========================================================================
    
    // adds neighbor tile to current tile's adjacency list if there is nothing on top and it is walkable
    private void CheckTile(Vector3 direction, UnitMove unitMove)
    {
        var halfExtents = new Vector3(0.25f, 0.5f, 0.25f);
        Collider[] colliders = Physics.OverlapBox(transform.position + direction, halfExtents);

        foreach (Collider item in colliders)
        {
            Tile tile = item.GetComponent<Tile>();
            
            // I was thinking we use if statements with these booleans
            // then every state has set parameters of which bools are true
            
            // SET GRASS AS WALKABLE
            //if (moveToGrass)
            //{
                if (tile != null && tile.type == TileType.GRASS /*|| tile.type == TileType.FOREST*/)
                {
                    RaycastHit hit;
                    if (!Physics.Raycast(tile.transform.position, Vector3.up, out hit, 1))
                        adjacencyList.Add(tile);
                }
            //}

            /*
            // SET FOREST AS WALKABLE
            if (moveToForest)
            {
                if (tile != null && tile.type == TileType.FOREST)
                {
                    RaycastHit hit;
                    if (!Physics.Raycast(tile.transform.position, Vector3.up, out hit, 1))
                        adjacencyList.Add(tile);
                }
            }

            // SET WATER AS WALKABLE
            if (moveToWater)
            {
                if (tile != null && tile.type == TileType.WATER)
                {
                    RaycastHit hit;
                    if (!Physics.Raycast(tile.transform.position, Vector3.up, out hit, 1))
                        adjacencyList.Add(tile);
                }
            }

            // SET MOUNTAIN AS WALKABLE
            if (moveToMountain)
            {
                if (tile != null && tile.type == TileType.MOUNTAIN)
                {
                    RaycastHit hit;
                    if (!Physics.Raycast(tile.transform.position, Vector3.up, out hit, 1))
                        adjacencyList.Add(tile);
                }
            }
            */
        }
        
    }

    //==========================================================================

    private void Start()
    {
        switch(type)
        {
            default:
            case TileType.GRASS:
                material = Resources.Load<Material>("Tiles/Grass");
                break;

            case TileType.WATER:
                material = Resources.Load<Material>("Tiles/Water");
                break;

            case TileType.FOREST:
                material = Resources.Load<Material>("Tiles/Forest");
                break;

            case TileType.MOUNTAIN:
                material = Resources.Load<Material>("Tiles/Mountain");
                break;
        }

        GetComponent<Renderer>().material = material;
    }

    private void Update()
    {
        //changes selectable tiles' color to show if they are walkable/target/current
        var selectableRangeColor = GetComponent<Renderer>();
        switch (state)
        {
            default:
            case TileState.DEFAULT:
                selectableRangeColor.material = material;
                break;

            case TileState.SELECTED:
                selectableRangeColor.material.color = Color.red;
                break;

            case TileState.TARGETED:
                selectableRangeColor.material.color = Color.green;
                break;

            case TileState.CURRENT:
                selectableRangeColor.material.color = Color.magenta;
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
            case Tile.TileType.GRASS:
                material = Resources.Load<Material>("Tiles/Grass");
                break;

            case Tile.TileType.WATER:
                material = Resources.Load<Material>("Tiles/Water");
                break;

            case Tile.TileType.FOREST:
                material = Resources.Load<Material>("Tiles/Forest");
                break;

            case Tile.TileType.MOUNTAIN:
                material = Resources.Load<Material>("Tiles/Mountain");
                break;
        }
        GetComponent<Renderer>().material = material;
    }
}