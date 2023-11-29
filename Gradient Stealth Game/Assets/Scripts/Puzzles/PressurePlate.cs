using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PressurePlate : MonoBehaviour
{
    [field: SerializeField] private int _assignmentCode = 0;

    private bool _activated = false;

    // Start is called before the first frame update
    void Awake()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!_activated && _assignmentCode != 0)
        {
            if(collision.tag == "RegionDetector")
            {
                GameObject mainObject = collision.transform.parent.gameObject;

                if (mainObject.tag == "Enemy") 
                {
                    ActivatePlate();
                }
            }
        }
    }

    private void ActivatePlate()
    {
        _activated = true;
        EventManager.EventTrigger(EventType.ASSIGNMENT_CODE_TRIGGER, _assignmentCode);
        GetComponent<SpriteRenderer>().color = Color.HSVToRGB(0f, 0f, 0.5f);
    }
}
