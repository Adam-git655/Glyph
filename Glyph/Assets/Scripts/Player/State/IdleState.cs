using UnityEngine;

public class IdleState : PlayerState
{

    private float _s;
    private float _oldSpeed;
    
    public IdleState(PlayerController playerController, PlayerStateMachine stateMachine, Animator animator, string animationName) : base(playerController, stateMachine, animator, animationName)
    {
    }

    public override void Enter()
    {
        base.Enter();

        _oldSpeed = Mathf.Abs(playerController.GetRigidbody().linearVelocityX);
        _s = _oldSpeed;
    }

    public override void Update()
    {
        base.Update();
        
        if(playerController.IsMoving()) stateMachine.ChangeState(playerController.WalkState);
        if(playerController.IsMoving() && Input.GetKey(KeyCode.LeftControl)) stateMachine.ChangeState(playerController.SprintState);
        if(Input.GetKeyDown(KeyCode.C)) stateMachine.ChangeState(playerController.CrouchState);
        if(playerController.CanJump()) stateMachine.ChangeState(playerController.JumpState);
        if (Input.GetKeyDown(KeyCode.LeftShift) && playerController.CanDash())
        {
            playerController.UseDash();
            stateMachine.ChangeState(playerController.DashState);
        }

        _s = Mathf.Lerp(_s, 0, playerController.Deacceleration * Time.deltaTime);
        playerController.Move(Vector2.right * playerController.GetFacingDirection(), _s, true);
    }

    public override void Exit()
    {
        base.Exit();
    }
}

