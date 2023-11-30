using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicProjectile : MonoBehaviour
{
    [Header("Projectile Variables")]
    public float lifeTime;
    private float _lifeTimer;
    public float speed;

    private Rigidbody2D _rb;

    [field: SerializeField] private float _alertOthersRadius = 3f;

    public void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
    }

    public void OnEnable()
    {
        _lifeTimer = lifeTime;
    }

    public void OnDisable()
    {
        
    }

    public void Update()
    {
        //Destroy projectiles that fly off screen
        _lifeTimer -= Time.deltaTime;
        if (_lifeTimer <= 0)
        {
            gameObject.SetActive(false);
        }
    }

    public void FixedUpdate()
    {
        _rb.velocity = _rb.velocity.normalized * speed; //Continue in current direction.
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
    }

    public void Go(Transform spawnpoint)
    {
        transform.position = spawnpoint.position;
        gameObject.SetActive(true);
        //var projectile = Instantiate(proj, new Vector3(this.transform.position.x, this.transform.position.y, this.transform.position.z), Quaternion.identity);
        _rb.velocity = transform.forward * speed;
    }
}
