using UnityEngine;

public class JumpState : PlayerState
{
    private float _fixTimer = .1f;

    private float _oldSpeed;
    
    public JumpState(PlayerController playerController, PlayerStateMachine stateMachine, Animator animator, string animationName) : base(playerController, stateMachine, animator, animationName)
    {
    }

    public override void Enter()
    {
        base.Enter();

        _fixTimer = 0.1f;

        _oldSpeed = playerController.GetRigidbody().linearVelocityX;
        
        playerController.GetRigidbody().ResetVelocityY(); // RESET THE Y VELOCITY BEFORE JUMPING
        playerController.ApplyJump();
        playerController.CreateDust();
    }

    public override void Update()
    {
        base.Update();

        playerController.Flip();
        
        _fixTimer -= Time.deltaTime;
        
        if (playerController.IsGrounded() && _fixTimer <= 0)
        {
            if (playerController.IsMoving())
            {
                if(Input.GetKey(KeyCode.LeftControl)) stateMachine.ChangeState(playerController.SprintState); 
                else stateMachine.ChangeState(playerController.WalkState);
                
            }else stateMachine.ChangeState(playerController.IdleState); 
        }

        if (Input.GetKeyDown(KeyCode.Space)) playerController.jumpBufferTimer = playerController.JumpBufferDuration;
        if(playerController.IsPoleDetected()) stateMachine.ChangeState(playerController.PoleClimbState);
        if (Input.GetKeyDown(KeyCode.LeftShift) && playerController.CanDash())
        {
            playerController.UseDash();
            stateMachine.ChangeState(playerController.DashState);
        }

        playerController.Move(Vector2.right * playerController.XInput, playerController.WalkSpeed, true);
    }

    public override void Exit()
    {
        base.Exit();
    }
}
