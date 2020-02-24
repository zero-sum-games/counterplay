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

    private void Start()
    {
        Init();
    }

    private float _buttonStartTime; // when space is pressed
    private float _buttonTimePressed; // how long space was held
    private void Update()
    {
        switch (State) // Movement states not Unit states
        {
            
        // Idle
        default:
        case MoveState.Idle:

            // If key is held start the timer and change MoveState to Selecting
            if (Input.GetKeyDown(KeyCode.Space) && !Input.GetKeyUp(KeyCode.Space))
            {
                _buttonStartTime = Time.time;
                State = MoveState.Selecting;
            }
            
            // otherwise if the key is pressed (not held) change MoveState to Selecting (no timer)
            else if (Input.GetKeyDown(KeyCode.Space))
            {
                State = MoveState.Selecting;
            }
            break;
                    

        // Viewing Movement Range
        case MoveState.Selecting:
            
            //Display selectable tiles in range and check the mouse
            FindSelectableTiles();
            CheckMouse();
            
            
//              ==================================================================================
//              ------------------ If you want to return to Idle from Selecting ------------------
//              ==================================================================================
    
                // when the held key is released stop the timer
                if (!Input.GetKeyDown(KeyCode.Space) && Input.GetKeyUp(KeyCode.Space))
                {
                    // key is released: measure the time
                    _buttonTimePressed = Time.time - _buttonStartTime;
                    
                    // if the timer is greater than 0.3 seconds return to MoveState.Idle
                    if (_buttonTimePressed > 0.3)
                    {
                        RemoveSelectableTiles();
                        State = MoveState.Idle;
                    }
                }
                    
                // otherwise if key is pressed but is released go back to Idle
                else if (Input.GetKeyDown(KeyCode.Space))
                {
                    RemoveSelectableTiles();
                    State = MoveState.Idle;
                }
            break;

        // Move to destination tile
        case MoveState.Moving:
            Move();
            break;
        }
    }
}
