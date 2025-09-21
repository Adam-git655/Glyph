using UnityEngine;

public class WallJumpState : PlayerState
{
    private Vector2 _jumpDirection;
    private float _jumpTimer;
    
    public WallJumpState(PlayerController playerController, PlayerContext playerContext, PlayerStateMachine stateMachine, Animator animator, string animationName) : base(playerController, playerContext, stateMachine, animator, animationName)
    {
    }

    public override void Enter()
    {
        base.Enter();

        _jumpTimer = .2f;
        playerController.Flip(playerController.GetFacingDirection() * -1f);
        
        _jumpDirection = new Vector2(1f * playerController.GetFacingDirection() , .5f) ;
        playerController.GetRigidbody().AddForce(_jumpDirection * playerContext.jumpForce, ForceMode2D.Impulse);

    }

    public override void Update()
    {
        base.Update();

        _jumpTimer -= Time.deltaTime;
        
        
        if(Input.GetKeyDown(KeyCode.R)) stateMachine.ChangeState(playerController.IdleState);
        if(_jumpTimer < 0) stateMachine.ChangeState(playerController.FallState);
    }

    public override void Exit()
    {
        base.Exit();
    }
}
