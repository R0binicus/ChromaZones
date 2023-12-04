using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{

    [field: SerializeField] private int _assignmentCode = 0;
    [field: SerializeField] private bool _closed = false;

    // Components
    private SpriteRenderer _spriteRenderer;
    private BoxCollider2D _box;

    // Start is called before the first frame update
    void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _box = GetComponentInChildren<BoxCollider2D>();
        if (_closed)
        {
            _spriteRenderer.enabled = false;
            _box.enabled = false;
            gameObject.tag = "Untagged";
            gameObject.layer = 0;
        }
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
            if (_closed == true)
            {
                _spriteRenderer.enabled = true;
                _box.enabled = true;
                gameObject.tag = "Obstacle";
                //interactableObject.interactionLayers = InteractionLayerMask.GetMask("Default");
                //gameObject.LayerMask.NameToLayer
                gameObject.layer = 8;
                _closed = false;

            }
            else
            {
                _spriteRenderer.enabled = false;
                _box.enabled = false;
                gameObject.tag = "Untagged";
                gameObject.layer = 0;
                _closed = true;
            }
        }
    }
}
