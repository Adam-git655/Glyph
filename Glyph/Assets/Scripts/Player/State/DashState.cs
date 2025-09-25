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
        playerController.trailRenderer.emitting = true;
    }

    public override void Update()
    {
        base.Update();

        _dashTimer -= Time.deltaTime;

        if (!playerController.IsGrounded() && playerController.IsPoleDetected())
        {
            stateMachine.ChangeState(playerController.PoleClimbState);
            return;
        }

        if (_dashTimer > 0)
        {
            playerController.Move(Vector2.right * playerController.GetFacingDirection(), playerController.DashSpeed, false);
        }
        else
        {
            if (playerController.IsGrounded())
            {
                if (playerController.IsMoving())
                    stateMachine.ChangeState(playerController.WalkState);
                else
                    stateMachine.ChangeState(playerController.IdleState);
            }
            else
            {
                stateMachine.ChangeState(playerController.FallState);
            }
        }
    }

    public override void Exit()
    {
        playerController.trailRenderer.emitting = false;
        base.Exit();
    }
}
