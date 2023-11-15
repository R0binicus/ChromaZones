using System.Collections;
using System.Collections.Generic;
using UnityEditor.Build;
using UnityEngine;

public class Player : MonoBehaviour
{
    private Rigidbody2D rb;
    private ManagerScript gameManager;
    private Transform transform;
    private Vector2 origin;

    //movement stuff
    [SerializeField] private float moveSpeed;
    private Vector2 moveDirection;

    public float ColourChangeSpeed = 0.1f;

    void Start()
    {
        // Set values and components
        rb = GetComponent<Rigidbody2D>();
        transform = GetComponent<Transform>();
        gameManager = GameObject.FindGameObjectWithTag("Manager").GetComponent<ManagerScript>();
        origin = transform.position;
    }

    void Update()
    {
        if (rb.velocity != Vector2.zero)
        {
            gameManager.colourTime = 0.5f * ColourChangeSpeed;
        }
        else
        {
            gameManager.colourTime = 0f;
        }
        Processinputs();
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

    private void Processinputs()
    {
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveY = Input.GetAxisRaw("Vertical");

        moveDirection = new Vector2(moveX, moveY).normalized;
    }
}
