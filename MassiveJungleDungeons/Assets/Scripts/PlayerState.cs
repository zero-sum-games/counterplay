using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerState : UnitState
{
    private PlayerMove _playerMove;
    private PlayerCombat _playerCombat;

    public ElementalState GetElementalState()
    {
        return _elementalState;
    }

    private void CheckKeyboard()
    {
        if (_playerMove.state != UnitMove.MoveState.Idle || _playerCombat.state != UnitCombat.CombatState.Idle) return;

        var nextState = (int) _elementalState;
        if(Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
            nextState -= 1;
        else if(Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
            nextState += 1;

        if (nextState < 0)
            nextState = 2;
        else if (nextState > 2)
            nextState = 0;

        if ((int) _elementalState != nextState)
        {
            _elementalState = (ElementalState) nextState;
            _playerMove.SetRange((int) _elementalState);
            SetStateParameters();
        }
    }

    private void Awake()
    {
        _playerMove = GetComponent<PlayerMove>();
        _playerCombat = GetComponent<PlayerCombat>();
    }

    private void Update()
    {
        if (_teamID == GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>().GetActiveTeamID())
            CheckKeyboard();
    }
}
