using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class EnemyCollisionDetection : MonoBehaviour
{
    private BoxCollider2D _collider;
    private PlayerController _playerController;    
    
    [SerializeField] private Transform groundCheckPoint;
    [SerializeField] private Transform wallCheckPoint;
    [SerializeField] private float ballDetectionRadius;
    [SerializeField] private float playerDetectionRange;
    [SerializeField] private float wallDetectionRange;
    [SerializeField] private float groundDetectionRange;
    [SerializeField] private LayerMask everyThing;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private LayerMask ballLayer;
    
    private Vector2 _boxCastSize;
   
    private bool _isPlayerDetected;
    private bool _isWallDetected;
    private bool _isGrounded;

    private int _direction;

    public float playerDistance;

    private void Awake()
    {
        _collider = GetComponent<BoxCollider2D>();
        _playerController = FindFirstObjectByType<PlayerController>(FindObjectsInactive.Exclude);
        
        _boxCastSize = new Vector2(2 , 2f);
    }

    public bool IsWallDetected(int facingDirection)
    {
        _direction = facingDirection;
        
        RaycastHit2D wallHit = Physics2D.Raycast(wallCheckPoint.position, Vector2.right * facingDirection,
            wallDetectionRange,groundLayer);

        return wallHit.collider is not null;
    }

    public bool IsGrounded()
    {
        RaycastHit2D groundHit = Physics2D.Raycast(groundCheckPoint.position, Vector2.down, groundDetectionRange
            , groundLayer);

        return groundHit.collider is not null;
    }

    public bool IsBallDetected(out GameObject ball)
    {
        ball = null;
        
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, ballDetectionRadius,ballLayer);

        foreach (var col in colliders)
        {
            if (ball is not null) continue;
            ball = col.gameObject;
        }
        
        return colliders.Length > 0;
    }
    
    public bool IsPlayerDetected(int facingDirection , out Transform player)
    {
        player = null;
        float detectionRange = IsFacingSameDirection(facingDirection)? 
                playerDetectionRange : playerDetectionRange / 2f;
        
        playerDistance = Vector2.Distance(transform.position, _playerController.transform.position);
        if (IsBlockingPath(playerDistance)) return false;
        
        if (playerDistance > detectionRange) return false;

        player = _playerController.transform;
        return true;
    }

    private bool IsBlockingPath(float playerDistance)
    {
        /*RaycastHit2D[] rightHits =
            Physics2D.RaycastAll(transform.position, Vector2.right , playerDetectionRange);
        
        RaycastHit2D[] leftHits =
            Physics2D.RaycastAll(transform.position, Vector2.left , playerDetectionRange);*/

        int everything = ~0 ;
        
        RaycastHit2D[] rightHits =
            Physics2D.BoxCastAll(transform.position, _boxCastSize, 0f, Vector2.right, 
                playerDetectionRange , everyThing);
        
        RaycastHit2D[] leftHits = 
            Physics2D.BoxCastAll(transform.position, _boxCastSize, 0f, Vector2.left,playerDetectionRange,
            everyThing);
        
        RaycastHit2D[] hits = _playerController.transform.position.x > transform.position.x ? rightHits : leftHits;

        float minObsDistance = Mathf.Infinity;
        
        foreach (var h in hits)
        {
            if (h.collider.gameObject == this.gameObject) continue;
            if (h.collider.CompareTag("Player")) continue;
            
            float distance = Vector3.Distance(transform.position, h.transform.position);
            if (distance < minObsDistance) minObsDistance = distance;
        }

        if (playerDistance > minObsDistance) return true;
        return false;
    }

    private bool IsFacingSameDirection(int facingDirection)
    {
        if (_playerController.transform.position.x > transform.position.x && facingDirection == 1) return true;
        if (_playerController.transform.position.x < transform.position.x && facingDirection == -1) return true;

        return false;
    }
    
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(groundCheckPoint.position, Vector3.down * groundDetectionRange);

        if (_direction == 0) _direction = 1;
        Gizmos.DrawRay(wallCheckPoint.position, Vector3.right * _direction * wallDetectionRange);

        Gizmos.DrawWireSphere(transform.position, ballDetectionRadius);
    }
}
