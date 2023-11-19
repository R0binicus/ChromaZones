using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManagerScript : MonoBehaviour
{
    public float colour;        // Final colour value for other things to access
    //public float colourCoords;    // Colour value taken from the current coords of the player
    public float colourTime;    // Colour value taken from amount of time spent while moving
    [SerializeField] private NavMeshPlus.Components.NavMeshSurface surfaceSingle;

    void Start()
    {
        surfaceSingle.BuildNavMesh();
    }

    void Update()
    {
        // Combine the colour values from movement coords and time taken to move
        colour = colourTime;//colourCoords + colourTime;
    }
}
