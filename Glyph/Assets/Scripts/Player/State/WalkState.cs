using UnityEngine;

public class WalkState : PlayerState
{
    private float _s;
    private float _oldSpeed;
    
    public WalkState(PlayerController playerController, PlayerStateMachine stateMachine, Animator animator, string animationName) : base(playerController, stateMachine, animator, animationName)
    {
    }

    public override void Enter()
    {
        base.Enter();
        
        animator.SetFloat("sprintMultiplier" , 1f);
       
        _oldSpeed = Mathf.Abs(playerController.GetRigidbody().linearVelocityX);
        _s = _oldSpeed;
    }

    public override void Update()
    {
        base.Update();
        
        playerController.Flip();

        if (!playerController.IsGrounded() && playerController.GetRigidbody().linearVelocityY < 0) stateMachine.ChangeState(playerController.FallState);
        if (playerController.IsMoving() == false) stateMachine.ChangeState(playerController.IdleState);
        if(Input.GetKeyDown(KeyCode.LeftControl)) stateMachine.ChangeState(playerController.SprintState);
        if(Input.GetKeyDown(KeyCode.C)) stateMachine.ChangeState(playerController.CrouchState);
        if (playerController.CanJump()) stateMachine.ChangeState(playerController.JumpState);
        if (Input.GetKeyDown(KeyCode.LeftShift) && playerController.CanDash())
        {
            playerController.UseDash();
            stateMachine.ChangeState(playerController.DashState);
        }
        if(Input.GetKeyDown(KeyCode.S)) stateMachine.ChangeState(playerController.SlideState);

        
        _s = Mathf.Lerp(_s, playerController.WalkSpeed, playerController.WalkAcceleration * Time.deltaTime);
        
        playerController.Move(Vector2.right * playerController.XInput, _s, true);
    }

    public override void Exit()
    {
        base.Exit();
    }
}
