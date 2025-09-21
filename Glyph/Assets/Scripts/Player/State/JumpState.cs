using UnityEngine;

public class JumpState : PlayerState
{

    public JumpState(PlayerController playerController, PlayerContext playerContext, PlayerStateMachine stateMachine, Animator animator, string animationName) : base(playerController, playerContext, stateMachine, animator, animationName)
    {
    }

    public override void Enter()
    {
        base.Enter();
        
        playerContext.jumpCount--;
        
        playerController.GetRigidbody().ResetVelocityY(); // RESET THE Y VELOCITY BEFORE JUMPING
        playerController.ApplyJump();
    }

    public override void Update()
    {
        base.Update();

        playerController.Flip();

        if (playerController.GetRigidbody().linearVelocity.y < 0) stateMachine.ChangeState(playerController.FallState);
        
        // ADD COYOTE TIMER
        
        if(playerContext.IsDashing()) stateMachine.ChangeState(playerController.DashState);
        if(playerContext.IsWallSliding()) stateMachine.ChangeState(playerController.WallSlideState);
        //if(playerController.IsPoleDetected()) stateMachine.ChangeState(playerController.PoleClimbState);
        
        playerController.Move(playerContext.xInput, playerContext.walkSpeed);
    }

    public override void Exit()
    {
        base.Exit();
    }
}
