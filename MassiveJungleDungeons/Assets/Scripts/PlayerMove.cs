using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : UnitMove
{
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

    private void Start()
    {
        Init();
    }

    private float _buttonStartTime; // when space is pressed
    private float _buttonTimePressed; // how long space was held

    private void Update()
    {
        switch (State)
        {
            default:
            case MoveState.Idle:

                if (Input.GetKeyDown(KeyCode.Space) && !Input.GetKeyUp(KeyCode.Space))
                {
                    _buttonStartTime = Time.time;
                    State = MoveState.Selecting;
                }
            
                else if (Input.GetKeyDown(KeyCode.Space))
                {
                    State = MoveState.Selecting;
                }
                break;
                           
            case MoveState.Selecting:
            
                FindSelectableTiles();
                CheckMouse();
    
                if (!Input.GetKeyDown(KeyCode.Space) && Input.GetKeyUp(KeyCode.Space))
                {
                    _buttonTimePressed = Time.time - _buttonStartTime;
                    
                    if (_buttonTimePressed > 0.3)
                    {
                        RemoveSelectableTiles();
                        State = MoveState.Idle;
                    }
                }
                    
                else if (Input.GetKeyDown(KeyCode.Space))
                {
                    RemoveSelectableTiles();
                    State = MoveState.Idle;
                }
                break;
        
            case MoveState.Moving:
                Move();
                break;
            }
    }
}
