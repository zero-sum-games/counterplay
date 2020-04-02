using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject[] teams;
    private int _activeTeamIndex = 0;

    void Update()
    {
        // TODO: write handling for making sure that players can only end turn if appropriate (units not moving or attacking, etc.)

        if(Input.GetKeyDown(KeyCode.Return))
        {
            teams[_activeTeamIndex].GetComponent<TeamManager>().Reset();
            
            _activeTeamIndex++;

            if (_activeTeamIndex >= teams.Length)
                _activeTeamIndex = 0; 
        }
    }

    public int GetActiveTeamID()
    {
        return _activeTeamIndex;
    }
}
