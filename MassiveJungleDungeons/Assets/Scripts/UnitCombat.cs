using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitCombat : MonoBehaviour
{
    public enum CombatState
    {
        Idle        = 0,
        Selecting   = 1,
        Attacking   = 2,
        Attacked    = 3
    }

    public CombatState state = CombatState.Idle;

    protected int _teamID;

    private GameObject[] _tiles;
}
