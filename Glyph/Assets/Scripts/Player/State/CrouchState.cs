using UnityEngine;

public class CrouchState : PlayerState
{

    public CrouchState(PlayerController playerController, PlayerContext playerContext, PlayerStateMachine stateMachine, Animator animator, string animationName) : base(playerController, playerContext, stateMachine, animator, animationName)
    {
    }

    public override void Enter()
    {
        base.Enter();
    }

    public override void Update()
    {
        base.Update();

        playerController.Flip();
        
        if (playerContext.IsCrouching() == false && CanStandUp())
        {
            if(playerContext.IsMoving()) stateMachine.ChangeState(playerController.WalkState);
            else stateMachine.ChangeState(playerController.IdleState);
        }
    }

    public override void Exit()
    {
        base.Exit();
    }

    private bool CanStandUp()
    {
        Vector2 origin = playerController.transform.position + Vector3.up * .5f;
        float circleRadius = 0.5f;

        Collider2D hit = Physics2D.OverlapCircle(origin, circleRadius, playerController.groundLayer);

        if(hit is null) Debug.Log("Can standup!");
        else Debug.Log("cannot standup!");
        
        return hit is null;
    }
}
