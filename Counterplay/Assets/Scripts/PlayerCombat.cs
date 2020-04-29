using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//==============================================================================
public class PlayerCombat : UnitCombat
{
    //==========================================================================
    private PlayerMove _playerMove;

    private float _buttonStartTime; // when right-mouse is pressed
    private float _buttonTimePressed; // how long right-mouse was held

    //==========================================================================
    private void Awake()
    {
        _playerMove = this.GetComponent<PlayerMove>();
        previousHealth = health;
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
            if (healthBar.gameObject.activeInHierarchy && !_displayForCombatSelection)
                healthBar.gameObject.SetActive(false);
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
                    if (_target != null)
                        if (Physics.Raycast(_target.transform.position, Vector3.down, out var hit, 1))
                            if (hit.collider.tag == "Tile")
                                _targetTile = hit.collider.gameObject.GetComponent<Tile>();

                    SetUnitUIs(true);

                    state = CombatState.Selected;
                }
                break;

            case CombatState.Selected:
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    RemoveSelectedTiles();

                    if (_target == null)
                    {
                        SetUnitUIs(false);
                        state = CombatState.Idle;
                    }
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
                        SetUnitUIs(false);
                        state = CombatState.Idle;
                    }
                }

                else if (Input.GetMouseButtonDown(1))
                {
                    RemoveSelectedTiles();
                    SetUnitUIs(false);
                    state = CombatState.Idle;
                }
                break;

            case CombatState.Attacking:
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    if (_targetTile != null)
                    {
                        _targetTile.SetActiveSelectors(false, true, false);
                        _targetTile.Reset(false, true);
                    }

                    DealDamage(this.GetComponent<PlayerState>().GetElementalState());
                    state = CombatState.Attacked;
                }
                break;

            case CombatState.Attacked:
                if (_targetTile != null)
                {
                    _targetTile.SetActiveSelectors(false, false, false);
                    _targetTile = null;
                    _target = null;

                    StartCoroutine(SetUnitUIs(false, 1.5f));
                }
                break;

            case CombatState.Dead:
                break;
        }
    }

    //==========================================================================

    private void DrawHealthBar()
    {
        if (previousHealth != health)
            previousHealth -= 1;

        healthFill.value = (float) previousHealth / maxHealth;

        if (!healthBar.gameObject.activeInHierarchy)
            healthBar.gameObject.SetActive(true);

        var currentPosition = transform.position;
        healthBar.position = new Vector3(currentPosition.x + _healthBarXOffset, currentPosition.y + _healthBarYOffset, currentPosition.z);
        healthBar.LookAt(new Vector3(healthBarRotation.transform.position.x, Camera.main.transform.position.y, healthBarRotation.transform.position.z));
    }

    private void SetUnitUIs(bool shouldBeActive)
    {
        foreach (var unit in GameObject.FindGameObjectsWithTag("Unit"))
        {
            var unitCombat = unit.GetComponent<PlayerCombat>();

            if (unitCombat.GetTeamID() == GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>().GetActiveTeamID()) continue;

            unitCombat.SetDisplayForCombatSelection(shouldBeActive);
            unitCombat.healthBar.gameObject.SetActive(shouldBeActive);

            var unitState = unit.GetComponent<PlayerState>();
            unitState.SetDisplayForCombatSelection(shouldBeActive);
            unitState.elementalTriangle.gameObject.SetActive(shouldBeActive);
        }
    }

    IEnumerator SetUnitUIs(bool shouldBeActive, float timeDelay)
    {
        yield return new WaitForSeconds(timeDelay);
        SetUnitUIs(shouldBeActive);
    }

    private void DealDamage(UnitState.ElementalState playerState)
    {
        if (_target != null)
        {
            var target = _target.GetComponent<PlayerCombat>();
            target.previousHealth = target.health;

            var targetState = _target.GetComponent<PlayerState>().GetElementalState();

            int strongDamageMultipliter = Random.value >= 0.5f ? 6 : 5;
            int normalDamageMultiplier = Random.value >= 0.5f ? 4 : 3;
            int weakDamageMultiplier = Random.value >= 0.5f ? 2 : 1;

            int damage = 1;
            switch (playerState)
            {
                default:
                case UnitState.ElementalState.Grass:
                    if (targetState == UnitState.ElementalState.Grass)
                        damage *= normalDamageMultiplier;
                    else if (targetState == UnitState.ElementalState.Water)
                        damage *= strongDamageMultipliter;
                    else
                        damage *= weakDamageMultiplier;
                    break;

                case UnitState.ElementalState.Water:
                    if (targetState == UnitState.ElementalState.Water)
                        damage *= normalDamageMultiplier;
                    else if (targetState == UnitState.ElementalState.Fire)
                        damage *= strongDamageMultipliter;
                    else
                        damage *= weakDamageMultiplier;
                    break;

                case UnitState.ElementalState.Fire:
                    if (targetState == UnitState.ElementalState.Fire)
                        damage *= normalDamageMultiplier;
                    else if (targetState == UnitState.ElementalState.Grass)
                        damage *= strongDamageMultipliter;
                    else
                        damage *= weakDamageMultiplier;
                    break;
            }

            target.health -= damage;

            if (target.health <= 0)
            {
                target.state = CombatState.Dead;

                if (deathText != null && !deathText.gameObject.activeInHierarchy)
                    deathText.gameObject.SetActive(true);
            }
        }
    }

    //==========================================================================
    public void Reset()
    {
        if (_targetTile != null)
        {
            _targetTile.SetActiveSelectors(false, false, false);
            _targetTile = null;

            _target = null;
        }

        if (_currentTile != null)
        {
            _currentTile.SetActiveSelectors(false, false, false);
            _currentTile = null;
        }

        _tilesInRange.Clear();
    }
}
