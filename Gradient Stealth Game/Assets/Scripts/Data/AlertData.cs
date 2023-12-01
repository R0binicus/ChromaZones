using Unity.VisualScripting;
using UnityEngine;

public class AlertData
{
    [field:SerializeField] public float AlertOthersRadius { get; private set; }
    [field:SerializeField] public Vector3 Centre;

    public AlertData(Vector3 Centre, float AlertOthersRadius)
    {
        this.Centre = Centre;
        this.AlertOthersRadius = AlertOthersRadius;
    }
}
