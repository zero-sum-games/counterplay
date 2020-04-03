using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;

public static class EditorMenu
{
    private static GameObject[] _tiles;

    private static void InitTiles() { _tiles = GameObject.FindGameObjectsWithTag("Tile"); }

    [MenuItem("Tools/Load New Tile Scripts")]
    private static void LoadNewTileScripts()
    {
        if (_tiles == null) InitTiles();

        foreach (var t in _tiles)
            if (t.TryGetComponent(typeof(Tile), out _) == false)
            {
                var material = t.GetComponent<Renderer>().material;
                t.AddComponent<Tile>();
                t.GetComponent<Tile>().SetMaterial(material);
            }
    }

    [MenuItem("Tools/Load Tile Materials")]
    private static void LoadTileMaterials()
    {
        if (_tiles == null) InitTiles();

        foreach (var t in _tiles)
            t.GetComponent<Tile>().LoadMaterial();
    }


    [MenuItem("Tools/Load Unit Materials")]
    private static void LoadUnitMaterials()
    {
        foreach (var unit in GameObject.FindGameObjectsWithTag("Unit"))
            unit.GetComponent<PlayerState>().SetStateParameters();
    }

    [MenuItem("Tools/Load 3D Objects On Tiles")]
    private static void Load3DObjectsOnTiles()
    {
        if (_tiles == null) InitTiles();

        foreach (var tile in _tiles)
        {
            GameObject tileObject;

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
            var tileObjectInstance = GameObject.Instantiate(tileObject, new Vector3(tile.transform.position.x, 0.5f, tile.transform.position.z), Quaternion.Euler(0.0f, 0.0f, 0.0f));
            tileObjectInstance.transform.parent = tile.transform;
        }
    }

    [MenuItem("Tools/Remove 3D Objects From Tiles")]
    private static void Remove3DObjectsFromTiles()
    {
        foreach (var tileObject in GameObject.FindGameObjectsWithTag("3D Tile Object"))
            GameObject.DestroyImmediate(tileObject);
    }
}
