using UnityEngine;

[RequireComponent(typeof(Rigidbody2D) , typeof(BoxCollider2D))]
public class Controller: MonoBehaviour
{
    private Rigidbody2D _rb;
    private Animator _animator; 
    private Collider2D _collider;
    private SpriteRenderer[] _spriteRenderer; 
    
    private float _xInput;
    [Space(5f),Header("Move Settings") , Space(5f)]
    [SerializeField] private float walkSpeed;
    [SerializeField] private float runSpeed;
    [SerializeField] private float jumpForce;
    [SerializeField] private int maxJumpCount;
    [SerializeField] private float dashForce;
    [SerializeField] private float dashDuration;
    private float _dashTimer;
    private int _currentJumpCount;
    
    private float _lastMoveInput;
    private int _facingDirection = 1;
    
    private bool _isRunning;
    private bool _jump;
    private bool _isGrounded;
    private bool _isJumping;
    private bool _dash;
    private bool _isDashing;
    private bool _canHide;
    private bool _hide;
    public bool isHidden { get; private set; }
    
    private float _jumpFixTimer; // THIS FIELD TO MAKE JUMP WORK PROPERLY - FIXING ANIMATION , JUMP COUNTER

    [SerializeField] private float objRadius;
    
    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _collider = GetComponent<Collider2D>();
        _animator = GetComponentInChildren<Animator>();
        _spriteRenderer = GetComponentsInChildren<SpriteRenderer>();
    }

    private void Start()
    {
        _currentJumpCount = maxJumpCount;
    }
    
    private void Update()
    {
        // LATER GO WITH RAYCAST FOR GROUND DETECT
        _isGrounded = _collider.IsTouchingLayers(LayerMask.GetMask("Ground"));

        Timers();

        CanHide();
        
        OnGroundReset();
        GetInputs();

        Hide();
        
        if (_dash)
        {
            _dashTimer = dashDuration;
            _dash = false;
        }
        
        _animator.SetBool("isJump", _isJumping);
        if(!_isJumping && _isDashing && _rb.linearVelocity == Vector2.zero) _animator.SetTrigger("idle");
    }

    private void FixedUpdate()
    {
        Move();
        Jump();
        Dash();
    }

    private void Timers()
    {
        _dashTimer -= Time.deltaTime;
        _jumpFixTimer -= Time.deltaTime;
    }
    
    private void OnGroundReset()
    {
        if (_isJumping && _isGrounded && _jumpFixTimer < 0)
        {
            _isJumping = false;
            _currentJumpCount = maxJumpCount;
        }
    }

    private void Hide()
    {
        if (_hide && !isHidden)
        {
            isHidden = true;

            foreach (var sprite in _spriteRenderer)
            {
                sprite.sortingLayerName = "Hidden";
            }
        }

        if (!_hide && isHidden)
        {
            isHidden = false;

            foreach (var sprite in _spriteRenderer)
            {
                sprite.sortingLayerName = "Player";
            }
        }
    }
    
    private void Dash()
    {
        if (_dashTimer > 0)
        {
            _isDashing = true;
            ApplyDash();
        }
        else
        {
             if(_isDashing) _isDashing = false;
        }
    }

    private void Jump()
    {
        if (CanJump())
        {
            if (_jump) _jump = false;

            _jumpFixTimer = .2f;            
            _isJumping = true;
            _currentJumpCount--;
            
            ApplyJump();
        }
    }

    private void Move()
    {
        bool isMoving = _rb.linearVelocity.x != 0 && _isGrounded && !_isDashing;
        
        if (!_isDashing)
        {
            float s = _isRunning ? runSpeed : walkSpeed;
            if(_xInput is not 0) _lastMoveInput = _xInput;

            if(_xInput is not 0) Flip(_xInput);
                
            _rb.linearVelocity =
                new Vector2(_xInput * s, _rb.linearVelocityY);
        }

        float animationSpeedMultiplier = _isRunning ? 1.3f : 1f; // TO MAKE ANIMATION SLOW OR FAST ACC. TO RUN AND WALK 
        _animator.SetFloat("speed", animationSpeedMultiplier);
        
        _animator.SetBool("isRun", isMoving);
    }

    private void ApplyDash()
    {
        float directionX = _lastMoveInput == 0 ? 1 : _lastMoveInput;
        _rb.linearVelocity = new Vector2(directionX, 0) * dashForce;
    }
    
    private void GetInputs()
    {
        _xInput = Input.GetAxisRaw("Horizontal");
        _isRunning = Input.GetKey(KeyCode.LeftControl) && _rb.linearVelocityX != 0 && _isGrounded;

        if (Input.GetKeyDown(KeyCode.Space)) _jump = true;
        
        if (Input.GetKeyUp(KeyCode.Space)) _jump = false;

        if (Input.GetKeyDown(KeyCode.LeftShift)) _dash = true;

        if (Input.GetKeyDown(KeyCode.W) && _canHide) _hide = true;
        if (Input.GetKeyDown(KeyCode.S) || !_canHide) _hide = false;
    }

    private void Flip(float input)
    {
        if(!Mathf.Approximately(input, _facingDirection))
            ApplyFlip();
    }

    private void ApplyFlip()
    {
        _facingDirection *= -1;
        transform.localScale = new Vector3(transform.localScale.x * -1, transform.localScale.y, transform.localScale.z);
    }
    
    private void ApplyJump()
    {
        //RESETTING THE VELOCITY
        _rb.linearVelocity = new Vector2(_rb.linearVelocityX, 0);
        _rb.AddForce(Vector2.up * jumpForce , ForceMode2D.Impulse);
    }
    
    private bool CanJump()
    {
        return _jump && _currentJumpCount > 0 && !_isDashing; 
    }

    private void CanHide()
    {
        Collider2D col = Physics2D.OverlapCircle(transform.position, objRadius, LayerMask.GetMask("Object"));

        if (col is not null)
        {
            if (col.bounds.Contains(_collider.bounds.center))
            {
                _canHide = true;
                print("can Hide!");
            }
            else
            {
                _canHide = false;
                print("not completely inside object");
            }
        }
        else
        {
            _canHide = false;
            print("there is nothing to hide!");
        }

        /*
        if (_collider.IsTouchingLayers(LayerMask.GetMask("Object")))
        {
            _canHide = true;
            print("can Hide");
        }
        else
        {
            _canHide = false;
            print("cannot Hide");
        }*/
    }

    public Collider2D GetCollider() => _collider;
    
    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, objRadius);
    }
}
