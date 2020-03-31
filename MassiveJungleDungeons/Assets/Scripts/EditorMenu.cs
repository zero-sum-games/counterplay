using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;

public static class EditorMenu
{
    [MenuItem("Tools/Assign New Tile Scripts")]
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

    [MenuItem("Tools/Assign Tile Materials by Type")]
    private static void AssignTileMaterialsByType()
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
}
