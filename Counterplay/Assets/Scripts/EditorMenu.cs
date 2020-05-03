using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

#if (UNITY_EDITOR)
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
            tile.GetComponent<Tile>().Load3DObject();
    }

    [MenuItem("Tools/Remove 3D Objects From Tiles")]
    private static void Remove3DObjectsFromTiles()
    {
        foreach (var tileObject in GameObject.FindGameObjectsWithTag("3D Tile Object"))
            GameObject.DestroyImmediate(tileObject);
    }
}
#endif