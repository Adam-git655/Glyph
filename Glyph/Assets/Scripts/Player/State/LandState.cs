using UnityEngine;

public class LandState : PlayerState
{
    private float _timer;
    private float _fallSpeed;
    private float _landDuration;


    public LandState(PlayerController playerController, PlayerStateMachine stateMachine, Animator animator, string animationName)
        : base(playerController, stateMachine, animator, animationName)
    {
    }

    public void SetFallSpeed(float speed)
    {
        _fallSpeed = speed;
    }

    public override void Enter()
    {
        base.Enter();

        _landDuration = Mathf.Clamp(_fallSpeed * 0.01f, 0.01f, 0.2f);
        _timer = _landDuration;
        playerController.GetRigidbody().linearVelocityX = 0;
        playerController.CreateDust();
    }

    public override void Update()
    {
        base.Update();

        _timer -= Time.deltaTime;

        if (_timer <= 0)
        {
            if (playerController.IsMoving())
            {
                if (Input.GetKey(KeyCode.LeftControl))
                    stateMachine.ChangeState(playerController.SprintState);
                else
                    stateMachine.ChangeState(playerController.WalkState);
            }
            else
            {
                stateMachine.ChangeState(playerController.IdleState);
            }
        }
    }

    public override void Exit()
    {
        base.Exit();
    }
}
