using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitState : MonoBehaviour
{ 
    
    public static UnitState Instance { get; private set; }

    public enum ElementalState
    {
        Grass   = 0,
        Water   = 1,
        Fire    = 2
    }
    
    public ElementalState elementalState = ElementalState.Grass;

    protected int _teamID;

    private void Start()
    {
        Instance = this;
        SetStateParameters();

        _teamID = transform.parent.gameObject.GetComponent<TeamManager>().teamID;
    }

    public void SetStateParameters()
    {
        // Color32 allows for byte values instead of floats from 0.0f - 1.0f
        Color32 color;

        switch (elementalState)
        {
            default:
            case ElementalState.Grass:
                color = new Color32(54, 224, 91, 1);
                break;
                
            case ElementalState.Water:
                color = new Color32(77, 125, 247, 1);
                break;

            case ElementalState.Fire:
                color = new Color32(242, 94, 61, 1);
                break;
            
        }

        GetComponent<Renderer>().material.color = color;
    }
}
