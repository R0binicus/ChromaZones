using System.Collections;
using System.Collections.Generic;
using Unity.PlasticSCM.Editor.WebApi;
using UnityEngine;

public class ColourRegion : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private ManagerScript gameManager;

    [SerializeField] private float transitionMultiplier = 2f;

    private float colourDiff;                           // Colour from game manager
    [SerializeField] private float localColour;         // Colour of this region specifically
    private float originalHue;                          // Original colour value when the level started

    public int state;

    void Start()
    {
        // Set values and components
        originalHue = localColour;
        gameManager = GameObject.FindGameObjectWithTag("Manager").GetComponent<ManagerScript>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        colourDiff = gameManager.colour;
        ProcessColour();
        SetColour();
    }

    // Processes the gameManager colour
    void ProcessColour()
    {
        // set new local colour value
        TransitionZones();

        //if local colour value is over 360, change the local colour values back to being under 360 with math
        if (localColour >= 360f)
        {
            //localColour = localColour - ((Mathf.Floor(localColour / 360f)) * 360);
            localColour = 0f;
        }

        SetStates();
    }

    // Sets the colour of the sprite renderer
    void SetColour()
    {
        // used 0.95 because otherwise it hurts my eyes
        spriteRenderer.color = Color.HSVToRGB(localColour/360f, 0.95f, 0.95f);
    }

    void SetStates()
    {
        switch(localColour) 
        {
            case float x when x < 60f:
                state = 0;
            break;
            case float x when x >= 60f && x < 180f :
                state = 1;
            break;
            case float x when x >= 180f && x < 300f :
                state = 2;
            break;
            case float x when x >= 300f && x < 360f :
                state = 0;
            break;
            default:
                state = 0;
            break;
        }
    }

    // If local colour value is in a 'transition zone' change the colour witha multiplier,
    // otherwise simply add it to the difference value
    void TransitionZones()
    {
        switch(localColour) 
        {
            case float x when x >= 50f && x < 70f :
                localColour = localColour + (colourDiff * transitionMultiplier);
            break;
            case float x when x >= 170f && x < 190f :
                localColour = localColour + (colourDiff * transitionMultiplier);
            break;
            case float x when x >= 290f && x < 310f :
                localColour = localColour + (colourDiff * transitionMultiplier);
            break;
            default:
                localColour = localColour + colourDiff;
            break;
        }
    }

    private void OnTriggerEnter(Collider collision)
    {
        if(collision.tag == "Player")
        {
            //Debug.Log("triggered check");
            //var p = collision.GetComponent<Player>();
//
            //if (!alreadyHealed)
            //{
            //    checkPoint.Play();
            //    p.currentCheckpoint = new Vector3(transform.position.x, 1.14f, transform.position.z);
            //    p.currentHealth = p.currentHealth + healAmount;
            //    if (p.currentHealth > p.maxHealth) p.currentHealth = p.maxHealth;
            //    p.healthBar.SetHealth(p.currentHealth);
            //    alreadyHealed = true;
            //}

        }
    }
}