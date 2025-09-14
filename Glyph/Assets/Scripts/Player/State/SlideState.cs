using UnityEngine;

public class SlideState : PlayerState
{
    private float _slideTimer;
    private int _facingDirection;
    
    public SlideState(PlayerController playerController, PlayerStateMachine stateMachine, Animator animator, string animationName) : base(playerController, stateMachine, animator, animationName)
    {
    }

    public override void Enter()
    {
        base.Enter();

        playerController.SlideCollider.enabled = true;
        playerController.DefaultCollider.enabled = false;
        playerController.CrouchCollider.enabled = false;

        _slideTimer = playerController.SlideDuration;
        _facingDirection = playerController.GetFacingDirection();
        
    }

    public override void Update()
    {
        base.Update();
        playerController.Slide();

        
        _slideTimer -= Time.deltaTime;
        
        if (_slideTimer < 0)
        {
            if (playerController.IsMoving())
            {
                if(Input.GetKey(KeyCode.LeftControl)) stateMachine.ChangeState(playerController.SprintState);
                else stateMachine.ChangeState(playerController.WalkState);
            }
            else stateMachine.ChangeState(playerController.IdleState);
        }

        if (!playerController.IsGrounded())
        {
            // GO TO FALL STATE
        }
        
        playerController.Move(Vector2.right * _facingDirection, playerController.SlideSpeed, true);
    }

    public override void Exit()
    {
        base.Exit();

        playerController.DefaultCollider.enabled = true;
        playerController.SlideCollider.enabled = false;
        playerController.CrouchCollider.enabled = false;

        playerController.SlideEnd();
    }
}
