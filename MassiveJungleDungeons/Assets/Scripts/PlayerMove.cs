using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//==============================================================================

public class PlayerMove : UnitMove
{
    //==========================================================================

    // Checking for player click on tile and executing move
    private void CheckMouse()
    {
        if (!Input.GetMouseButtonUp(0)) return;
            if (Camera.main == null) return;
                var ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (!Physics.Raycast(ray, out var hit)) return;
            
                if (!hit.collider.CompareTag("Tile")) return;
                    var t = hit.collider.GetComponent<Tile>();
                    
                    if(t.state == Tile.TileState.Selected)
                        MoveToTile(t);
    }
    //==========================================================================
    
    // Function for viewing selectable tiles in movement range
    private void ViewMovementRange()
    {
        if (!Input.GetKeyDown(KeyCode.Space)) return;
            State = MoveState.Selecting;
            Debug.Log("Displaying Movement Range");
    }
    
    //==========================================================================

    private void Start()
    {
        Init();
    }

    private void Update()
    {
        switch (State) // Movement states not Unit states
        {
            // View movement range
            default:
            case MoveState.Idle:
                ViewMovementRange();
//                if (!Input.GetKeyDown(KeyCode.Space)) return;
//                state = MoveState.Selecting;
                break;
            
            // Choosing what selectable tile to go to
            case MoveState.Selecting:
                FindSelectableTiles();
                CheckMouse();
                break;

            // Choosing destination tile
            case MoveState.Moving:
                Move();
                break;
        }
    }
}
