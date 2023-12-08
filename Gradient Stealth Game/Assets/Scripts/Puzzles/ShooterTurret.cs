using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShooterTurret : MonoBehaviour
{
    [field: SerializeField] private int _assignmentCode = 0;

    [Header("Turret Stats")]
    [SerializeField] private uint _maxPoolNum;
    [SerializeField] private float _fireRate;
    [SerializeField] private float _fireCD = 0f;
    [SerializeField] private bool _shootDisable = false;
    private List<GameObject> _projectiles;
    
    
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
        _alertData = new AlertData(transform.position, _alertOthersRadius, 1);
        _projectiles = ObjectPooler.CreateObjectPool(_maxPoolNum, _proj, transform);
        
        foreach (GameObject obj in _projectiles)
        {
            BasicProjectile projectile = obj.GetComponent<BasicProjectile>();
            projectile.SetProjectileData(ProjectileData);
            projectile.SetTurrentParent(this);
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
        var obj = ObjectPooler.GetPooledObject(_projectiles);

        if (obj == null)
        {
            Debug.Log("NO OBJECTS IN POOL");
        }

        obj.GetComponent<BasicProjectile>().Go(transform);
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
