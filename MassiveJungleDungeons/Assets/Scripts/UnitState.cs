using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//==============================================================================
public class UnitState : MonoBehaviour
{
    //==========================================================================
    
    public static UnitState Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }
    
    //==========================================================================
    
    public enum ElementalState
    {
        Grass   = 0,
        Water   = 1,
        Fire    = 2
    }
    // Default Elemental State is Grass
    public ElementalState elementalState = ElementalState.Grass;
    
    //==========================================================================
    
    // booleans for each type of terrain to be walkable
        public bool moveToGrassland = false;
        public bool moveToLake = false;
        public bool moveToForest = false;
        public bool moveToMountain = false;
    
    //==========================================================================
    
    // set the parameters of the game object according to current elemental state
    protected void SetStateParameters()
    {
        // Color32 allows for byte values instead of floats from 0.0f - 1.0f
        Color32 color;

        switch (elementalState)
        {
            default:
            case ElementalState.Grass:
                color = new Color32(54, 224, 91, 1);
                
                moveToGrassland = true;
                moveToForest = true;
                moveToLake = false;
                moveToMountain = false;
                break;
                
            case ElementalState.Water:
                color = new Color32(77, 125, 247, 1);
                
                moveToGrassland = true;
                moveToForest = true;
                moveToLake = true;
                moveToMountain = false;
                break;

            case ElementalState.Fire:
                color = new Color32(242, 94, 61, 1);
                
                moveToGrassland = true;
                moveToForest = true;
                moveToLake = false;
                moveToMountain = true;
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
