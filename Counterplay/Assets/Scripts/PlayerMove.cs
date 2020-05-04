using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//==============================================================================
public class PlayerMove : UnitMove
{
    //==========================================================================
    private float _buttonStartTime; // when space is pressed
    private float _buttonTimePressed; // how long space was held

    private void Start()
    {
        Init();
    }

    private void Update()
    {
        if (_teamID != GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>().GetActiveTeamID())
        {
            if (_currentTile != null)
                _currentTile.Reset(true, false);

            state = MoveState.Idle;

            return;
        }

        if (this.GetComponent<PlayerCombat>().state != UnitCombat.CombatState.Idle) return;

        switch (state)
        {
            default:
            case MoveState.Idle:
                if(_currentTile == null)
                {
                    _currentTile = GetCurrentTile();
                    _currentTile.SetActiveSelectors(false, false, true);
                }

                if (Input.GetKeyDown(KeyCode.Space) && !Input.GetKeyUp(KeyCode.Space))
                {
                    _buttonStartTime = Time.time;
                    state = MoveState.Selected;
                }

                else if (Input.GetKeyDown(KeyCode.Space))
                {
                    state = MoveState.Selected;
                }
                break;

            case MoveState.Selected:
                FindAndSelectTiles();
                CheckMouse();

                if (!Input.GetKeyDown(KeyCode.Space) && Input.GetKeyUp(KeyCode.Space))
                {
                    _buttonTimePressed = Time.time - _buttonStartTime;

                    if (_buttonTimePressed > 0.3f)
                    {
                        RemoveSelectedTiles();
                        state = MoveState.Idle;
                    }
                }

                else if (Input.GetKeyDown(KeyCode.Space))
                {
                    RemoveSelectedTiles();
                    state = MoveState.Idle;
                }
                break;

            case MoveState.Moving:
                RemoveSelectedTiles();
                Move();
                break;

            case MoveState.Moved:
                _currentTile.SetActiveSelectors(false, false, false);
                _currentTile = GetCurrentTile();
                _currentTile.SetActiveSelectors(false, false, true);
                break;
        }
    }

    //==========================================================================
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
                        _currentTile = hit.collider.GetComponent<Tile>();

                        if (_currentTile.state == Tile.TileState.Selected)
                        {
                            _currentTile.SetActiveSelectors(true, false, false);
                            MoveToTile(_currentTile);
                        }
                        else
                        {
                            RemoveSelectedTiles();
                            state = MoveState.Idle;
                        }
                    }
                }
            }
        }
    }

    public void Reset()
    {
        _currentTile.SetActiveSelectors(false, false, false);
        _currentTile = null;

        _path.Clear();

        _selectedTiles.Clear();
    }
}
