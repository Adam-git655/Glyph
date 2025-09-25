using System;
using System.Collections;
using UnityEditor.Tilemaps;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    private Rigidbody2D _rb;
    private Animator _animator;

    public PlayerStateMachine StateMachine { get; private set; }

    [SerializeField] private GameObject playerVisual;

    [SerializeField] public TrailRenderer trailRenderer;

    [SerializeField] public ParticleSystem DustPS;

    [SerializeField] public Transform respawnPoint;
    
    #region PlayerSettings

    [field: SerializeField] public float WalkSpeed { get; private set; }
    [field: SerializeField] public float RunSpeed { get; private set; }
    [field: SerializeField] public float CrouchSpeed { get; private set; }
    [field: SerializeField] public float JumpForce { get; private set; }
    [field: SerializeField] public float DashSpeed { get; private set; }
    [field: SerializeField] public float DashDuration { get; private set; } 
    [field: SerializeField] public float WalkAcceleration { get; private set; }
    [field: SerializeField] public float RunAcceleration { get; private set; }
    [field: SerializeField] public float AirAcceleration { get; private set; }
    [field: SerializeField] public float CrouchAcceleration { get; private set; }
    [field: SerializeField] public float Deacceleration { get; private set; }
    [field: SerializeField] public float AirDeacceleration { get; private set; }
    [field: SerializeField] public float SlideDuration { get; private set; }
    [field: SerializeField] public float SlideSpeed { get; private set; }
    
    private float _coyoteTimer;
    [SerializeField] private float coyoteDuration;
    
    [HideInInspector] public float jumpBufferTimer;
    [field: SerializeField] public float JumpBufferDuration { get; private set; }

    private int _facingDirection = 1;
    [field: SerializeField] private float dustVelocityThreshold;

    private bool canDash = true;
    private bool hasAirDashed = false;
    [field: SerializeField] private float dashCooldown = 1f;
    private float timeSinceLastDash;

    [field: SerializeField] public BoxCollider2D DefaultCollider { get; private set; }
    [field: SerializeField] public BoxCollider2D SlideCollider { get; private set; }
    [field: SerializeField] public BoxCollider2D CrouchCollider { get; private set; }
    
    #endregion

    #region GroundCheck

    [Space(10f), Header("GroundCheck"), Space(5f)] 
    
    [SerializeField] private Transform groundCheckPoint;
    [SerializeField] private float groundCheckDistance;
    [SerializeField] public LayerMask groundLayer;
    
    #endregion
    
    #region PoleCheck

    [Space(10f) , Header("PoleCheck") , Space(5f)]
    
    [SerializeField] private Transform poleCheckPoint;
    [SerializeField] private float poleCheckDistance;
    [SerializeField] private LayerMask poleLayerMask;
    
    #endregion
    
    #region PlayerState

    public IdleState IdleState { get; private set; }
    public WalkState WalkState { get; private set; }
    public SprintState SprintState { get; private set; }
    public CrouchState CrouchState { get; private set; }
    public JumpState JumpState { get; private set; }
    public FallState FallState { get; private set; }
    public LandState LandState { get; private set; }
    public GrabState GrabState { get; private set; }
    public DashState DashState { get; private set; }
    public SlideState SlideState { get; private set; }
    public PoleClimbState PoleClimbState { get; private set; }
    
    #endregion

    private float _defaultGravity;
    
    public float XInput
    {
        get { return Input.GetAxisRaw("Horizontal"); }
       private set{}
    }

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _animator = GetComponentInChildren<Animator>();

        StateMachine = new PlayerStateMachine();
        
        #region Assign States

        IdleState = new IdleState(this , StateMachine , _animator , "idle");
        WalkState = new WalkState(this , StateMachine , _animator , "walk");
        SprintState = new SprintState(this, StateMachine, _animator, "walk");
        CrouchState = new CrouchState(this, StateMachine, _animator, "crouch");
        JumpState = new JumpState(this, StateMachine, _animator, "jump");
        FallState = new FallState(this, StateMachine, _animator);
        LandState = new LandState(this, StateMachine, _animator, "land");
        GrabState = new GrabState(this, StateMachine, _animator, "grab");
        DashState = new DashState(this, StateMachine, _animator, "dash");
        SlideState = new SlideState(this, StateMachine, _animator, "slide");
        PoleClimbState = new PoleClimbState(this, StateMachine, _animator, "poleClimb");

        #endregion
    }

    private void Start()
    {
        _facingDirection = 1;
        _defaultGravity = _rb.gravityScale;

        DefaultCollider.enabled = true;
        SlideCollider.enabled = false;
        CrouchCollider.enabled = false;
        
        StateMachine.EnterState(IdleState);
    }

    private void Update()
    {
        jumpBufferTimer -= Time.deltaTime;
        timeSinceLastDash += Time.deltaTime;

        if (timeSinceLastDash > dashCooldown)
            canDash = true;

        if (IsGrounded())
        {
            _coyoteTimer = coyoteDuration;
            hasAirDashed = false;
        }
        else _coyoteTimer -= Time.deltaTime;
        
        StateMachine.CurrentState.Update();
    }

    public void Move(Vector2 moveDirection, float moveSpeed, bool applyGravity = true)
    {
        if(applyGravity) moveDirection.y = _rb.linearVelocity.y;
        _rb.linearVelocity = new Vector2(moveDirection.x * moveSpeed, moveDirection.y);
    }

    public void ApplyJump() => _rb.AddForce(Vector2.up * JumpForce , ForceMode2D.Impulse);

    public bool CanJump()
    {
        return (Input.GetKeyDown(KeyCode.Space) && (_coyoteTimer > 0 || IsGrounded()))
               || (jumpBufferTimer > 0 && IsGrounded());
    }

    public bool CanDash()
    {
        if (!canDash) return false;
        if (!IsGrounded() && hasAirDashed) return false;
        return true;
    }

    public void UseDash()
    {
        canDash = false;
        timeSinceLastDash = 0f;

        if (!IsGrounded())
        {
            hasAirDashed = true;
        }
    }

    public bool IsPoleDetected()
    {
        return Physics2D.Raycast(poleCheckPoint.position, Vector2.right * _facingDirection, poleCheckDistance,
            poleLayerMask);
    }
    
    public bool IsGrounded()
    {
        return Physics2D.Raycast(groundCheckPoint.position, Vector2.down, groundCheckDistance, groundLayer);
    }

    public void DisableGravity() => _rb.SetZeroGravity();
    public void EnableGravity() => _rb.gravityScale = _defaultGravity;
    
    public void Flip()
    {
        if (XInput is 0) return;

        if (!Mathf.Approximately(XInput, _facingDirection))
        {
            if (Mathf.Abs(_rb.linearVelocityX) > dustVelocityThreshold)
            {
                CreateDust();
            }
            
            _facingDirection *= -1;

            playerVisual.transform.localScale =
                new Vector3(playerVisual.transform.localScale.x * -1, playerVisual.transform.localScale.y, playerVisual.transform.localScale.z);

            //Flip the colliders of the actual player

            Vector2 newOffset = DefaultCollider.offset;
            newOffset.x *= -1;
            DefaultCollider.offset = newOffset;

            newOffset = CrouchCollider.offset;
            newOffset.x *= -1;
            CrouchCollider.offset = newOffset;

            newOffset = SlideCollider.offset;
            newOffset.x *= -1;
            SlideCollider.offset = newOffset;
        }
    }

    public int GetFacingDirection() => _facingDirection;
    public bool IsMoving() => XInput != 0;
    public Rigidbody2D GetRigidbody() => _rb;

    public void Slide()
    {
        playerVisual.transform.localPosition = new Vector3(0f, -1.5f, 0f);
    }
    
    public void SlideEnd() => playerVisual.transform.localPosition = new Vector3(0f, -2f, 0f);

    public void CreateDust()
    {
        DustPS.Play();
    }

    public void Die()
    {
        Debug.Log("player dead");
        transform.position = respawnPoint.position;
    }

    public IEnumerator LerpPlayer(Vector3 start, Vector3 end, float duration)
    {
        float timeElapsed = 0f;

        while(timeElapsed < duration)
        {
            transform.position = Vector3.Lerp(start, end, timeElapsed / duration);
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        EnableGravity();
        transform.position = end;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawRay(groundCheckPoint.position, Vector2.down * groundCheckDistance);
        Gizmos.DrawRay(poleCheckPoint.position, Vector2.right * _facingDirection * poleCheckDistance);
    }
}
