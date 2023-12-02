using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{

    [field: SerializeField] private int _assignmentCode = 0;
    [field: SerializeField] private bool _disableOnStart = false;

    // Start is called before the first frame update
    void Awake()
    {
        
    }

    private void OnEnable()
    {
        
    }

    private void OnDisable()
    {
        //EventManager.EventUnsubscribe(EventType.ASSIGNMENT_CODE_TRIGGER, AssignmentCodeHandler);
    }

    void Start()
    {
        if (_assignmentCode != 0)
        {
            EventManager.EventSubscribe(EventType.ASSIGNMENT_CODE_TRIGGER, AssignmentCodeHandler);
        }

        if (_disableOnStart)
        {
            _disableOnStart = false;
            gameObject.SetActive(false);
        }
    }

    private void AssignmentCodeHandler(object data)
    {
        if (data == null)
        {
            Debug.Log("DoorScript AssignmentCodeHandler is null");
        }
        if (_assignmentCode == (int)data)
        {
            if (gameObject.activeInHierarchy == false)
            {
                gameObject.SetActive(true);
            }
            else
            {
                EventManager.EventUnsubscribe(EventType.ASSIGNMENT_CODE_TRIGGER, AssignmentCodeHandler);
                gameObject.SetActive(false);
            }
        }
    }

    private void OnDestroy()
    {
        EventManager.EventUnsubscribe(EventType.ASSIGNMENT_CODE_TRIGGER, AssignmentCodeHandler);
    }
}
