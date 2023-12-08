using Unity.VisualScripting;
using UnityEngine;

public class AlertData
{
    [field:SerializeField] public float AlertOthersRadius { get; private set; }
    [field:SerializeField] public Vector3 Centre;

    [field:SerializeField] public int Type; // 0 = enemies, 1 = projectile

    public AlertData(Vector3 Centre, float AlertOthersRadius, int Type)
    {
        this.Centre = Centre;
        this.AlertOthersRadius = AlertOthersRadius;
        this.Type = Type;
    }
}
