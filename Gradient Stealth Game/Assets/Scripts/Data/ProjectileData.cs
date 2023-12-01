using Unity.VisualScripting;
using UnityEngine;

[System.Serializable]
public class ProjectileData
{
    [field:SerializeField] public float Speed { get; private set; }
    [field:SerializeField] public float LifeTime { get; private set; }

    public ProjectileData(float Speed, float LifeTime)
    {
        this.Speed = Speed;
        this.LifeTime = LifeTime;
    }
}
