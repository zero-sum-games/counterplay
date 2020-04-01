using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCombat : UnitCombat
{
    private PlayerMove _playerMove;

    private void Awake()
    {
        _playerMove = this.GetComponent<PlayerMove>();
    }

    private void Update()
    {
        if (_teamID != GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>().GetActiveTeamID()
            || _playerMove.state != UnitMove.MoveState.Idle)
        {
            state = CombatState.Idle;
            return;
        }

        switch (state)
        {
            default:
            case CombatState.Idle:
                if (Input.GetMouseButtonDown(1))
                {
                    InitializeAttack();
                }
                break;

            case CombatState.Attacking:
                Debug.Log("Attacking " + _target.ToString());
                state = CombatState.Attacked;
                break;

            case CombatState.Attacked:
                break;
        }
    }

    private void InitializeAttack()
    {
        Debug.Log("Initialized an attack.");

        Tile currentTile = null;

        if (Physics.Raycast(this.transform.position, Vector3.down, out var hit, 1))
        {
            currentTile = hit.collider.GetComponent<Tile>();
            currentTile.visited = true;
        }

        var process = new Queue<Tile>();
        process.Enqueue(currentTile);

        while(process.Count > 0)
        {
            var t = process.Dequeue();

            if (t.distance >= attackRange)
                continue;

            if(t != currentTile && Physics.Raycast(t.transform.position, Vector3.up, out var col, 1))
                if (col.collider.tag == "Unit")
                {
                    _target = col.transform.gameObject;
                    state = CombatState.Attacking;
                    return;
                }

            foreach (var tile in t.adjAttackList)
            {
                if (tile.visited)
                    continue;

                tile.parent = t;
                tile.visited = true;
                tile.distance = t.distance + 1;

                process.Enqueue(tile);
            }
        }
    }
}
