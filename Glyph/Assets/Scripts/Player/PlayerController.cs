using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    private Rigidbody2D _rb;
    private Animator _animator;
    private Camera _mainCam;

    private PlayerContext _playerContext;

    public PlayerStateMachine StateMachine { get; private set; }

    #region PlayerSettings

    [SerializeField] private float walkSpeed;
    [SerializeField] private float runSpeed;
    [SerializeField] private float jumpForce;
    [SerializeField] private int maxJumpCount;
    [SerializeField] private float dashForce;
    [SerializeField] private float dashDuration;
    [SerializeField] private float dashCooldownDuration;
    [SerializeField] private float slideSpeed;
    [SerializeField] private float slideDuration;
    [SerializeField] private float wallSlideSpeed;
    
    private float _coyoteTimer;
    [SerializeField] private float coyoteDuration;
    
    [field: SerializeField] public float JumpBufferDuration { get; private set; }

    private int _facingDirection = 1;

    #endregion

    #region GroundCheck

    [Space(10f), Header("GroundCheck"), Space(5f)] 
    
    [SerializeField] private Transform groundCheckPoint;
    [SerializeField] private float groundCheckDistance;
    [SerializeField] public LayerMask groundLayer;

    [SerializeField] private float wallCheckDistance;
    [SerializeField] private Vector2 wallCheckOffset;
    [SerializeField] private Vector2 wallCheckSize;
        
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
    public GrabState GrabState { get; private set; }
    public DashState DashState { get; private set; }
    public SlideState SlideState { get; private set; }
    public PoleClimbState PoleClimbState { get; private set; }
    public FallState FallState { get; private set; }
    public WallSlideState WallSlideState { get; private set; }
    public WallJumpState WallJumpState { get; private set; }
    
    #endregion

    [SerializeField] private Transform ballThrowPoint;
    [SerializeField] private GameObject ballPrefab;
    
    private float _defaultGravity;
    
    private void Awake()
    {
        _mainCam = Camera.main;
        _rb = GetComponent<Rigidbody2D>();
        _animator = GetComponentInChildren<Animator>();

        _playerContext = new PlayerContext
        {
            walkSpeed = walkSpeed,
            runSpeed = runSpeed,
            jumpForce = jumpForce,
            jumpCount = maxJumpCount,
            dashForce = dashForce,
            dashDuration = dashDuration,
            dashCooldownDuration = dashCooldownDuration,
            slideSpeed = slideSpeed,
            slideDuration = slideDuration,
            jumpBufferDuration = JumpBufferDuration,
            wallSlideSpeed = wallSlideSpeed
        };
        
        StateMachine = new PlayerStateMachine();
        
        #region Assign States

        IdleState = new IdleState(this, _playerContext , StateMachine , _animator , "idle");
        WalkState = new WalkState(this, _playerContext , StateMachine , _animator , "walk");
        SprintState = new SprintState(this,_playerContext ,StateMachine, _animator, "walk");
        CrouchState = new CrouchState(this,_playerContext ,StateMachine, _animator, "crouch");
        JumpState = new JumpState(this,_playerContext ,StateMachine, _animator, "jump");
        GrabState = new GrabState(this,_playerContext ,StateMachine, _animator, "grab");
        DashState = new DashState(this,_playerContext ,StateMachine, _animator, "dash");
        SlideState = new SlideState(this,_playerContext, StateMachine, _animator, "slide");
        PoleClimbState = new PoleClimbState(this,_playerContext, StateMachine, _animator, "poleClimb");
        FallState = new FallState(this, _playerContext, StateMachine, _animator, "fall");
        WallSlideState = new WallSlideState(this, _playerContext, StateMachine, _animator, "wallSlide");
        WallJumpState = new WallJumpState(this, _playerContext, StateMachine, _animator, "jump");

        #endregion
    }

    private void Start()
    {
        _playerContext.moveMode = MoveMode.Walk;
        
        _facingDirection = 1;
        _defaultGravity = _rb.gravityScale;

        StateMachine.EnterState(IdleState);
    }

    private void Update()
    {
        GetInputs();
        CheckIsGrounded();
        CheckIsOnWall();

        StateMachine.CurrentState.Update();
        
        //TryThrowBall();
    }

    private void GetInputs()
    {
        _playerContext.xInput = Input.GetAxisRaw("Horizontal");
        _playerContext.yInput = Input.GetAxisRaw("Vertical");

        if (Input.GetKeyDown(KeyCode.Q))
        {
            if (_playerContext.moveMode == MoveMode.Walk) _playerContext.moveMode = MoveMode.Sprint;
            else _playerContext.moveMode = _playerContext.moveMode = MoveMode.Walk;
        }
    }

    public void ResetVelocity() => _rb.ResetVelocity();
    
    private void TryThrowBall()
    {
        Vector2 mousePosition = _mainCam.ScreenToWorldPoint(new Vector3(Input.mousePosition.x,
            Input.mousePosition.y, Mathf.Abs(_mainCam.transform.position.z)));
        
        Vector2 direction = (mousePosition - (Vector2)transform.position).normalized;

        /*if (CanThrowBall(mousePosition))
        {
            GameObject ball = Instantiate(ballPrefab, ballThrowPoint.position, Quaternion.identity);
            if (ball.TryGetComponent(out Ball b))
            {
                b.Init(direction , 30f);
            }
        }*/
    }

    private bool CanThrowBall(Vector2 mousePosition)
    {
        return Input.GetMouseButtonDown(0) &&((_facingDirection == 1 && mousePosition.x > ballThrowPoint.position.x) ||
          _facingDirection == -1 && mousePosition.x < ballThrowPoint.position.x);
    }
    
    public void Move(float xDirection, float xSpeed,float ySpeed = 0f , bool applyDefaultGravity = true)
    {
        Vector2 moveDirection = Vector2.right * xDirection;

        if(applyDefaultGravity) ySpeed = _rb.linearVelocity.y;
        _rb.linearVelocity = new Vector2(moveDirection.x * xSpeed, ySpeed);
    }
    
    public void ApplyJump() => _rb.AddForce(Vector2.up * jumpForce , ForceMode2D.Impulse);

    public void CheckIsGrounded()
    {
        _playerContext.isGrounded = Physics2D.Raycast(groundCheckPoint.position, Vector2.down, groundCheckDistance, groundLayer);
        if (_playerContext.isGrounded)
        {
            if (_rb.linearVelocityY > 0f) return;
            if(_playerContext.jumpCount != maxJumpCount) _playerContext.jumpCount = maxJumpCount;
        }
    }

    public void CheckIsOnWall()
    {
        RaycastHit2D h = Physics2D.BoxCast(transform.position + (Vector3)wallCheckOffset, wallCheckSize, 0f,
            transform.right * _facingDirection, wallCheckDistance, groundLayer);
        
        _playerContext.isOnWall = h.collider != null;
    }
    
    public void Flip(float x = 0f)
    {
        if (x is not 0) PerformFlip();
        
        if (_playerContext.xInput is 0) return;

        if (!Mathf.Approximately(_playerContext.xInput, _facingDirection)) PerformFlip();
    }

    private void PerformFlip()
    {
        _facingDirection *= -1;

        //playerVisual.transform.localScale =
        //    new Vector3(playerVisual.transform.localScale.x * -1, playerVisual.transform.localScale.y, playerVisual.transform.localScale.z);
            
        if (_facingDirection == -1)
            transform.localEulerAngles = new Vector3(0, 180, 0);
        else transform.localEulerAngles = Vector3.zero;
    }
    
    public int GetFacingDirection() => _facingDirection;
    
    public Rigidbody2D GetRigidbody() => _rb;

    private void OnDrawGizmos()
    {
        if (groundCheckPoint == null) return;
        if (poleCheckPoint == null) return;

        
        Gizmos.color = Color.red;

        Gizmos.DrawWireCube(transform.position + (Vector3)wallCheckOffset, wallCheckSize);
        
        Gizmos.color = Color.white;
        Gizmos.DrawRay(groundCheckPoint.position, Vector2.down * groundCheckDistance);
        Gizmos.DrawRay(poleCheckPoint.position, Vector2.right * _facingDirection * poleCheckDistance);

        Gizmos.color = Color.green;
        
        Gizmos.DrawWireCube(transform.position + (Vector3)wallCheckOffset + Vector3.right * _facingDirection * wallCheckDistance , 
            wallCheckSize);
    }
}
