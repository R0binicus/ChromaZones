using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PressurePlate : MonoBehaviour
{
    [field: SerializeField] private int _sentAssignmentCode = 0;
    [field: SerializeField] private int _recievedAssignmentCode = 0;
    [field: SerializeField] private bool _playerMode = false;
    [field: SerializeField] private SpriteRenderer _fillSprite;
    [field: SerializeField] private bool _disabled = false;
    [field: SerializeField] private bool _resetOnAssignmentCodeTrigger = false;
    private SpriteRenderer _borderSprite;
    public float _originalHue;

    // Start is called before the first frame update
    void Awake()
    {
        _borderSprite = GetComponent<SpriteRenderer>();
        Color.RGBToHSV(_fillSprite.color, out var H, out var S, out var V);
        _originalHue = H * 360;
        if (_disabled)
        {
            DisablePlate();
        }
    }

    private void OnEnable()
    {
        if (_recievedAssignmentCode != 0 && _resetOnAssignmentCodeTrigger)
        {
            EventManager.EventSubscribe(EventType.ASSIGNMENT_CODE_TRIGGER, AssignmentCodeHandler);
        }
    }

    private void OnDisable()
    {
        EventManager.EventUnsubscribe(EventType.ASSIGNMENT_CODE_TRIGGER, AssignmentCodeHandler);
        StopAllCoroutines();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (_sentAssignmentCode != 0)
        {
            if(collision.tag == "RegionDetector")
            {
                GameObject mainObject = collision.transform.parent.gameObject;

                if (mainObject.tag == "Enemy" && !_playerMode) 
                {
                    ActivatePlate();
                }
                else if (mainObject.tag == "Player" && _playerMode)
                {
                    ActivatePlate();
                }
            }
        }
    }

    private void ActivatePlate()
    {
        if (!_disabled)
        {
            DisablePlate();
            EventManager.EventTrigger(EventType.ASSIGNMENT_CODE_TRIGGER, _sentAssignmentCode);
            StartCoroutine(DisableForASec());
        }
    }

    private void DisablePlate()
    {
        _disabled = true;
        _borderSprite.color = Color.HSVToRGB(0f, 0f, 0.5f);
        _fillSprite.color = Color.HSVToRGB(0f, 0f, 0.5f);
    }

    private void EnablePlate()
    {
        _disabled = false;
        _borderSprite.color = Color.HSVToRGB(0f, 0f, 1f);
        _fillSprite.color = Color.HSVToRGB(_originalHue/360, 1f, 0.6f);
    }

    private void AssignmentCodeHandler(object data)
    {
        if (data == null)
        {
            Debug.Log("PressurePlate AssignmentCodeHandler is null");
        }
        if (_recievedAssignmentCode == (int)data)
        {
            StartCoroutine(DelayAssignmentCheck());
        }
    }

    private IEnumerator DisableForASec()
    {
        _resetOnAssignmentCodeTrigger = false;
        yield return new WaitForSeconds(0.2f);
        _resetOnAssignmentCodeTrigger = true;
    }

    private IEnumerator DelayAssignmentCheck()
    {
        yield return new WaitForSeconds(0.1f);
        if (_resetOnAssignmentCodeTrigger)
            {
                if (_disabled)
                {
                    EnablePlate();
                }
                else
                {
                    DisablePlate();
                }
            }
    }
}
