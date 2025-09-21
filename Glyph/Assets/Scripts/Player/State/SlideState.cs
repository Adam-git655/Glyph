using UnityEngine;

public class SlideState : PlayerState
{
    private float _slideTimer;
    private int _facingDirection;

    public SlideState(PlayerController playerController, PlayerContext playerContext, PlayerStateMachine stateMachine, Animator animator, string animationName) : base(playerController, playerContext, stateMachine, animator, animationName)
    {
    }

    public override void Enter()
    {
        base.Enter();

        _slideTimer = playerContext.slideDuration;
        _facingDirection = playerController.GetFacingDirection();
    }

    public override void Update()
    {
        base.Update();
        
        _slideTimer -= Time.deltaTime;
        
        if (_slideTimer < 0)
        {
            if (playerContext.IsMoving())
            {
                if(playerContext.IsSprinting()) stateMachine.ChangeState(playerController.SprintState);
                else stateMachine.ChangeState(playerController.WalkState);
            }
            else stateMachine.ChangeState(playerController.IdleState);
        }

        if (playerContext.isGrounded == false) stateMachine.ChangeState(playerController.FallState);
        
        playerController.Move(_facingDirection, playerContext.slideSpeed);
    }

    public override void Exit()
    {
        base.Exit();
    }
}
