using UnityEngine;

public abstract class EnemyBehaviour : MonoBehaviour
{
    public abstract void ResetBehaviour();
    public abstract void UpdateLogicBehaviour();
    public abstract void UpdatePhysicsBehaviour();
}
