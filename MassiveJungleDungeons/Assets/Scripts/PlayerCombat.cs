using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCombat : UnitCombat
{
    private PlayerMove _playerMove;

    public GameObject selector;

    private float _buttonStartTime; // when right-mouse is pressed
    private float _buttonTimePressed; // how long right-mouse was held

    private void Awake()
    {
        _playerMove = this.GetComponent<PlayerMove>();
    }

    private void Start()
    {
        Init();
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
                        selector.transform.position = new Vector3(_target.transform.position.x, 0.51f, _target.transform.position.z);

                    state = CombatState.Selected;
                }
                break;

            case CombatState.Selected:
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    RemoveSelectedTiles();

                    selector.SetActive(true);

                    if (_target == null)
                        state = CombatState.Idle;
                    else
                        state = CombatState.Attacking;
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
                selector.SetActive(false);
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
            target.health -= amount;
        }
    }

    private void DrawHealthBar()
    {
        healthFill.value = (float) health / maxHealth;

        var currentPosition = transform.position;
        healthBar.position = new Vector3(currentPosition.x, currentPosition.y + _healthBarYOffset, currentPosition.z);

        healthBar.LookAt(Camera.main.transform);
    }
}
