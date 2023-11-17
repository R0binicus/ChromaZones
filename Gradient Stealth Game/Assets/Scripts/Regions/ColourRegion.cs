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
                state = 1;
            break;
            case float x when x >= 60f && x < 180f :
                state = 2;
            break;
            case float x when x >= 180f && x < 300f :
                state = 3;
            break;
            case float x when x >= 300f && x < 360f :
                state = 1;
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

    // Set state variable when entering a region
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Set the object's regionState to be the current
        // state of the region. Also increment the region
        // layer by 1.
        if(collision.tag == "RegionDetector")
        {
            if (collision.transform.parent.tag == "Player") 
            {
                var player = collision.transform.parent.GetComponent<Player>();
                player.regionLayer += 1;
                player.regionState = state;
            }
            else if (collision.transform.parent.tag == "Enemy")
            {
                var enemy = collision.transform.parent.GetComponent<Enemy>();
                enemy.regionLayer += 1;
                enemy.regionState = state;
            }
        }
    }

    // Set state variable whenever it moves inside a region
    private void OnTriggerStay2D(Collider2D collision)
    {
        // This should only be useful when the region changes state
        // while the player or enemy is inside a region
        if(collision.tag == "RegionDetector")
        {
            if (collision.transform.parent.tag == "Player") 
            {
                var player = collision.transform.parent.GetComponent<Player>();
                player.regionState = state;
                if (player.regionState == 3)
                {
                    player.ChangeSpriteVisibility(0.5f);
                }
            }
            else if (collision.transform.parent.tag == "Enemy")
            {
                var enemy = collision.transform.parent.GetComponent<Enemy>();
                enemy.regionState = state;
            }
        }
    }

    // Reset state variable if no longer on a region
    private void OnTriggerExit2D(Collider2D collision)
    {
        if(collision.tag == "RegionDetector")
        {
            // What this does is reduce a region layer by 1
            // If region layer is 0, it should be in NO region
            // so it needs to be manually told to reset the object's
            // regionState to zero (no region)
            if (collision.transform.parent.tag == "Player") 
            {
                var player = collision.transform.parent.GetComponent<Player>();
                player.regionLayer -= 1;
                player.ChangeSpriteVisibility(1f);

                if (player.regionLayer <= 0)
                {
                    player.regionLayer = 0;
                    player.regionState = 0;
                }
            }
            else if (collision.transform.parent.tag == "Enemy")
            {
                var enemy = collision.transform.parent.GetComponent<Enemy>();
                enemy.regionLayer -= 1;

                if (enemy.regionLayer <= 0)
                {
                    enemy.regionLayer = 0;
                    enemy.regionState = 0;
                }
            }
        }
    }
}