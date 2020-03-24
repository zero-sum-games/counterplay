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

    private List<Tile> _withinRangeTiles = new List<Tile>();
    private GameObject[] _tiles;
    public Tile[] markedTiles;
    private GameObject targetMarker;

    private Tile _currentTile;

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

            _withinRangeTiles.Add(t);
            var seenTiles = new List<Tile>();
            if (t != _currentTile)
                t.state = Tile.TileState.Selected;

            for (int i = 0; i < atkRange; i++)
            {
                foreach (Tile T in _withinRangeTiles)
                {
                    foreach (Tile tn in T.adjacencyList)
                    {
                        _tempAtkTiles.Add(tn);
                    }
                }
                _withinRangeTiles = _tempAtkTiles;
                _tempAtkTiles = new List<Tile>();
                if (i < atkRange - 1)
                {
                    seenTiles.AddRange(_withinRangeTiles);
                }
            }
            foreach (Tile item in seenTiles) { _withinRangeTiles.Remove(item); }

            seenTiles = new List<Tile>();

            MarkTargetableTiles();
        }
    }

    protected void MakeAttack(PlayerCombat myTarget)
    {
        if (!myTarget.markedTarget) return;
        {
            // TODO: make more complex later
            if (myTarget.playerHP <= 0)
                myTarget.playerHP -= atkDamage - (myTarget.playerDef);
            else { Destroy(myTarget.gameObject); }
        }
    }

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

    private void MarkTargetableTiles()
    {
        GameObject targetMark = Resources.Load("targetMarker") as GameObject;
        foreach(Tile t in _withinRangeTiles)
        {
            targetMarker = Instantiate(targetMark,  t.gameObject.transform);
            targetMarker.transform.parent = t.gameObject.transform;
            
            
        }
    }

    protected void RemoveTargetTiles()
    {
        if (_currentTile != null) { 
            _currentTile.state = Tile.TileState.Default;
            _currentTile = null;
        }

        foreach(var Target in _withinRangeTiles)
            Target.ResetTargetMarkers();

        _withinRangeTiles.Clear();
    }
}
