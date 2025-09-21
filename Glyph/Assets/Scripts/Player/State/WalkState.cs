using UnityEngine;

public class WalkState : GroundState
{
   
    private int walkMultiplierID = Animator.StringToHash("walkMultiplier");
    
    public WalkState(PlayerController playerController, PlayerContext playerContext, PlayerStateMachine stateMachine, Animator animator, string animationName) : base(playerController, playerContext, stateMachine, animator, animationName)
    {
    }

    public override void Enter()
    {
        base.Enter();
        
        animator.SetFloat(walkMultiplierID, 1f);
    }

    public override void Update()
    {
        base.Update();
        
        playerController.Flip();
        
        if(playerContext.IsMoving() == false) stateMachine.ChangeState(playerController.IdleState);
        if(playerContext.IsSliding()) stateMachine.ChangeState(playerController.SlideState);
        if(playerContext.IsCrouching()) stateMachine.ChangeState(playerController.CrouchState);

        playerController.Move(playerContext.xInput, playerContext.walkSpeed);
    }

    public override void Exit()
    {
        base.Exit();
    }
}
