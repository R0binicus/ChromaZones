using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShooterTurret : MonoBehaviour
{
    //References
    private Rigidbody rb;

    [Header("Turret Stats")]
    public GameObject proj;
    [SerializeField] private float _projectileLifeTime;

    [SerializeField] private uint _maxPoolNum;
    public float projectileSpeed;

    public float fireRate;
    public float fireCD;

    private List<GameObject> _projectiles;

    private void Start()
    {
        //rb = GetComponent<Rigidbody>();

        _projectiles = ObjectPooler.CreateObjectPool(_maxPoolNum, proj, this.transform);
    }

    // Update is called once per frame
    void Update()
    {
        if (fireCD <= 0)
        {
            Shoot();
            fireCD = fireRate;
        }

        fireCD -= Time.deltaTime;
    }

    void Shoot()
    {
        //var projectile = Instantiate(proj, new Vector3(this.transform.position.x, this.transform.position.y, this.transform.position.z), Quaternion.identity);
        //projectile.GetComponent<Rigidbody2D>().velocity = this.transform.forward.normalized * projectileSpeed;

        //projectile.lifeTime = _projectileLifeTime;
        //projectile.speed = projectileSpeed;

        var obj = ObjectPooler.GetPooledObject(_projectiles);

        if (obj == null)
        {
            Debug.Log("HRRR NO OBJECTS IN POOL");
        }

        //obj.SetActive(true);
        obj.GetComponent<BasicProjectile>().Go(this.transform);
    }
}
