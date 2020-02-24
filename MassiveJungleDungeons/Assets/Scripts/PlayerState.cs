using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//==============================================================================
public class PlayerState : UnitState
{
    //==========================================================================

    private PlayerMove _playerMove;
    private PlayerCombat _playerCombat;

    //==========================================================================

    private void CheckKeyboard()
    {
        var nextState = (int) elementalState;
        if(Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
            nextState -= 1;
        else if(Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
            nextState += 1;

        if (nextState < 0)
            nextState = 2;
        else if (nextState > 2)
            nextState = 0;

        elementalState = (ElementalState) nextState;

        _playerMove.SetRange((int) elementalState);
        _playerCombat.SetAttack((int) elementalState);
    }

    //==========================================================================

    private void Start()
    {
        _playerMove = GetComponent<PlayerMove>();
        _playerCombat = GetComponent<PlayerCombat>();
    }

    private void Update()
    {
        CheckKeyboard();
        SetStateParameters();
    }
}
