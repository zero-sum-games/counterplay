using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;

//==============================================================================

public static class EditorMenu
{
    //==========================================================================

    // Assign default blank tile material to all tiles in the current scene
    // [MenuItem("Tools/Assign Tile Material")]
    private static void AssignTileMaterial()
    {
        GameObject[] tiles = GameObject.FindGameObjectsWithTag("Tile");

        foreach(GameObject t in tiles)
        {
            Material material;

            Tile tile = t.GetComponent<Tile>();
            switch(tile.type)
            {
                default:
                case Tile.TileType.Grassland:
                    material = Resources.Load<Material>("Tiles/Grassland");
                    break;

                case Tile.TileType.Lake:
                    material = Resources.Load<Material>("Tiles/Lake");
                    break;

                case Tile.TileType.Forest:
                    material = Resources.Load<Material>("Tiles/Forest");
                    break;

                case Tile.TileType.Mountain:
                    material = Resources.Load<Material>("Tiles/Mountain");
                    break;
            }
            t.GetComponent<Renderer>().material = material;
        }
    }

    //==========================================================================

    // Assign the tile script to all tiles in the current scene
    // [MenuItem("Tools/Assign Tile Script")]
    private static void AssignTileScript()
    {
        GameObject[] tiles = GameObject.FindGameObjectsWithTag("Tile");
        
        // Check if object has a Tile component, if not add one
        // Prevents duplicate tile components
        foreach (GameObject t in tiles)
            if (t.TryGetComponent(typeof(Tile), out Component component) == false)
            {
                t.AddComponent<Tile>();
            }
    }
    
    //==========================================================================

    // Runs AssignTileScript and AssignTileMaterial
    [MenuItem("Tools/Assign Tile Type")]
    private static void AssignTileType()
    {
        AssignTileScript();
        AssignTileMaterial();
    }
    

}
