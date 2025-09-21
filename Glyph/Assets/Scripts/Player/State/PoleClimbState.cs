using UnityEngine;

public class PoleClimbState : PlayerState
{
    private const string _poleClimbMove = "poleClimbMove";
    private bool _climbing = false;


    public PoleClimbState(PlayerController playerController, PlayerContext playerContext, PlayerStateMachine stateMachine, Animator animator, string animationName) : base(playerController, playerContext, stateMachine, animator, animationName)
    {
    }

    public override void Enter()
    {
        base.Enter();
        
    }

    public override void Update()
    {
        base.Update();
        
        
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (playerContext.xInput != 0 && !Mathf.Approximately(playerContext.xInput, playerController.GetFacingDirection()))
                stateMachine.ChangeState(playerController.JumpState);
            else
                stateMachine.ChangeState(playerController.IdleState);
        }

        float yInput = Input.GetAxisRaw("Vertical");

        if (yInput != 0) _climbing = true;
        else _climbing = false;

        //playerController.Move(Vector2.up * yInput, 40, false);
        
        animator.SetBool(_poleClimbMove, _climbing);
    }

    public override void Exit()
    {
        base.Exit();
    }
}
