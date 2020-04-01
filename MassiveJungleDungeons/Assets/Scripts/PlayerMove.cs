using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : UnitMove
{
    private float _buttonStartTime; // when space is pressed
    private float _buttonTimePressed; // how long space was held

    public GameObject selector;

    private void CheckMouse()
    {
        if (Input.GetMouseButtonUp(0))
        {
            var camera = Camera.main;
            if(camera != null)
            {
                var ray = camera.ScreenPointToRay(Input.mousePosition);
                if(Physics.Raycast(ray, out var hit))
                {
                    if(hit.collider.CompareTag("Tile"))
                    {
                        var t = hit.collider.GetComponent<Tile>();
                        if (t.state == Tile.TileState.Selected)
                        {
                            var targetPosition = t.gameObject.transform.position;
                            selector.transform.position = new Vector3(targetPosition.x, 0.51f, targetPosition.z);
                            MoveToTile(t);
                        }
                    }
                }
            }
        }
    }

    private void Start()
    {
        Init();
    }

    private void Update()
    {
        if (_teamID == GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>().GetActiveTeamID())
        {
            switch (State)
            {
                default:
                case MoveState.Idle:
                    selector.transform.position = new Vector3(transform.position.x, 0.51f, transform.position.z);

                    if (Input.GetKeyDown(KeyCode.Space) && !Input.GetKeyUp(KeyCode.Space))
                    {
                        _buttonStartTime = Time.time;
                        State = MoveState.Selected;
                    }

                    else if (Input.GetKeyDown(KeyCode.Space))
                    {
                        State = MoveState.Selected;
                    }
                    break;

                case MoveState.Selected:
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

                case MoveState.Moved:
                    break;
            }
        }
        else
        {
            State = MoveState.Idle;
        }
    }
}
