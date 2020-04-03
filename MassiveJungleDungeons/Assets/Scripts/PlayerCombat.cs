using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCombat : UnitCombat
{
    private PlayerMove _playerMove;

    private float _buttonStartTime; // when right-mouse is pressed
    private float _buttonTimePressed; // how long right-mouse was held

    private void Awake()
    {
        _playerMove = this.GetComponent<PlayerMove>();
        previousHealth = health;
    }

    private void Start()
    {
        Init();
    }

    public void Reset()
    {
        if(_targetTile != null)
        {
            _targetTile.SetActiveSelectors(false, false, false);
            _targetTile = null;

            _target = null;
        }

        if(_currentTile != null)
        {
            _currentTile.SetActiveSelectors(false, false, false);
            _currentTile = null;
        }

        _tilesInRange.Clear();
    }

    private void Update()
    {
        DrawHealthBar();

        if (_teamID != GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>().GetActiveTeamID())
        {
            state = CombatState.Idle;
            return;
        }
        
        if (_playerMove.state != UnitMove.MoveState.Idle && _playerMove.state != UnitMove.MoveState.Moved)
        {
            state = CombatState.Idle;
            return;
        }

        switch (state)
        {
            default:
            case CombatState.Idle:
                if (Input.GetMouseButtonDown(1))
                {
                    if (!Input.GetMouseButtonUp(1))
                        _buttonStartTime = Time.time;

                    FindTilesInRange();

                    _target = GetTarget();
                    if(_target != null)
                        if (Physics.Raycast(_target.transform.position, Vector3.down, out var hit, 1))
                            if (hit.collider.tag == "Tile")
                                _targetTile = hit.collider.gameObject.GetComponent<Tile>();

                    state = CombatState.Selected;
                }
                break;

            case CombatState.Selected:
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    RemoveSelectedTiles();

                    if (_target == null)
                        state = CombatState.Idle;
                    else
                    {
                        _targetTile.SetActiveSelectors(false, true, false);
                        state = CombatState.Attacking;
                    }
                }

                if (!Input.GetMouseButtonDown(1) && Input.GetMouseButtonUp(1))
                {
                    _buttonTimePressed = Time.time - _buttonStartTime;
                    if (_buttonTimePressed > 0.3f)
                    {
                        RemoveSelectedTiles();
                        state = CombatState.Idle;
                    }
                }

                else if (Input.GetMouseButtonDown(1))
                {
                    RemoveSelectedTiles();
                    state = CombatState.Idle;
                }
                break;

            case CombatState.Attacking:
                if(Input.GetKeyDown(KeyCode.Space))
                {
                    if (_targetTile != null)
                    {
                        _targetTile.SetActiveSelectors(false, true, false);
                        _targetTile.Reset(false, true);
                    }

                    switch (this.GetComponent<PlayerState>().GetElementalState())
                    {
                        default:
                        case UnitState.ElementalState.Grass:
                            DealDamage(40);
                            break;

                        case UnitState.ElementalState.Water:
                            DealDamage(30);
                            break;

                        case UnitState.ElementalState.Fire:
                            DealDamage(50);
                            break;
                    }
                    state = CombatState.Attacked;
                }
                break;

            case CombatState.Attacked:
                if(_targetTile != null)
                {
                    _targetTile.SetActiveSelectors(false, false, false);
                    _targetTile = null;
                    _target = null;
                }
                break;

            case CombatState.Dead:
                break;
        }
    }

    private void DealDamage(int amount)
    {
        if (_target != null)
        {
            var target = _target.GetComponent<PlayerCombat>();
            target.previousHealth = target.health;
            target.health -= amount;
            if (target.health > 0)
                target.state = CombatState.Dead;
        }
    }

    private void DrawHealthBar()
    {
        if(previousHealth != health)
            previousHealth -= 1;
        healthFill.value = (float) previousHealth / maxHealth;

        var currentPosition = transform.position;
        healthBar.position = new Vector3(currentPosition.x + 0.75f, currentPosition.y + _healthBarYOffset, currentPosition.z);

        healthBar.LookAt(Camera.main.transform);
    }
}
