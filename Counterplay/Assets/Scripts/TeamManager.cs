using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//==============================================================================
public class TeamManager : MonoBehaviour
{
    //==========================================================================
    public int teamID;

    // TODO: add unit manager for individual teams
    public GameObject[] units;

    //==========================================================================
    public bool CanAdvance()
    {
        foreach(var unit in units)
        {
            var playerMove = unit.GetComponent<PlayerMove>();
            if (playerMove.state != UnitMove.MoveState.Idle && playerMove.state != UnitMove.MoveState.Moved) return false;

            var playerCombat = unit.GetComponent<PlayerCombat>();
            if (playerCombat.state != UnitCombat.CombatState.Idle && playerCombat.state != UnitCombat.CombatState.Attacked) return false;
        }

        return true;
    }

    //==========================================================================
    public void Reset()
    {
        foreach(var unit in units)
        {
            unit.GetComponent<PlayerMove>().Reset();
            unit.GetComponent<PlayerCombat>().Reset();
        }
    }
}
