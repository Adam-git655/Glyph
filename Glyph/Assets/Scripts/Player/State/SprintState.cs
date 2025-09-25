using UnityEngine;

public class SprintState : PlayerState
{
    private const string sprintMultiplier = "sprintMultiplier";

    private float _s;
    private float _oldSpeed;
    
    public SprintState(PlayerController playerController, PlayerStateMachine stateMachine, Animator animator, string animationName) : base(playerController, stateMachine, animator, animationName)
    {
    }


    public override void Enter()
    {
        base.Enter();

        playerController.CreateDust();
        
        animator.SetFloat(sprintMultiplier , 1.3f);

        _oldSpeed = Mathf.Abs(playerController.GetRigidbody().linearVelocityX);
        _s = _oldSpeed;
    }

    public override void Update()
    {
        base.Update();
        
        playerController.Flip();

        if (!playerController.IsGrounded() && playerController.GetRigidbody().linearVelocityY < 0) stateMachine.ChangeState(playerController.FallState);
        if (playerController.IsMoving() == false) stateMachine.ChangeState(playerController.IdleState);
        if(Input.GetKeyUp(KeyCode.LeftControl)) stateMachine.ChangeState(playerController.WalkState);
        if(playerController.CanJump()) stateMachine.ChangeState(playerController.JumpState);
        if(Input.GetKeyDown(KeyCode.S)) stateMachine.ChangeState(playerController.SlideState);
        if (Input.GetKeyDown(KeyCode.LeftShift) && playerController.CanDash())
        {
            playerController.UseDash();
            stateMachine.ChangeState(playerController.DashState);
        }

        _s = Mathf.Lerp(_s, playerController.RunSpeed, playerController.RunAcceleration * Time.deltaTime);
        playerController.Move(Vector2.right * playerController.XInput, _s, true);
    }

    public override void Exit()
    {
        base.Exit();
        animator.SetFloat(sprintMultiplier, 1f);
    }
}
