using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    private Rigidbody2D rb;
    private ManagerScript gameManager;

    //movement
    [SerializeField] private float moveSpeed;
    private Vector2 moveDirection;
    

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        gameManager = GameObject.FindGameObjectWithTag("Manager").GetComponent<ManagerScript>();
    }

    void Update()
    {
        Processinputs();
    }

    private void FixedUpdate()
    {
        if (moveDirection.x != 0 || moveDirection.y != 0)
        {
            rb.velocity = new Vector2(moveDirection.x * moveSpeed, moveDirection.y * moveSpeed);
            
            gameManager.colour += 1f;
        }
        else 
        {
            rb.velocity = new Vector2(0, 0);
        }
    }

    private void Processinputs()
    {
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveY = Input.GetAxisRaw("Vertical");

        moveDirection = new Vector2(moveX, moveY).normalized;
    }
}
