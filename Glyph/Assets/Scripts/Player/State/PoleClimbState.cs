using UnityEngine;

public class PoleClimbState : PlayerState
{
    private const string _poleClimbMove = "poleClimbMove";
    private bool _climbing = false;
    
    public PoleClimbState(PlayerController playerController, PlayerStateMachine stateMachine, Animator animator, string animationName) : base(playerController, stateMachine, animator, animationName)
    {
    }

    public override void Enter()
    {
        base.Enter();
        
        playerController.DisableGravity();
    }

    public override void Update()
    {
        base.Update();

        if (playerController.IsPoleDetected() == false)
        {
            if (playerController.IsGrounded())
                stateMachine.ChangeState(playerController.IdleState);
            else
                stateMachine.ChangeState(playerController.FallState);
        }
        
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (playerController.XInput != 0 && !Mathf.Approximately(playerController.XInput, playerController.GetFacingDirection()))
                stateMachine.ChangeState(playerController.JumpState);
            else
                stateMachine.ChangeState(playerController.FallState);
        }

        float yInput = Input.GetAxisRaw("Vertical");

        if (yInput != 0) _climbing = true;
        else _climbing = false;

        playerController.Move(Vector2.up * yInput, 70, false);
        
        animator.SetBool(_poleClimbMove, _climbing);
    }

    public override void Exit()
    {
        base.Exit();
        playerController.EnableGravity();
    }
}
