using UnityEngine;

public class SprintState : GroundState
{
    private int walkMultiplierID = Animator.StringToHash("walkMultiplier");
    
    public SprintState(PlayerController playerController, PlayerContext playerContext, PlayerStateMachine stateMachine, Animator animator, string animationName) : base(playerController, playerContext, stateMachine, animator, animationName)
    {
    }

    public override void Enter()
    {
        base.Enter();
        
        animator.SetFloat(walkMultiplierID, 1.3f);
    }

    public override void Update()
    {
        base.Update();
        
        playerController.Flip();
        
        if(playerContext.IsMoving() == false) stateMachine.ChangeState(playerController.IdleState);
        if(playerContext.IsSprinting() == false && playerContext.IsMoving() && playerContext.isGrounded) stateMachine.ChangeState(playerController.WalkState);
        if(playerContext.IsSliding()) stateMachine.ChangeState(playerController.SlideState);
        if(playerContext.IsJumping()) stateMachine.ChangeState(playerController.JumpState);
        
        playerController.Move(playerContext.xInput, playerContext.runSpeed);
    }

    public override void Exit()
    {
        base.Exit();
        
        animator.SetFloat(walkMultiplierID,1f);
    }
}
