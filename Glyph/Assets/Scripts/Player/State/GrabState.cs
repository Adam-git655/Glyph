using UnityEngine;

public class GrabState : PlayerState
{
    private int _dashDirection;
    
    public GrabState(PlayerController playerController, PlayerStateMachine stateMachine, Animator animator, string animationName) : base(playerController, stateMachine, animator, animationName)
    {
    }

    public override void Enter()
    {
        base.Enter();

        _dashDirection = playerController.GetFacingDirection();
    }

    public override void Update()
    {
        base.Update();
    }

    public override void Exit()
    {
        base.Exit();
    }
}
