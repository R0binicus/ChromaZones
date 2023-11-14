using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColourRegion : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private ManagerScript gameManager;
    [SerializeField] public float colour;
    [SerializeField] public float tempColour = 0f;
    [SerializeField] private float originalColour;

    // Start is called before the first frame update
    void Start()
    {
        originalColour = colour;
        gameManager = GameObject.FindGameObjectWithTag("Manager").GetComponent<ManagerScript>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        colour = gameManager.colour;
        GetColour();
        SetColour();
        //if (tempColour > 360f)
        //{
        //    tempColour = colour - ((Mathf.Floor(colour/360f))*360);
        //}
        //else if (tempColour < 0)
        //{
        //    tempColour = 360f - originalColour;
        //}
    }

    void GetColour()
    {
        tempColour = colour + originalColour;
        if (tempColour >= 360f)
        {
            tempColour = tempColour - ((Mathf.Floor(tempColour/360f))*360);
        }
        //else if (colour < 0)
        //{
        //    tempColour = 360f - originalColour;
        //}
        //else 
        //{
        //    tempColour = colour + originalColour;
        //}
    }

    void SetColour()
    {
        spriteRenderer.color = Color.HSVToRGB(tempColour/360f, 1f, 1f);
    }
}
