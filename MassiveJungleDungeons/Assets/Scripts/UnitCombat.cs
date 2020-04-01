using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitCombat : MonoBehaviour
{
    public enum CombatState
    {
        Idle        = 0,
        Attacking   = 1,
        Attacked    = 2
    }

    public CombatState state = CombatState.Idle;

    protected int _teamID;

    public int attackRange;

    protected GameObject _target;

    private void Awake()
    {
        _teamID = transform.parent.gameObject.GetComponent<TeamManager>().teamID;
    }

    private void Start()
    {
        SetAttackRange((int)this.GetComponent<PlayerState>().GetElementalState());
    }

    public void SetAttackRange(int elementalState)
    {
        switch (elementalState)
        {
            // Grass
            default:
            case 0:
                attackRange = 2;
                break;

            // Water
            case 1:
                attackRange = 3;
                break;

            // Fire
            case 2:
                attackRange = 4;
                break;

        }
    }
}
