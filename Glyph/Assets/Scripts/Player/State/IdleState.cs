using UnityEngine;

public class IdleState : GroundState
{
    public IdleState(PlayerController playerController, PlayerContext playerContext, PlayerStateMachine stateMachine, Animator animator, string animationName) : base(playerController, playerContext, stateMachine, animator, animationName)
    {
    }

    public override void Enter()
    {
        base.Enter();
    }

    public override void Update()
    {
        base.Update();

        playerController.GetRigidbody().ResetVelocityX();
        
        if(playerContext.IsMoving()) stateMachine.ChangeState(playerController.WalkState);
        if(playerContext.IsCrouching()) stateMachine.ChangeState(playerController.CrouchState);
    }

    public override void Exit()
    {
        base.Exit();
    }
}
