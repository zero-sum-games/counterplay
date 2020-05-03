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
    private void Awake()
    {
        _playerMove = GetComponent<PlayerMove>();
        _playerCombat = GetComponent<PlayerCombat>();
    }

    private void OnValidate()
    {
        SetStateParameters();
    }

    private void Update()
    {
        DrawElementalTriangle();

        if (_teamID != GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>().GetActiveTeamID())
        {
            if (elementalTriangle.gameObject.activeInHierarchy && !_displayForCombatSelection)
                elementalTriangle.gameObject.SetActive(false);

            return;
        }

        CheckKeyboard();
    }

    //==========================================================================
    private void DrawElementalTriangle()
    {
        if (!elementalTriangle.gameObject.activeInHierarchy)
            elementalTriangle.gameObject.SetActive(true);

        // the float (-1.0f) subtraction is taken from the canvas transform's x position
        var currentPosition = transform.position;
        elementalTriangle.position = new Vector3(currentPosition.x + _elementalTriangleXOffset, currentPosition.y + _elementalTriangleYOffset, currentPosition.z);
        elementalTriangle.LookAt(new Vector3(elementalTriangleRotation.transform.position.x, Camera.main.transform.position.y, elementalTriangleRotation.transform.position.z));

        if (!(_playerMove.state == UnitMove.MoveState.Idle && _playerCombat.state == UnitCombat.CombatState.Idle))
        {
            elementalTriangleDeselected.gameObject.SetActive(true);
            elementalTriangleGrass.gameObject.SetActive(false);
            elementalTriangleWater.gameObject.SetActive(false);
            elementalTriangleFire.gameObject.SetActive(false);
            return;
        }

        elementalTriangleDeselected.gameObject.SetActive(false);

        switch (_elementalState)
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
            if (!IsValidStateChange((ElementalState) nextState)) return;

            _elementalState = (ElementalState) nextState;

            SetStateParameters();
        }
    }

    private bool IsValidStateChange(ElementalState nextState)
    {
        Tile tile = GetComponent<PlayerMove>().GetCurrentTile();
        Tile.TileType tileType = tile.GetComponent<Tile>().type;

        switch(tileType)
        {
            default:
            case Tile.TileType.Grassland:
            case Tile.TileType.Forest:
                return true;

            case Tile.TileType.Lake:
                return nextState != ElementalState.Fire;

            case Tile.TileType.Mountain:
                return nextState == ElementalState.Fire;
        }
    }

    //==========================================================================
    public ElementalState GetElementalState()
    {
        return _elementalState;
    }
}
