using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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

    public Transform elementalTriangle;
    public Transform elementalTriangleRotation;
    public Image elementalTriangleFire;
    public Image elementalTriangleGrass;
    public Image elementalTriangleWater;
    protected float _elementalTriangleXOffset = 0.0f;
    protected float _elementalTriangleYOffset = 2.4f;

    private void Start()
    {
        Instance = this;
        SetStateParameters();

        _teamID = transform.parent.gameObject.GetComponent<TeamManager>().teamID;
    }

    public void SetStateParameters()
    {
        var renderer = GetComponent<Renderer>();
        if (renderer == null) return;

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

        if (renderer.sharedMaterial != null)
            renderer.sharedMaterial.color = color;
    }
}
