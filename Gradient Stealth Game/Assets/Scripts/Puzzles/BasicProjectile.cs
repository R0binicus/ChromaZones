using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicProjectile : MonoBehaviour
{
    [Header("Projectile Variables")]
    public float LifeTime;
    private float _timer;
    public float Speed;
    public float AlertOthersRadius = 3f;

    private Rigidbody2D _rb;

    public void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
    }

    public void OnEnable()
    {
        _timer = LifeTime;
    }

    public void OnDisable()
    {
        
    }

    public void Update()
    {
        //Destroy projectiles that fly off screen
        _timer -= Time.deltaTime;
        if (_timer <= 0)
        {
            gameObject.SetActive(false);
        }
    }

    public void FixedUpdate()
    {
        _rb.velocity = _rb.velocity.normalized * Speed; //Continue in current direction.
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            if (collision.gameObject.GetComponent<Player>().RegionState != 3)
            {
                gameObject.SetActive(false);
                EventManager.EventTrigger(EventType.AREA_CHASE_TRIGGER, transform.position);
            }
        }
        else if (collision.tag == "Obstacle")
        {
            gameObject.SetActive(false);
        }
    }

    public void Go(Transform spawnpoint)
    {
        transform.position = spawnpoint.position;
        gameObject.SetActive(true);
        _rb.velocity = transform.forward * Speed;
    }
}
