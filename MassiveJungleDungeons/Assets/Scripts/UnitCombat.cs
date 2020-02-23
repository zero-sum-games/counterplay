using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitCombat : MonoBehaviour
{
    /*  save for later development of combat system
    protected enum AttackState
    {
        Primary    = 0,
        Secondary  = 1,
       
    }*/

    public int atkRange;
    public int atkDamage;
    public int playerDef;
    public int playerHP;

    private List<Tile> _withingRangeTiles = new List<Tile>();
    private GameObject[] _tiles;

    private Tile _currentTile;
    //==========================================================================

    //State attribute parameters
    public void SetAttack(int elementalState)
    {
        switch (elementalState)
        {

            default:
            case 0: // Grass
                atkRange = 2;
                atkDamage = 30;
                playerDef = 20;
                break;

            case 1: // Water
                atkRange = 1;
                atkDamage = 20;
                playerDef = 40;
                break;

            case 2: // Fire
                atkRange = 3;
                atkDamage = 40;
                playerDef = 10;
                break;

        }
    }

    //==========================================================================
    protected void Init()
    {
        _tiles = GameObject.FindGameObjectsWithTag("Tile");
    }

    protected void FindRangeTiles()
    {
        ComputeAdjacencyList();

        var process = new Queue<Tile>();
        _currentTile = GetCurrentTile();
        _currentTile.visited = true;
        process.Enqueue(_currentTile);
        var _tempAtkTiles = new List<Tile>();

        while (process.Count > 0)
        {
            var t = process.Dequeue();

            _withingRangeTiles.Add(t);
            var seenTiles = new List<Tile>();
            if (t != _currentTile)
                t.state = Tile.TileState.Selected;

            for (int i = 0; i < atkRange; i++)
            {
                foreach (Tile T in _withingRangeTiles)
                {
                    foreach (Tile tn in T.adjacencyList)
                    {
                        _tempAtkTiles.Add(tn);
                    }
                }
                _withingRangeTiles = _tempAtkTiles;
                _tempAtkTiles = new List<Tile>();
                if (i < atkRange - 1)
                {
                    seenTiles.AddRange(_withingRangeTiles);
                }
            }
            foreach (Tile item in seenTiles) { _withingRangeTiles.Remove(item); }

            seenTiles = new List<Tile>();

        }

    }


    //==========================================================================
    private void ComputeAdjacencyList()
    {
        foreach (var tile in _tiles)
        {
            var t = tile.GetComponent<Tile>();
            t.FindTarget(this);
        }
    }

    private Tile GetCurrentTile()
    {
        var tile = GetTargetTile(gameObject);
        tile.state = Tile.TileState.Current;

        return tile;
    }

    private static Tile GetTargetTile(GameObject target)
    {
        Tile tile = null;
        if (Physics.Raycast(target.transform.position, Vector3.down, out var hit, 1))
            tile = hit.collider.GetComponent<Tile>();

        return tile;
    }


}
