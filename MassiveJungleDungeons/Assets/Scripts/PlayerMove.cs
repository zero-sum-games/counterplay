using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//==============================================================================

public class PlayerMove : UnitMove
{
    //==========================================================================

    private void CheckMouse()
    {
        if(Input.GetMouseButtonUp(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            RaycastHit hit;
            if(Physics.Raycast(ray, out hit))
            {
                if(hit.collider.tag == "Tile")
                {
                    Tile t = hit.collider.GetComponent<Tile>();
                    if(t.state == Tile.TileState.SELECTED)
                        MoveToTile(t);
                }
            }
        }
    }

    //==========================================================================

    private void Start()
    {
        Init();
    }

    private void Update()
    {
        switch (state)
        {
            default:
            case MoveState.IDLE:
                if (Input.GetKeyDown(KeyCode.Space))
                    state = MoveState.SELECTING;
                break;

            case MoveState.SELECTING:
                FindSelectableTiles();
                CheckMouse();
                break;

            case MoveState.MOVING:
                Move();
                break;
        }
    }
}
