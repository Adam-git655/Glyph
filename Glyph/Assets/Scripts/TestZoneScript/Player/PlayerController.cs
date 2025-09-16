using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class PlayerControllerTest : MonoBehaviour
{
    private Rigidbody2D _rb;
    private Collider2D _collider;
    private LineRenderer _lineRenderer;
    private AbilityManager _abilityManager;
    private PlayerInput _input;
    private Camera _mainCamera;
    private CameraController _cameraController;

    [Header("Movement Settings")]
    [SerializeField] private float _speed = 5f;
    [SerializeField] private float _jumpForce = 10f;
    [SerializeField] private float _climbSpeed = 3f;
    [SerializeField] private LayerMask _groundLayer;

    [Header("Camera")]
    [Tooltip("If left empty, will use Camera.main")]
    [SerializeField] private Camera _mainCameraOverride;

    [Header("Debug")]
    [SerializeField] private bool _debugDrawAim = false;

    private Vector2 _moveInput;
    private Vector2 _mouseScreenPos;
    private bool _onLadder;
    private bool _jumpRequested;
    private bool _useAbilityRequested;
    private bool _cyclePrevRequested;
    private bool _cycleNextRequested;
    private bool _isFacingRight = true;
    private float _defaultGravity;
    private float _fallSpeedDampingChangeThreshold;
    private AbilityPickup _nearbyPickup;
    private bool _pickupRequested = false;
    public AbilityPickup GetNearbyPickup() => _nearbyPickup;

    void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _collider = GetComponent<Collider2D>();
        _lineRenderer = GetComponent<LineRenderer>();
        _abilityManager = GetComponent<AbilityManager>();

        if (_lineRenderer != null)
        {
            _lineRenderer.positionCount = 2;
        }

        _input = new PlayerInput();
        _defaultGravity = _rb.gravityScale;

        _mainCamera = _mainCameraOverride != null ? _mainCameraOverride : Camera.main;
        _cameraController = CameraController._instance;
    }

    void Start()
    {
        if (_mainCamera == null)
            Debug.LogWarning("PlayerController: no camera assigned and Camera.main is null.");

        _cameraController = CameraController._instance;
        if (_cameraController != null)
        {
            _fallSpeedDampingChangeThreshold = _cameraController._fallSpeedYDampingChangeThreshold;
            _cameraController.SetFacingDirection(_isFacingRight);
        }
    }

    void OnEnable() => _input.Enable();
    void OnDisable() => _input.Disable();

    void Update()
    {
        if (InventoryManager._instance != null && InventoryManager._instance.IsPaused) return;
        _moveInput = _input.PlayerControls.Move.ReadValue<Vector2>();
        _mouseScreenPos = _input.PlayerControls.MousePosition.ReadValue<Vector2>();

        if (_input.PlayerControls.Jump.triggered) _jumpRequested = true;
        if (_input.PlayerControls.UseAbility.triggered) _useAbilityRequested = true;
        if (_input.PlayerControls.CyclePrev.triggered) _cyclePrevRequested = true;
        if (_input.PlayerControls.CycleNext.triggered) _cycleNextRequested = true;
        if (_input.PlayerControls.Pickup.triggered) _pickupRequested = true;

        if (_mainCamera != null)
        {
            float zDist = Mathf.Abs(_mainCamera.transform.position.z - transform.position.z);
            Vector3 mouseWorld3 = _mainCamera.ScreenToWorldPoint(new Vector3(_mouseScreenPos.x, _mouseScreenPos.y, zDist));
            Vector2 mouseWorld = mouseWorld3;
            if (_debugDrawAim)
            {
                Debug.DrawLine(transform.position, mouseWorld, Color.cyan);
            }

            Vector2 aimDir = (mouseWorld - (Vector2)transform.position).normalized;
            HandleFacing(aimDir.x);
            UpdateAimLine(aimDir);

            if (_useAbilityRequested)
            {
                _abilityManager?.UseCurrentAbility(transform.position, mouseWorld);
                _useAbilityRequested = false;
            }
        }

        if (_cyclePrevRequested) { _abilityManager?.CyclePrev(); _cyclePrevRequested = false; }
        if (_cycleNextRequested) { _abilityManager?.CycleNext(); _cycleNextRequested = false; }
        if (_pickupRequested) { TryPickup(); _pickupRequested = false; }
    }

    private void FixedUpdate()
    {
        if (_onLadder)
        {
            _rb.gravityScale = 0f;
            _rb.linearVelocity = new Vector2(_moveInput.x * _speed, _moveInput.y * _climbSpeed);
        }
        else
        {
            _rb.gravityScale = _defaultGravity;

            // set horizontal velocity
            Vector2 v = _rb.linearVelocity;
            v.x = _moveInput.x * _speed;
            _rb.linearVelocity = v;

            // jump
            if (_jumpRequested && IsGrounded())
            {
                v.y = _jumpForce;
                _rb.linearVelocity = v;
            }
            _jumpRequested = false;

            // camera fall detection
            if (_cameraController != null)
            {
                float vy = _rb.linearVelocity.y;
                if (vy < _fallSpeedDampingChangeThreshold && !_cameraController._isLerpingYDamping && !_cameraController._lerpedFromPlayerFalling)
                {
                    _cameraController.LerpYDamping(true);
                }
                else if (vy >= 0f && !_cameraController._isLerpingYDamping && _cameraController._lerpedFromPlayerFalling)
                {
                    _cameraController._lerpedFromPlayerFalling = false;
                    _cameraController.LerpYDamping(false);
                }
            }
        }
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

    private bool IsGrounded()
    {
        if (_collider == null) return false;

        Bounds bounds = _collider.bounds;
        Vector2 origin = new Vector2(bounds.center.x, bounds.min.y);
        float extraHeight = 0.1f;

        RaycastHit2D hit = Physics2D.Raycast(origin, Vector2.down, extraHeight, _groundLayer);
        if (_debugDrawAim)
        {
            Debug.DrawRay(origin, Vector2.down * extraHeight, hit.collider ? Color.green : Color.red);
        }
        return hit.collider != null;
    }

    private void TryPickup()
    {
        if (_nearbyPickup == null)
        {
            return;
        }

        Ability ability = _nearbyPickup.GetAbility();
        if (ability == null)
        {
            Debug.LogWarning("PlayerController.TryPickup: pickup has no Ability assigned.");
            return;
        }

        bool added = _abilityManager != null && _abilityManager.AddAbility(ability);
        if (added)
        {
            Destroy(_nearbyPickup.gameObject);
            _nearbyPickup = null;
        }
        else
        {
            Debug.Log("Unable to pick up ability (duplicate or inventory full).");
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Ladder"))
        {
            _onLadder = true;
            _rb.linearVelocity = Vector2.zero;
        }

        if (other.TryGetComponent<AbilityPickup>(out var pickup))
        {
            _nearbyPickup = pickup;
            Debug.Log($"Press E to pick up {pickup.GetDisplayName()}");
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Ladder"))
        {
            _onLadder = false;
        }

        if (_nearbyPickup != null && other.TryGetComponent<AbilityPickup>(out var pickup) && pickup == _nearbyPickup)
        {
            _nearbyPickup = null;
        }
    }
}
