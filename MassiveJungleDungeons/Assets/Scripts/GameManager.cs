using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject[] teams;

    private int _activeTeamIndex = 0;

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Return))
        {
            // TODO: add reset function that resets states of all team's units when it becomes active again - put it in the team manager
            
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
