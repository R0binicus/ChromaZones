using System.Collections;
using System.Collections.Generic;
using Unity.PlasticSCM.Editor.WebApi;
using UnityEngine;

public class ColourRegion : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private ManagerScript gameManager;

    [SerializeField] public float colour;               // Colour from game manager
    [SerializeField] public float localColour = 0f;     // Colour of this region specifically
    private float originalHue;                          // Original colour value when the level started

    void Start()
    {
        // Set values and components
        originalHue = colour;
        gameManager = GameObject.FindGameObjectWithTag("Manager").GetComponent<ManagerScript>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        colour = gameManager.colour;
        ProcessColour();
        SetColour();
    }

    // Processes the gameManager colour
    void ProcessColour()
    {
        // set local colour value
        localColour = colour + originalHue;

        //localColour %= 360f;
        //if local colour value is over 360, change the local colour values back to being under 360 with math
        if (localColour >= 360f)
        {
            // Mathf.Floor rounds down to the nearest int value
            // eg. if local colour = 750 
            // local colour = 750 - ((Mathf.Floor(750/360))*360)
            // local colour = 750 - ((Mathf.Floor(2.08333))*360)
            // local colour = 750 - (2*360)
            // local colour = 750 - 720
            // local colour = 30
            localColour = localColour - ((Mathf.Floor(localColour / 360f)) * 360);
        }
    }

    // Sets the colour of the sprite renderer
    void SetColour()
    {
        // used 0.95 because otherwise it hurts my eyes
        spriteRenderer.color = Color.HSVToRGB(localColour/360f, 0.95f, 0.95f);
    }
}
