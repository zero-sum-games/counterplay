using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerState : UnitState
{
    private PlayerMove _playerMove;
    private PlayerCombat _playerCombat;

    public ElementalState GetElementalState()
    {
        return elementalState;
    }

    private void CheckKeyboard()
    {
        if (_playerMove.state != UnitMove.MoveState.Idle || _playerCombat.state != UnitCombat.CombatState.Idle) return;

        var nextState = (int) elementalState;
        if(Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
            nextState -= 1;
        else if(Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
            nextState += 1;

        if (nextState < 0)
            nextState = 2;
        else if (nextState > 2)
            nextState = 0;

        if ((int) elementalState != nextState)
        {
            elementalState = (ElementalState) nextState;

            _playerMove.SetMovementRange((int) elementalState);
            _playerCombat.SetAttackRange((int) elementalState);

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

        if (_playerMove.state == UnitMove.MoveState.Idle && _playerCombat.state == UnitCombat.CombatState.Idle)
            DrawElementalTriangle(false);
        else
            DrawElementalTriangle(true);
    }

    public void OnValidate()
    {
        SetStateParameters();
    }

    private void DrawElementalTriangle(bool isElementalTriangleDeselected)
    {
        // the float (-1.0f) subtraction is taken from the canvas transform's x position
        var currentPosition = transform.position;
        elementalTriangle.position = new Vector3(currentPosition.x + _elementalTriangleXOffset, currentPosition.y + _elementalTriangleYOffset, currentPosition.z);
        elementalTriangle.LookAt(new Vector3(elementalTriangleRotation.transform.position.x, Camera.main.transform.position.y, elementalTriangleRotation.transform.position.z));

        if (isElementalTriangleDeselected) 
        { 
            elementalTriangleDeselected.gameObject.SetActive(true);
            elementalTriangleGrass.gameObject.SetActive(false);
            elementalTriangleWater.gameObject.SetActive(false);
            elementalTriangleFire.gameObject.SetActive(false);
            return; 
        }

        elementalTriangleDeselected.gameObject.SetActive(false);

        switch (elementalState)
        {
            default:
            case ElementalState.Grass:
                elementalTriangleGrass.gameObject.SetActive(true);
                elementalTriangleWater.gameObject.SetActive(false);
                elementalTriangleFire.gameObject.SetActive(false);
                break;

            case ElementalState.Water:
                elementalTriangleGrass.gameObject.SetActive(false);
                elementalTriangleWater.gameObject.SetActive(true);
                elementalTriangleFire.gameObject.SetActive(false);
                break;

            case ElementalState.Fire:
                elementalTriangleGrass.gameObject.SetActive(false);
                elementalTriangleWater.gameObject.SetActive(false);
                elementalTriangleFire.gameObject.SetActive(true);
                break;
        }
    }
}
