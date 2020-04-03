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

    [MenuItem("Tools/Load Tile Materials")]
    private static void LoadTileMaterials()
    { 
        var tiles = GameObject.FindGameObjectsWithTag("Tile");

        foreach(var t in tiles)
        {
            t.GetComponent<Tile>().LoadMaterial();
        }
    }


    [MenuItem("Tools/Load Unit Materials")]
    private static void LoadUnitMaterials()
    {
        var units = GameObject.FindGameObjectsWithTag("Unit");

        foreach (var unit in units)
            unit.GetComponent<PlayerState>().SetStateParameters();
    }

    [MenuItem("Tools/Load 3D Objects On Tiles")]
    private static void Load3DObjectsOnTiles()
    {
        var tiles = GameObject.FindGameObjectsWithTag("Tile");

        foreach(var tile in tiles)
        {
            var tileObject = new GameObject();

            switch(tile.GetComponent<Tile>().type)
            {
                default:
                case Tile.TileType.Grassland:
                    tileObject = Resources.Load("Grass") as GameObject;
                    break;

                case Tile.TileType.Forest:
                    tileObject = Resources.Load("Forest") as GameObject;
                    break;

                case Tile.TileType.Lake:
                    tileObject = Resources.Load("Water") as GameObject;
                    break;

                case Tile.TileType.Mountain:
                    tileObject = Resources.Load("Mountain") as GameObject;
                    break;
            }
            GameObject.Instantiate(tileObject, new Vector3(tile.transform.position.x, 0.5f, tile.transform.position.z), new Quaternion());
        }
    }

    [MenuItem("Tools/Remove 3D Objects From Tiles")]
    private static void Remove3DObjectsFromTiles()
    {
        var tileObjects = GameObject.FindGameObjectsWithTag("3D Tile Object");

        foreach (var tileObject in tileObjects)
            GameObject.DestroyImmediate(tileObject);
    }
}
