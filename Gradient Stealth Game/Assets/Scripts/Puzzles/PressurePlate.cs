using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PressurePlate : MonoBehaviour
{
    [field: SerializeField] private int _assignmentCode = 0;

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
        if(collision.tag == "RegionDetector")
        {
            GameObject mainObject = collision.transform.parent.gameObject;

            if (mainObject.tag == "Enemy") 
            {
                if (_assignmentCode != 0) 
                {
                    EventManager.EventTrigger(EventType.ASSIGNMENT_CODE_TRIGGER, _assignmentCode);
                }
            }
        }
    }
}
