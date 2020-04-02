using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;

public static class EditorMenu
{
    [MenuItem("Tools/Load New Tile Scripts")]
    private static void AssignNewTileScripts()
    {
        var tiles = GameObject.FindGameObjectsWithTag("Tile");

        foreach (var t in tiles)
            if (t.TryGetComponent(typeof(Tile), out var component) == false)
            {
                var material = t.GetComponent<Renderer>().material;

                t.AddComponent<Tile>();

                t.GetComponent<Tile>().SetMaterial(material);
            }
    }

    [MenuItem("Tools/Load Tile Materials by Type")]
    private static void AssignTileMaterialsByType()
    { 
        var tiles = GameObject.FindGameObjectsWithTag("Tile");

        foreach(var t in tiles)
        {
            t.GetComponent<Tile>().LoadMaterial();
        }
    }
}
