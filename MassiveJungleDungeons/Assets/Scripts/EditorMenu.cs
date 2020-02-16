using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

//==============================================================================

public class EditorMenu
{
    //==========================================================================

    // Assign default blank tile material to all tiles in the current scene
    [MenuItem("Tools/Assign Tile Material")]
    public static void AssignTileMaterial()
    {
        var material = Resources.Load<Material>("Tile");

        GameObject[] tiles = GameObject.FindGameObjectsWithTag("Tile");

        foreach(GameObject t in tiles)
            t.GetComponent<Renderer>().material = material;
    }

    //==========================================================================

    // Assign the tile script to all tiles in the current scene
    [MenuItem("Tools/Assign Tile Script")]
    public static void AssignTileScript()
    {
        GameObject[] tiles = GameObject.FindGameObjectsWithTag("Tile");

        foreach (GameObject t in tiles)
            t.AddComponent<Tile>();
    }
}
