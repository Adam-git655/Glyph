using UnityEngine;

public class PlayerState
{
    protected PlayerController playerController;
    protected PlayerStateMachine stateMachine;
    protected PlayerContext playerContext;   
    protected Animator animator;
    
    private string _animationName;
    
    public PlayerState(PlayerController playerController ,PlayerContext playerContext ,PlayerStateMachine stateMachine , Animator animator , string animationName)
    {
        this.playerController = playerController;
        this.playerContext = playerContext;
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
        playerContext.dashCoolDownTimer = Mathf.Clamp(playerContext.dashCoolDownTimer -= Time.deltaTime
            , -5f, playerContext.dashCooldownDuration);

        playerContext.jumpBufferTimer = Mathf.Clamp(playerContext.jumpBufferTimer -= Time.deltaTime,
            -5f, playerContext.jumpBufferDuration);
    }

    public virtual void Exit()
    {
        animator.SetBool(_animationName, false);
    }
}

// TODO : ALMOST IN EVERY STATE CHECK IF NOT ON GROUND THEN SWITCH TO FALL STATE
