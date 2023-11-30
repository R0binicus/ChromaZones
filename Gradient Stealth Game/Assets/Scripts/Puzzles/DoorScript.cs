using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorScript : MonoBehaviour
{

    [field: SerializeField] private int _assignmentCode = 0;

    private bool _activated = false;

    // Start is called before the first frame update
    void Awake()
    {
        
    }

    private void OnEnable()
    {
        if (_assignmentCode != 0)
        {
            EventManager.EventSubscribe(EventType.ASSIGNMENT_CODE_TRIGGER, AssignmentCodeHandler);
        }
    }

    private void OnDisable()
    {
        EventManager.EventUnsubscribe(EventType.ASSIGNMENT_CODE_TRIGGER, AssignmentCodeHandler);
    }

    private void AssignmentCodeHandler(object data)
    {
        if (data == null)
        {
            Debug.Log("DoorScript AssignmentCodeHandler is null");
        }

        if (_assignmentCode == (int)data)
        {
            this.gameObject.SetActive(false);
        }
    }
}
