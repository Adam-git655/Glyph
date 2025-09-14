using UnityEngine;

public class DashState : PlayerState
{
    private float _dashTimer;
    
    public DashState(PlayerController playerController, PlayerStateMachine stateMachine, Animator animator, string animationName) : base(playerController, stateMachine, animator, animationName)
    {
    }

    public override void Enter()
    {
        base.Enter();

        _dashTimer = playerController.DashDuration;
    }

    public override void Update()
    {
        base.Update();

        _dashTimer -= Time.deltaTime;

        if(!playerController.IsGrounded() && playerController.IsPoleDetected()) stateMachine.ChangeState(playerController.PoleClimbState);
        
        if (_dashTimer <= 0)
        {
            if(playerController.IsMoving()) stateMachine.ChangeState(playerController.IdleState);
            else stateMachine.ChangeState(playerController.IdleState);
        }
        
        playerController.Move(Vector2.right * playerController.GetFacingDirection(), playerController.DashSpeed, false);
    }

    public override void Exit()
    {
        base.Exit();
    }
}
