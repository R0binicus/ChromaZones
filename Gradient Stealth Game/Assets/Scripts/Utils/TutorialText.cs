using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TutorialText : MonoBehaviour
{
    private TextMeshProUGUI _text;
    [field: SerializeField] private int _assignmentCode = 0;

    [field: SerializeField] private bool _visibleOnStart = true;

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
    void Start()
    {
        _text = GetComponent<TextMeshProUGUI>();
        if (!_visibleOnStart)
        {
            _text.enabled = false;
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
            _visibleOnStart = !_visibleOnStart;
            _text.enabled = _visibleOnStart;
        }
    }
}
