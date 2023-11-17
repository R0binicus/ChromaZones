using System.Collections;
using System.Collections.Generic;
using UnityEditor.Build;
using UnityEngine;

public class Player : MonoBehaviour
{
    // Components
    private Rigidbody2D rb;
    private ManagerScript gameManager;
    private SpriteRenderer _spriteRenderer;
    private Transform transform;
    private Vector2 origin;

    // Regions
    public int regionState = 0;
    public int regionLayer = 0;

    //movement
    [SerializeField] private float moveSpeed = 3f;
    private Vector2 moveDirection;

    public float ColourChangeSpeed = 0.1f;

    void Start()
    {
        // Set values and components
        rb = GetComponent<Rigidbody2D>();
        transform = GetComponent<Transform>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        gameManager = GameObject.FindGameObjectWithTag("Manager").GetComponent<ManagerScript>();
        origin = transform.position;
    }

    void Update()
    {
        if (rb.velocity != Vector2.zero && regionState != 3)
        {
            gameManager.colourTime = 0.5f * ColourChangeSpeed;
        }
        else
        {
            gameManager.colourTime = 0f;
        }

        ProcessInputs();
    }

    private void FixedUpdate()
    {
        // if player is doing movement inputs, move the player and add to colour time counter
        if (moveDirection.x != 0 || moveDirection.y != 0)
        {
            rb.velocity = new Vector2(moveDirection.x * moveSpeed, moveDirection.y * moveSpeed);
            
            //gameManager.colourTime += 0.5f * ColourChangeSpeed;
        }
        else // set velocity to zero
        {
            rb.velocity = new Vector2(0, 0);
        }

        //// Multiplies current x and y coordinates together for the colour 
        //// Subtracts the origin coords from the equasion, so the starting position should not affect the colour of the regions
        //gameManager.colourCoords = (transform.position.x) * (transform.position.y);
        ////gameManager.colourCoords = Vector3.Distance(transform.position, origin) 
    }

    private void ProcessInputs()
    {
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveY = Input.GetAxisRaw("Vertical");

        moveDirection = new Vector2(moveX, moveY).normalized;
    }

    // Change between visible and 'hiding'
    public void ChangeSpriteVisibility(float val)
    {
        Color tmp = _spriteRenderer.color;
        tmp.a = val;
        _spriteRenderer.color = tmp;
    }
}
