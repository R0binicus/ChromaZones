using UnityEngine.Assertions;

public class StateMachine
{
    public State CurrentState;

    public StateMachine(State startingState)
    {
        CurrentState = startingState;
        CurrentState.Enter();
    }

    public void ChangeState(State newState)
    {
        Assert.IsNotNull(newState);
        CurrentState.Exit();
        CurrentState = newState;
        CurrentState.Enter();
    }
}
