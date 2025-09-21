using UnityEngine;

public class WallSlideState : PlayerState
{
    private float slideSpeed;
    private float xInput;
    
    public WallSlideState(PlayerController playerController, PlayerContext playerContext, PlayerStateMachine stateMachine, Animator animator, string animationName) : base(playerController, playerContext, stateMachine, animator, animationName)
    {
    }

    public override void Enter()
    {
        base.Enter();
        
        slideSpeed = 0f;
        xInput = 0f;
    }

    public override void Update()
    {
        base.Update();

        if (playerContext.isGrounded) stateMachine.ChangeState(playerController.IdleState);
        if(playerContext.isOnWall == false && playerContext.isGrounded == false) stateMachine.ChangeState(playerController.FallState);
        
        if (playerContext.yInput < 0) slideSpeed = playerContext.wallSlideSpeed * 2f; 
        else if (playerContext.yInput == 0f) slideSpeed = playerContext.wallSlideSpeed;

        if(playerContext.xInput != 0 && !Mathf.Approximately(playerContext.xInput, playerController.GetFacingDirection()))
            xInput = playerContext.xInput;

        if (Input.GetKeyDown(KeyCode.Space))
        {
            stateMachine.ChangeState(playerController.WallJumpState);
            return;
        }
        
        playerController.Move( xInput,5f, slideSpeed, applyDefaultGravity:false);
    }

    public override void Exit()
    {
        base.Exit();
    }
}
