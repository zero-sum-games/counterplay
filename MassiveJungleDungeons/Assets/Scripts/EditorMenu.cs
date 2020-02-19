using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

//==============================================================================

public class EditorMenu
{
    //==========================================================================

    // Assign default blank tile material to all tiles in the current scene
    // [MenuItem("Tools/Assign Tile Material")]
    public static void AssignTileMaterial()
    {
        GameObject[] tiles = GameObject.FindGameObjectsWithTag("Tile");

        foreach(GameObject t in tiles)
        {
            Material material;

            Tile tile = t.GetComponent<Tile>();
            switch(tile.type)
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
            t.GetComponent<Renderer>().material = material;
        }
    }

    //==========================================================================

    // Assign the tile script to all tiles in the current scene
    // [MenuItem("Tools/Assign Tile Script")]
    public static void AssignTileScript()
    {
        GameObject[] tiles = GameObject.FindGameObjectsWithTag("Tile");
        
        // For all objects tagged tile that lack a Tile component, add one
        foreach (GameObject t in tiles)
            if (t.TryGetComponent(typeof(Tile), out Component component) == false)
            {
                t.AddComponent<Tile>();
            }
    }
    
    //==========================================================================

    // Assign both the tile material and/or tile script if tile is missing one/both
    [MenuItem("Tools/Assign Tile State")]
    public static void AssignTileState()
    {
        AssignTileScript();
        AssignTileMaterial();
    }
    

}
