using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;

public static class EditorMenu
{
    [MenuItem("Tools/Assign Tile Material")]
    private static void AssignTileMaterial()
    {
        var tiles = GameObject.FindGameObjectsWithTag("Tile");

        foreach(var t in tiles)
        {
            Material material;

            var tile = t.GetComponent<Tile>();
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

    [MenuItem("Tools/Assign Tile Script")]
    private static void AssignTileScript()
    {
        var tiles = GameObject.FindGameObjectsWithTag("Tile");
        
        foreach (var t in tiles)
            if (t.TryGetComponent(typeof(Tile), out var component) == false)
            {
                t.AddComponent<Tile>();
            }
    }

    [MenuItem("Tools/Assign Tile Type")]
    private static void AssignTileType()
    {
        AssignTileScript();
        AssignTileMaterial();
    }
}
