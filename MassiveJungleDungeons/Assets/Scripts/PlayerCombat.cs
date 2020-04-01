using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCombat : UnitCombat
{
    private PlayerMove _playerMove;

    private void Awake()
    {
        _teamID = transform.parent.gameObject.GetComponent<TeamManager>().teamID;
        _playerMove = this.GetComponent<PlayerMove>();
    }

    private void Update()
    {
        if (_teamID != GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>().GetActiveTeamID()) return;

        if (_playerMove.state != UnitMove.MoveState.Idle) return;

        switch (state)
        {
            default:
            case CombatState.Idle:
                if (Input.GetMouseButtonDown(1))
                    state = CombatState.Selecting;
                break;

            case CombatState.Selecting:
                if (Input.GetMouseButtonDown(1))
                    state = CombatState.Idle;
                break;

            case CombatState.Attacking:
                break;

            case CombatState.Attacked:
                break;
        }
    }
}
