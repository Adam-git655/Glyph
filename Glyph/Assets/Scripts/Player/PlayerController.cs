// PlayerController.cs
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Cinemachine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(PlayerInteractor))]
public class PlayerController : MonoBehaviour
{
    #region Components
    private Rigidbody2D _rb;
    private Collider2D _collider;
    private LineRenderer _lineRenderer;
    private AbilityManager _abilityManager;
    private InputSystem_Actions _input;
    private Camera _mainCamera;
    private CameraController _cameraController;
    private PlayerInteractor _interactor;
    private Animator _animator;
    #endregion

    #region Visuals & Interaction
    [Header("Visuals")]
    [SerializeField] private GameObject _playerVisual;

    [Header("Interaction")]
    [Tooltip("Optional: assign a child/sibling Collider2D (Is Trigger) used to detect interactables.")]
    [SerializeField] private Collider2D _interactionCollider;
    #endregion

    #region Movement Settings (exposed as public properties for state classes)
    [field: SerializeField] public float WalkSpeed { get; private set; } = 5f;
    [field: SerializeField] public float RunSpeed { get; private set; } = 8f;
    [field: SerializeField] public float CrouchSpeed { get; private set; } = 2.5f;
    [field: SerializeField] public float JumpForce { get; private set; } = 12f;
    [field: SerializeField] public float DashSpeed { get; private set; } = 14f;
    [field: SerializeField] public float DashDuration { get; private set; } = 0.16f;
    [field: SerializeField] public float WalkAcceleration { get; private set; } = 50f;
    [field: SerializeField] public float RunAcceleration { get; private set; } = 80f;
    [field: SerializeField] public float CrouchAcceleration { get; private set; } = 30f;
    [field: SerializeField] public float Deacceleration { get; private set; } = 40f;
    [field: SerializeField] public float SlideDuration { get; private set; } = 0.6f;
    [field: SerializeField] public float SlideSpeed { get; private set; } = 12f;
    #endregion

    #region Timers & Buffers
    private float _coyoteTimer = 0f;
    [SerializeField] private float _coyoteDuration = 0.12f;
    [HideInInspector] public float jumpBufferTimer = 0f;
    [field: SerializeField] public float JumpBufferDuration { get; private set; } = 0.12f;
    #endregion

    #region Colliders (public fields expected by state classes)
    [field: SerializeField] public BoxCollider2D DefaultCollider { get; private set; }
    [field: SerializeField] public BoxCollider2D SlideCollider { get; private set; }
    [field: SerializeField] public BoxCollider2D CrouchCollider { get; private set; }
    #endregion

    #region Ground & Pole Checks
    [Header("Ground & Pole Checks")]
    [SerializeField] private Transform _groundCheckPoint;
    [SerializeField] private float _groundCheckDistance = 0.12f;
    [SerializeField] public LayerMask groundLayer;
    [SerializeField] private Transform _poleCheckPoint;
    [SerializeField] private float _poleCheckDistance = 0.6f;
    [SerializeField] private LayerMask _poleLayerMask;
    #endregion

    #region Camera & Debug
    [Header("Camera & Input")]
    [SerializeField] private Camera _mainCameraOverride;
    [SerializeField] private bool _debugDrawAim = false;
    #endregion

    #region Runtime state
    private Vector2 _moveInput;
    private Vector2 _mouseScreenPos;
    private bool _onLadder = false;
    private bool _jumpRequested = false;
    private bool _useAbilityRequested = false;
    private bool _cyclePrevRequested = false;
    private bool _cycleNextRequested = false;
    private bool _isFacingRight = true;
    private float _defaultGravity;
    private float _fallSpeedDampingChangeThreshold = -15f;

    private bool _canDash = true;
    private bool _hasAirDashed = false;
    private float _timeSinceLastDash = 0f;
    [SerializeField] private float _dashCooldown = 1f;
    #endregion

    #region State machine (colleague classes expect these)
    public PlayerStateMachine StateMachine { get; private set; }
    public IdleState IdleState { get; private set; }
    public WalkState WalkState { get; private set; }
    public SprintState SprintState { get; private set; }
    public CrouchState CrouchState { get; private set; }
    public JumpState JumpState { get; private set; }
    public GrabState GrabState { get; private set; }
    public DashState DashState { get; private set; }
    public SlideState SlideState { get; private set; }
    public PoleClimbState PoleClimbState { get; private set; }
    #endregion

    #region Legacy-compatible XInput (keeps colleague state code unchanged)
    public float XInput
    {
        get { return Input.GetAxisRaw("Horizontal"); }
        private set { }
    }
    #endregion

    #region Unity callbacks
    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _collider = GetComponent<Collider2D>();
        _lineRenderer = GetComponent<LineRenderer>();
        _abilityManager = GetComponent<AbilityManager>();
        _interactor = GetComponent<PlayerInteractor>();
        _animator = GetComponentInChildren<Animator>();
        _input = new InputSystem_Actions();

        if (_lineRenderer != null) _lineRenderer.positionCount = 2;
        _defaultGravity = _rb.gravityScale;

        _mainCamera = _mainCameraOverride != null ? _mainCameraOverride : Camera.main;
        _cameraController = CameraController._instance;
        if (_cameraController != null)
        {
            _fallSpeedDampingChangeThreshold = _cameraController._fallSpeedYDampingChangeThreshold;
            _cameraController.SetFacingDirection(_isFacingRight);
        }

        // Setup states (use colleague state classes)
        StateMachine = new PlayerStateMachine();
        IdleState = new IdleState(this, StateMachine, _animator, "idle");
        WalkState = new WalkState(this, StateMachine, _animator, "walk");
        SprintState = new SprintState(this, StateMachine, _animator, "walk");
        CrouchState = new CrouchState(this, StateMachine, _animator, "crouch");
        JumpState = new JumpState(this, StateMachine, _animator, "jump");
        GrabState = new GrabState(this, StateMachine, _animator, "grab");
        DashState = new DashState(this, StateMachine, _animator, "dash");
        SlideState = new SlideState(this, StateMachine, _animator, "slide");
        PoleClimbState = new PoleClimbState(this, StateMachine, _animator, "poleClimb");

        // IMPORTANT: if interaction collider sits on a child, assign it to the PlayerInteractor so triggers are forwarded
        if (_interactor != null)
        {
            _interactor.SetInteractionCollider(_interactionCollider);
        }
    }

    private void Start()
    {
        if (DefaultCollider != null) DefaultCollider.enabled = true;
        if (SlideCollider != null) SlideCollider.enabled = false;
        if (CrouchCollider != null) CrouchCollider.enabled = false;

        StateMachine.EnterState(IdleState);
    }

    private void OnEnable() => _input.Enable();
    private void OnDisable() => _input.Disable();

    private void Update()
    {
        if (InventoryManager._instance != null && InventoryManager._instance.IsPaused) return;

        // New Input System reads
        _moveInput = _input.Player.Move.ReadValue<Vector2>();
        _mouseScreenPos = _input.Player.MousePosition.ReadValue<Vector2>();

        if (_input.Player.Jump.triggered)
        {
            _jumpRequested = true;
            jumpBufferTimer = JumpBufferDuration;
        }

        if (_input.Player.UseAbility.triggered) _useAbilityRequested = true;
        if (_input.Player.CyclePrev.triggered) _cyclePrevRequested = true;
        if (_input.Player.CycleNext.triggered) _cycleNextRequested = true;

        if (_input.Player.Interact.triggered)
        {
            _interactor?.TryInteract();
        }

        // timers
        jumpBufferTimer -= Time.deltaTime;
        _timeSinceLastDash += Time.deltaTime;
        if (_timeSinceLastDash > _dashCooldown) _canDash = true;

        // Aim + facing (keeps your ability integration)
        if (_mainCamera != null)
        {
            float zDist = Mathf.Abs(_mainCamera.transform.position.z - transform.position.z);
            Vector3 mouseWorld3 = _mainCamera.ScreenToWorldPoint(new Vector3(_mouseScreenPos.x, _mouseScreenPos.y, zDist));
            Vector2 mouseWorld = mouseWorld3;

            if (_debugDrawAim) Debug.DrawLine(transform.position, mouseWorld, Color.cyan);

            Vector2 aimDir = (mouseWorld - (Vector2)transform.position).normalized;
            HandleFacing(aimDir.x);
            UpdateAimLine(aimDir);

            if (_useAbilityRequested)
            {
                _abilityManager?.UseCurrentAbility(transform.position, mouseWorld);
                _useAbilityRequested = false;
            }
        }

        if (_cyclePrevRequested)
        {
            _abilityManager?.CyclePrev();
            _cyclePrevRequested = false;
        }

        if (_cycleNextRequested)
        {
            _abilityManager?.CycleNext();
            _cycleNextRequested = false;
        }

        // coyote timer
        if (IsGrounded())
        {
            _coyoteTimer = _coyoteDuration;
            _hasAirDashed = false;
        }
        else
        {
            _coyoteTimer -= Time.deltaTime;
        }

        // colleague-style state Update
        StateMachine.CurrentState?.Update();

        // camera fall damping handling
        if (_cameraController != null)
        {
            float vy = _rb.linearVelocity.y;
            if (vy < _fallSpeedDampingChangeThreshold && !_cameraController._isLerpingYDamping && !_camera_controller_check_reset())
            {
                _camera_controller_check_reset(); // call to reset handled inside method
                _cameraController.LerpYDamping(true);
            }
            else if (vy >= 0f)
            {
                _camera_controller_check_reset();
            }
        }
    }

    private bool _camera_controller_check_reset()
    {
        if (_cameraController == null) return false;
        if (!_cameraController._isLerpingYDamping && _cameraController._lerpedFromPlayerFalling)
        {
            _cameraController._lerpedFromPlayerFalling = false;
            _cameraController.LerpYDamping(false);
        }
        return true;
    }

    private void FixedUpdate()
    {
        if (_onLadder)
        {
            _rb.gravityScale = 0f;
            _rb.linearVelocity = new Vector2(_moveInput.x * WalkSpeed, _moveInput.y * 3f);
            return;
        }

        _rb.gravityScale = _defaultGravity;
    }
    #endregion

    #region Movement & helpers (used by states)
    public void Move(Vector2 moveDirection, float moveSpeed, bool applyGravity = true)
    {
        if (!applyGravity)
        {
            _rb.linearVelocity = new Vector2(moveDirection.x * moveSpeed, moveDirection.y);
            return;
        }
        _rb.linearVelocity = new Vector2(moveDirection.x * moveSpeed, _rb.linearVelocity.y);
    }

    public void ApplyJump()
    {
        _rb.AddForce(Vector2.up * JumpForce, ForceMode2D.Impulse);
    }

    public bool CanJump()
    {
        return (Input.GetKeyDown(KeyCode.Space) && (_coyoteTimer > 0f || IsGrounded()))
               || (jumpBufferTimer > 0f && IsGrounded());
    }

    public bool CanDash()
    {
        if (!_canDash) return false;
        if (!IsGrounded() && _hasAirDashed) return false;
        return true;
    }

    public void UseDash()
    {
        _canDash = false;
        _timeSinceLastDash = 0f;
        if (!IsGrounded()) _hasAirDashed = true;
    }

    public bool IsPoleDetected()
    {
        if (_poleCheckPoint == null) return false;
        Vector2 dir = Vector2.right * (_isFacingRight ? 1f : -1f);
        return Physics2D.Raycast(_poleCheckPoint.position, dir, _poleCheckDistance, _poleLayerMask);
    }

    public bool IsGrounded()
    {
        if (_groundCheckPoint != null)
        {
            RaycastHit2D hit = Physics2D.Raycast(_groundCheckPoint.position, Vector2.down, _groundCheckDistance, groundLayer);
            if (_debugDrawAim) Debug.DrawRay(_groundCheckPoint.position, Vector2.down * _groundCheckDistance, hit.collider ? Color.green : Color.red);
            return hit.collider != null;
        }
        else
        {
            if (_collider == null) return false;
            Bounds bounds = _collider.bounds;
            Vector2 origin = new Vector2(bounds.center.x, bounds.min.y);
            float extraHeight = 0.1f;
            RaycastHit2D hit = Physics2D.Raycast(origin, Vector2.down, extraHeight, groundLayer);
            if (_debugDrawAim) Debug.DrawRay(origin, Vector2.down * extraHeight, hit.collider ? Color.green : Color.red);
            return hit.collider != null;
        }
    }

    public void DisableGravity() => _rb.gravityScale = 0f;
    public void EnableGravity() => _rb.gravityScale = _defaultGravity;

    public void Flip()
    {
        if (XInput == 0) return;
        if (!Mathf.Approximately(XInput, GetFacingDirection()))
        {
            Vector3 s = _playerVisual != null ? _playerVisual.transform.localScale : transform.localScale;
            s.x = s.x * -1f;
            if (_playerVisual != null) _playerVisual.transform.localScale = s;
            else transform.localScale = s;

            _isFacingRight = !_isFacingRight;
        }
    }

    public int GetFacingDirection() => _isFacingRight ? 1 : -1;
    public bool IsMoving() => !Mathf.Approximately(XInput, 0f);
    public Rigidbody2D GetRigidbody() => _rb;

    public void Slide()
    {
        if (_playerVisual != null)
            _playerVisual.transform.localPosition = new Vector3(0f, -1.5f, 0f);
    }

    public void SlideEnd()
    {
        if (_playerVisual != null)
            _playerVisual.transform.localPosition = new Vector3(0f, -2f, 0f);
    }

    public IEnumerator LerpPlayer(Vector3 start, Vector3 end, float duration)
    {
        float timeElapsed = 0f;
        DisableGravity();
        while (timeElapsed < duration)
        {
            transform.position = Vector3.Lerp(start, end, timeElapsed / duration);
            timeElapsed += Time.deltaTime;
            yield return null;
        }
        EnableGravity();
        transform.position = end;
    }
    #endregion

    #region Camera / Ability Helpers
    public void PanCameraOnContact(float panDistance, float panTime, PanDirection panDirection, bool panToStart)
    {
        _cameraController?.PanCameraOnContact(panDistance, panTime, panDirection, panToStart);
    }

    public void SwapCamera(CinemachineCamera left, CinemachineCamera right, Vector2 dir)
    {
        _cameraController?.SwapCamera(left, right, dir);
    }

    private void HandleFacing(float aimDirX)
    {
        if (aimDirX < 0f && _isFacingRight)
        {
            _isFacingRight = false;
            Vector3 s = transform.localScale;
            s.x = -Mathf.Abs(s.x);
            transform.localScale = s;
            _cameraController?.SetFacingDirection(_isFacingRight);
        }
        else if (aimDirX > 0f && !_isFacingRight)
        {
            _isFacingRight = true;
            Vector3 s = transform.localScale;
            s.x = Mathf.Abs(s.x);
            transform.localScale = s;
            _cameraController?.SetFacingDirection(_isFacingRight);
        }
    }

    private void UpdateAimLine(Vector2 aimDir)
    {
        if (_lineRenderer == null || _abilityManager == null) return;
        float range = _abilityManager.GetCurrentRange();
        Vector2 endPoint = (Vector2)transform.position + aimDir * range;
        _lineRenderer.SetPosition(0, transform.position);
        _lineRenderer.SetPosition(1, endPoint);
    }
    #endregion

    #region Triggers & Gizmos
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Only used if PlayerInteractor is using this GameObject's collider (PlayerInteractor handles forwarded collider if configured)
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        // Only used if PlayerInteractor is using this GameObject's collider
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        // optional
    }

    private void OnDrawGizmos()
    {
        if (_groundCheckPoint != null)
        {
            Gizmos.color = Color.white;
            Gizmos.DrawRay(_groundCheckPoint.position, Vector2.down * _groundCheckDistance);
        }
        if (_poleCheckPoint != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawRay(_poleCheckPoint.position, Vector2.right * GetFacingDirection() * _poleCheckDistance);
        }
    }
    #endregion
}
