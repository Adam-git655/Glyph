using UnityEngine;

public class FallState : PlayerState
{
    public FallState(PlayerController playerController, PlayerContext playerContext, PlayerStateMachine stateMachine, Animator animator, string animationName) : base(playerController, playerContext, stateMachine, animator, animationName)
    {
    }

    public override void Enter()
    {
        base.Enter();
        
    }

    public override void Update()
    {
        base.Update();

        playerController.Flip();
        
        if (playerContext.isGrounded)
        {
            if (playerContext.IsMoving())
            {
                if(playerContext.IsSprinting()) stateMachine.ChangeState(playerController.SprintState);
                else stateMachine.ChangeState(playerController.WalkState);
                
            }else stateMachine.ChangeState(playerController.IdleState);
        }
        
        if(playerContext.IsDashing()) stateMachine.ChangeState(playerController.DashState);
        // IF TOUCHING WALL -- GO TO WALL SLIDE STATE
        
        if(playerContext.IsWallSliding()) stateMachine.ChangeState(playerController.WallSlideState);
        if(playerContext.IsJumping()) stateMachine.ChangeState(playerController.JumpState); 
        
        playerController.Move(playerContext.xInput, playerContext.walkSpeed);
    }

    public override void Exit()
    {
        base.Exit();
    }
}
