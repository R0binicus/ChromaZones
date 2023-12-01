using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShooterTurret : MonoBehaviour
{
    [Header("Turret Stats")]
    [SerializeField] private uint _maxPoolNum;
    [SerializeField] private float _fireRate;
    [SerializeField] private float _fireCD = 0f;
    private List<GameObject> _projectiles;
    
    private AlertData _alertData;


    [Header("Projectile Stats")]
    [SerializeField] private GameObject _proj;
    [SerializeField] private float _alertOthersRadius;
    [field: SerializeField] public ProjectileData ProjectileData { get; private set; }

    private void Start()
    {
        _alertData = new AlertData(transform.position, _alertOthersRadius);
        _projectiles = ObjectPooler.CreateObjectPool(_maxPoolNum, _proj, transform);
        
        foreach (GameObject obj in _projectiles)
        {
            BasicProjectile projectile = obj.GetComponent<BasicProjectile>();
            projectile.SetProjectileData(ProjectileData);
            projectile.TurretParent = this;
        }
    }

    void Update()
    {
        if (_fireCD <= 0)
        {
            Shoot();
            _fireCD = _fireRate;
        }

        _fireCD -= Time.deltaTime;
    }

    void Shoot()
    {
        var obj = ObjectPooler.GetPooledObject(_projectiles);

        if (obj == null)
        {
            Debug.Log("HRRR NO OBJECTS IN POOL");
        }

        obj.GetComponent<BasicProjectile>().Go(transform);
    }

    public void AlertOthers(Vector3 Centre)
    {
        _alertData.Centre = Centre;
        EventManager.EventTrigger(EventType.AREA_CHASE_TRIGGER, _alertData);
    }
}
