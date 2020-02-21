using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//==============================================================================
public class UnitState : MonoBehaviour
{
    //==========================================================================

    public enum ElementalState
    {
        Fire    = 0,
        Water   = 1,
        Grass   = 2
    }

    public ElementalState elementalState = ElementalState.Fire;
    
    //==========================================================================

    // set the COLOR of the game object according to current elemental state
    protected void SetStateParameters()
    {
        // Color32 allows for byte values instead of floats from 0.0f - 1.0f
        Color32 color;

        switch (elementalState)
        {
            default:
            case ElementalState.Fire:
                color = new Color32(242, 94, 61, 1);
                break;

            case ElementalState.Water:
                color = new Color32(77, 125, 247, 1);
                break;

            case ElementalState.Grass:
                color = new Color32(54, 224, 91, 1);
                break;
        }

        GetComponent<Renderer>().material.color = color;
    }

    //==========================================================================

    private void Start()
    {
        SetStateParameters();
    }
}
