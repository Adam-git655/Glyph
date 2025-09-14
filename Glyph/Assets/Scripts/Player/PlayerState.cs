using UnityEngine;

public class PlayerState
{
    protected PlayerController playerController;
    protected PlayerStateMachine stateMachine;
    protected Animator animator;
        
    private string _animationName;
    
    public PlayerState(PlayerController playerController , PlayerStateMachine stateMachine , Animator animator , string animationName)
    {
        this.playerController = playerController;
        this.stateMachine = stateMachine;
        this.animator = animator;
        
        _animationName = animationName;
    }

    public virtual void Enter()
    {
        animator.SetBool(_animationName, true);
    }

    public virtual void Update()
    {
    }

    public virtual void Exit()
    {
        animator.SetBool(_animationName, false);
    }
}

// TODO : ALMOST IN EVERY STATE CHECK IF NOT ON GROUND THEN SWITCH TO FALL STATE
