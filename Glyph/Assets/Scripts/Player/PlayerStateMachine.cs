using UnityEngine;

public class PlayerStateMachine
{
    public PlayerState CurrentState { get; private set; }
    private PlayerState blockedState = null;

    public void EnterState(PlayerState newState)
    {
        CurrentState = newState;
        CurrentState.Enter();
    }

    public void ChangeState(PlayerState newState)
    {
        if (CurrentState == blockedState)
            return;

        CurrentState.Exit();
        CurrentState = newState;
        CurrentState.Enter();
    }

    public void BlockState(PlayerState state)
    {
        blockedState = state;
    }

    public void UnBlockAnyBlockedState()
    {
        blockedState = null;
    }
}
