using UnityEngine;

public abstract class State
{
    public virtual void Enter() {}

    public virtual void LogicUpdate() {}

    public virtual void PhysicsUpdate() {}

    public virtual void Exit() {}

    public virtual void Check() {}

    public virtual void OnCollisionEnter2D(Collision2D collision) {}
}
