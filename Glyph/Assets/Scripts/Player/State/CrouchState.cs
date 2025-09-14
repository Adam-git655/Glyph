using UnityEditor;
using UnityEngine;

public class CrouchState : PlayerState
{
    private const string crouchWalk = "crouchWalk";

    private float _s;
    private float _oldSpeed;
    
    public CrouchState(PlayerController playerController, PlayerStateMachine stateMachine, Animator animator, string animationName) : base(playerController, stateMachine, animator, animationName)
    {
    }

    public override void Enter()
    {
        base.Enter();

        playerController.CrouchCollider.enabled = true;
        playerController.DefaultCollider.enabled = false;
        playerController.SlideCollider.enabled = false;

        _oldSpeed = Mathf.Abs(playerController.GetRigidbody().linearVelocityX);
        _s = _oldSpeed;
    }

    public override void Update()
    {
        base.Update();

        playerController.Flip();
        
        // TODO : FIRST CHECK IF SOMETHING IS UP THERE DON'T STAND 

        if (Input.GetKeyDown(KeyCode.C) && CanStandUp())
        {
            if(playerController.IsMoving()) stateMachine.ChangeState(playerController.WalkState);
            else stateMachine.ChangeState(playerController.IdleState);
        }

        _s = Mathf.Lerp(_s, playerController.CrouchSpeed, playerController.CrouchAcceleration * Time.deltaTime);
        playerController.Move(Vector2.right * playerController.XInput , _s);

        animator.SetBool(crouchWalk , playerController.IsMoving());
    }

    public override void Exit()
    {
        base.Exit();

        playerController.DefaultCollider.enabled = true;
        playerController.CrouchCollider.enabled = false;
        playerController.SlideCollider.enabled = false;

        animator.SetBool(crouchWalk, false);
    }

    private bool CanStandUp()
    {
        Vector2 colliderCenter = (Vector2)playerController.transform.position + playerController.DefaultCollider.offset;
        Vector2 circlePosition = colliderCenter + new Vector2(0f, playerController.DefaultCollider.size.y / 2f);
        float circleRadius = 0.3f;

        Collider2D hit = Physics2D.OverlapCircle(circlePosition, circleRadius, playerController.groundLayer);

        return hit == null;
    }
}
