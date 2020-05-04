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

        if (_playerMove.state != UnitMove.MoveState.Idle && _playerMove.state != UnitMove.MoveState.HasMoved)
        {
            state = CombatState.Idle;
            return;
        }

        switch (state)
        {
            default:
            case CombatState.Idle:
                if (Input.GetKeyDown(KeyCode.LeftShift))
                {
                    if (!Input.GetKeyUp(KeyCode.LeftShift))
                        _buttonStartTime = Time.time;

                    FindTilesInRange();

                    SetUnitUIs(true);

                    state = CombatState.Selected;
                }
                break;

            case CombatState.Selected:
                if (!Input.GetKeyDown(KeyCode.LeftShift) && Input.GetKeyUp(KeyCode.LeftShift))
                {
                    _buttonTimePressed = Time.time - _buttonStartTime;
                    if (_buttonTimePressed > 0.3f)
                    {
                        RemoveSelectedTiles();
                        SetUnitUIs(false);
                        state = CombatState.Idle;
                    }
                }
                else if (Input.GetKeyDown(KeyCode.LeftShift))
                {
                    RemoveSelectedTiles();

                    SetUnitUIs(false);
                }

                CheckMouse();
                break;

            case CombatState.Attacking:
                ManipulateTile();

                if (_target != null)
                {
                    DealDamage(this.GetComponent<PlayerState>().GetElementalState());

                    _target = null;
                    ResetTargetTile();

                    StartCoroutine(SetUnitUIs(false, 0.8f));
                }

                else
                {
                    ResetTargetTile();
                    SetUnitUIs(false);
                }
                
                state = CombatState.HasAttacked;
                break;

            case CombatState.HasAttacked:
                break;

            case CombatState.Dead:
                break;
        }
    }

    //==========================================================================
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

    private void CheckMouse()
    {
        if (Input.GetMouseButtonUp(0))
        {
            var camera = Camera.main;
            if (camera != null)
            {
                var ray = camera.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out var hit))
                {
                    if (hit.collider.CompareTag("Tile"))
                    {
                        _targetTile = hit.collider.GetComponent<Tile>();

                        if (_targetTile.state == Tile.TileState.Selected)
                        {
                            _targetTile.SetActiveSelectors(false, true, false);
                            _target = GetTargetTile();

                            state = CombatState.Attacking;
                        }
                        else
                        {
                            ResetTargetTile();
                            SetUnitUIs(false);

                            state = CombatState.Idle;
                        }

                        RemoveSelectedTiles();
                    }
                    else if(hit.collider.CompareTag("Unit") && hit.collider.gameObject.GetComponent<PlayerCombat>().GetTeamID() != _teamID)
                    {
                        _target = hit.collider.gameObject;

                        if (Physics.Raycast(_target.transform.position, Vector3.down, out var tile, 1f))
                            _targetTile = tile.collider.gameObject.GetComponent<Tile>();
                            
                        if(!_tilesInRange.Contains(_targetTile))
                        {
                            _target = null;

                            ResetTargetTile();
                            SetUnitUIs(false);

                            state = CombatState.Idle;
                        }

                        else 
                        {
                            _targetTile.SetActiveSelectors(false, true, false);
                            state = CombatState.Attacking;
                        }

                        RemoveSelectedTiles();
                    }
                }
            }
        }
    }

    public void ManipulateTile()
    {
        Tile.TileType tileType = _targetTile.type;

        UnitState.ElementalState elementalState = GetComponent<PlayerState>().GetElementalState();
        switch(elementalState)
        {
            default:
            case UnitState.ElementalState.Grass:
                if (tileType == Tile.TileType.Lake)
                {
                    _targetTile.Remove3DObject();
                    _targetTile.type = Tile.TileType.Ice;
                }
                break;

            case UnitState.ElementalState.Water:
                if (tileType == Tile.TileType.Mountain)
                {
                    _targetTile.Remove3DObject();
                    _targetTile.type = Tile.TileType.MtnPass;
                }
                break;
            
            case UnitState.ElementalState.Fire:
                if (tileType == Tile.TileType.Forest)
                {
                    _targetTile.Remove3DObject();
                    _targetTile.type = Tile.TileType.Ash;
                }
                break;
        }

        _targetTile.Load3DObject();
    }

    private GameObject GetTargetTile()
    {
        if (Physics.Raycast(_targetTile.transform.position, Vector3.up, out var hit, 1f))
            if (hit.collider.gameObject.GetComponent<PlayerCombat>().GetTeamID() != _teamID)
                return hit.collider.gameObject;

        return null;
    }

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

                StartCoroutine(DeathRestart(0.5f));
            }
        }
    }

    IEnumerator DeathRestart(float timeDelay)
    {
        yield return new WaitForSeconds(timeDelay);
        FindObjectOfType<MenuManager>().RestartMatch();

    }

    //==========================================================================
    public void Reset()
    {
        _target = null;

        ResetTargetTile();

        if (_currentTile != null)
        {
            _currentTile.SetActiveSelectors(false, false, false);
            _currentTile = null;
        }

        _tilesInRange.Clear();
    }

    private void ResetTargetTile()
    {
        if (_targetTile != null)
        {
            _targetTile.SetActiveSelectors(false, false, false);
            _targetTile = null;
        }
    }
}
