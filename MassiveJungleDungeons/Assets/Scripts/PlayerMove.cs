using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//==============================================================================
public class PlayerMove : UnitMove
{
    //==========================================================================
    void Start()
    {
        Init();
    }

    void Update()
    {
        switch(state)
        {
            default:
            case MoveState.IDLE:
                FindSelectableTiles();
                CheckMouse();
                break;
            case MoveState.MOVING:
                Move();
                break;
        }
    }

    void CheckMouse()
    {
        if(Input.GetMouseButtonUp(0))
        {
            // cast 3D ray from where user clicks on the screen
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            RaycastHit hit;
            if(Physics.Raycast(ray, out hit))
            {
                if(hit.collider.tag == "Tile")
                {
                    Tile t = hit.collider.GetComponent<Tile>();
                    if(t.state == Tile.TileState.SELECTED)
                    {
                        MoveToTile(t);
                    }
                }
            }
        }
    }
}
