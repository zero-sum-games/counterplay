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

    private void Start()
    {
        Init();
    }

    private void Update()
    {
        if (_teamID != GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>().GetActiveTeamID()) return;
        
        if (_playerMove.state != UnitMove.MoveState.Idle &&
            _playerMove.state != UnitMove.MoveState.Moved)
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
                    state = CombatState.Selected;
                }
                break;

            case CombatState.Selected:
                FindAndSelectTiles();

                if (Input.GetMouseButtonDown(1))
                {
                    RemoveSelectedTiles();
                    state = CombatState.Idle;
                }
                break;

            case CombatState.Attacking:
                break;

            case CombatState.Attacked:
                break;
        }
    }
}
