using UnityEngine;

public class GroundState : PlayerState
{
    public GroundState(PlayerController playerController, PlayerContext playerContext, PlayerStateMachine stateMachine, Animator animator, string animationName) : base(playerController, playerContext, stateMachine, animator, animationName)
    {
    }


    public override void Enter()
    {
        base.Enter();
    }

    public override void Update()
    {
        base.Update();

        if (playerContext.IsJumping()) stateMachine.ChangeState(playerController.JumpState);
        if (playerContext.IsFalling()) stateMachine.ChangeState(playerController.FallState);
        if(playerContext.IsSprinting() && playerContext.IsMoving()) stateMachine.ChangeState(playerController.SprintState);
        if(playerContext.IsDashing()) stateMachine.ChangeState(playerController.DashState);
    }

    public override void Exit()
    {
        base.Exit();
    }
}
