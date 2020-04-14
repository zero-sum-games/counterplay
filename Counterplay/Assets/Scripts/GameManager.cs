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
            if (!teams[_activeTeamIndex].GetComponent<TeamManager>().CanAdvance()) return;

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
