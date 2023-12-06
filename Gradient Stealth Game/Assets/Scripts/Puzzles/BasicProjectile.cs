using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicProjectile : MonoBehaviour
{
    [Header("Projectile Variables")]
    private float _lifeTime;
    private float _timer;
    private float _speed;

    private ShooterTurret _turretParent;

    private Rigidbody2D _rb;

    public void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
    }

    public void OnEnable()
    {
        _timer = _lifeTime;
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

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            if (collision.gameObject.GetComponent<Player>().RegionState != 3)
            {
                _turretParent.AlertOthers(transform.position);
                gameObject.SetActive(false);
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
        _rb.velocity = transform.up * _speed;
    }

    public void SetProjectileData(ProjectileData projectileData)
    {
        _lifeTime = projectileData.LifeTime;
        _speed = projectileData.Speed;
    }

    public void SetTurrentParent(ShooterTurret parent)
    {
        _turretParent = parent;
    }
}
