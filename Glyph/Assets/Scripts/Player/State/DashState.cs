using UnityEngine;

public class DashState : PlayerState
{
    private float _dashTimer;
    private float _dashDirection;
    
    public DashState(PlayerController playerController, PlayerContext playerContext, PlayerStateMachine stateMachine, Animator animator, string animationName) : base(playerController, playerContext, stateMachine, animator, animationName)
    {
    }

    public override void Enter()
    {
        base.Enter();

        _dashTimer = playerContext.dashDuration;
        _dashDirection = playerController.GetFacingDirection();
    }

    public override void Update()
    {
        base.Update();

        _dashTimer -= Time.deltaTime;

        //if(!playerController.CheckIsGrounded() && playerController.IsPoleDetected()) stateMachine.ChangeState(playerController.PoleClimbState);
        
        if (_dashTimer <= 0)
        {
            if (playerContext.isGrounded)
            {
                if (playerContext.IsMoving()) stateMachine.ChangeState(playerController.WalkState);
                else stateMachine.ChangeState(playerController.IdleState);
            }else 
                stateMachine.ChangeState(playerController.FallState);
        }
        
        if(playerContext.IsWallSliding()) stateMachine.ChangeState(playerController.WallSlideState);
        
        playerController.Move(_dashDirection, playerContext.dashForce, applyDefaultGravity:false);
    }

    public override void Exit()
    {
        base.Exit();
        playerContext.dashCoolDownTimer = playerContext.dashCooldownDuration;
        playerController.ResetVelocity();
    }
}
