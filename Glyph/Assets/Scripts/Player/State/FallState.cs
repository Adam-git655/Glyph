using UnityEngine;

public class FallState : PlayerState
{
    private float _s;
    private float _oldSpeed;

    public FallState(PlayerController playerController, PlayerStateMachine stateMachine, Animator animator) : base(playerController, stateMachine, animator, null)
    {
    }

    public override void Enter()
    {
        _oldSpeed = playerController.GetRigidbody().linearVelocityX;
        _s = _oldSpeed;
    }

    public override void Update()
    {
        base.Update();

        playerController.Flip();

        if (playerController.IsGrounded())
        {
            float fallSpeed = Mathf.Abs(playerController.GetRigidbody().linearVelocityY);
            playerController.LandState.SetFallSpeed(fallSpeed);
            stateMachine.ChangeState(playerController.LandState);
            return;
        }

        if (playerController.IsPoleDetected()) stateMachine.ChangeState(playerController.PoleClimbState);

        if (Input.GetKeyDown(KeyCode.LeftShift) && playerController.CanDash())
        {
            playerController.UseDash();
            stateMachine.ChangeState(playerController.DashState);
            return;
        }

        if (Mathf.Abs(playerController.XInput) > 0.01f)
        {
            _s = Mathf.Lerp(_s, playerController.XInput * playerController.WalkSpeed, playerController.AirAcceleration * Time.deltaTime);
        }
        else
        {
            _s = Mathf.Lerp(_s, 0, playerController.AirDeacceleration * Time.deltaTime);
        }

        playerController.Move(Vector2.right * Mathf.Sign(_s), Mathf.Abs(_s), true);
    }

    public override void Exit()
    {
    }
}
