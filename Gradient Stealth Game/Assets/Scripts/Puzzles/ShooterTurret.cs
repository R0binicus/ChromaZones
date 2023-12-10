using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ShooterTurret : MonoBehaviour
{
    [field: SerializeField] private int _assignmentCode = 0;

    [Header("Turret Stats")]
    [SerializeField] private uint _maxPoolNum;
    [SerializeField] private float _fireRate;
    [SerializeField] private float _fireCD = 0f;
    [SerializeField] private bool _shootDisable = false;
    private List<BasicProjectile> _projectiles;
    
    
    private AlertData _alertData;


    [Header("Projectile Stats")]
    [SerializeField] private GameObject _proj;
    [SerializeField] private float _alertOthersRadius;
    [field: SerializeField] public ProjectileData ProjectileData { get; private set; }

    private void OnEnable()
    {
        EventManager.EventSubscribe(EventType.ASSIGNMENT_CODE_TRIGGER, AssignmentCodeHandler);
        EventManager.EventSubscribe(EventType.LOSE, Death);
    }

    private void OnDisable()
    {
        EventManager.EventUnsubscribe(EventType.ASSIGNMENT_CODE_TRIGGER, AssignmentCodeHandler);
        EventManager.EventUnsubscribe(EventType.LOSE, Death);
    }

    private void Start()
    {
        _alertData = new AlertData(transform.position, _alertOthersRadius);
        _projectiles = ObjectPooler.CreateObjectPool<BasicProjectile>(_maxPoolNum, _proj, transform);
        
        foreach (BasicProjectile projectile in _projectiles)
        {
            projectile.SetProjectileData(ProjectileData);
            projectile.TurretParent = this;
        }
    }

    void Update()
    {
        if (!_shootDisable)
        {
            if (_fireCD <= 0)
            {

                    Shoot();

                _fireCD = _fireRate;
            }

            _fireCD -= Time.deltaTime;
        }
    }

    void Shoot()
    {
        var list = _projectiles.Cast<MonoBehaviour>().ToList();
        BasicProjectile projectile = ObjectPooler.GetPooledObject(list) as BasicProjectile;

        if (projectile == null)
        {
            Debug.Log("HRRR NO OBJECTS IN POOL");
        }

        projectile.Go(transform);
    }

    public void AlertOthers(Vector3 centre)
    {
        _alertData.Centre = centre;
        EventManager.EventTrigger(EventType.AREA_CHASE_TRIGGER, _alertData);
    }

    private void AssignmentCodeHandler(object data)
    {
        if (data == null)
        {
            Debug.Log("DoorScript AssignmentCodeHandler is null");
        }
        if (_assignmentCode == (int)data)
        {
            _shootDisable = !_shootDisable;
        } 
    }

    public void Death(object data)
    {
        _shootDisable = true;
    }
}
