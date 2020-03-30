using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCombat : UnitCombat
{
    public bool combatMode = false;
    public bool markedTarget = false;

    PlayerState _playerState;

    Color tempTarget;

    private void Start()
    {
        Init();
        _playerState = gameObject.GetComponent<PlayerState>();
        playerHP = 100;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift) && !combatMode)
        {
            combatMode = true;
            FindRangeTiles();
        }
        else if (combatMode)
        {
            if (Input.GetMouseButtonDown(0))
            {
                RaycastHit hit;
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out hit))
                {
                    if (hit.collider.tag == "Player")
                    {
                        var myTarget = hit.collider.gameObject.GetComponent<PlayerCombat>();
                        MakeAttack(myTarget);
                    }
                }
            } 
        }
        else if (Input.GetKeyDown(KeyCode.LeftShift))
        {
                combatMode = false;
                RemoveTargetTiles();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "TargetMark")
        {
            markedTarget = true;
            tempTarget = other.GetComponent<Renderer>().material.color;
            other.GetComponent<Renderer>().material.color = Color.yellow;
            
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "TargetMark")
        {
            markedTarget = false;
        }
    }
}
