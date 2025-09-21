using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D) , typeof(EnemyCollisionDetection))]
public class EnemyAI : MonoBehaviour
{
   protected enum EnemyStates { Idle , Patrol , Chase , Suspicious}

   private Rigidbody2D _rb;
   private EnemyCollisionDetection _collisionDetection;
   private Transform _playerTransform; 
   
   [SerializeField] protected EnemyStates enemyStates;
   
   [SerializeField] protected float patrolSpeed;
   [SerializeField] protected float chaseSpeed;
   [SerializeField] protected float playerCatchDistance;
   
   [SerializeField] private int facingDirection;
   
   private float _idleTimer;
   [SerializeField] private float minIdleTime;
   [SerializeField] private float maxIdleTime;
   [SerializeField] private float minSuspiciousTime, maxSuspiciousTime;
   private float _suspiciousTimer;
   private Vector3 _currentPosition;

   private GameObject _targetBall;
   
   private void Awake()
   {
      _rb = GetComponent<Rigidbody2D>();
      _collisionDetection = GetComponent<EnemyCollisionDetection>();
   }

   private void Start()
   {
      facingDirection = transform.localScale.x > 0 ? 1 : -1;
   }

   protected virtual void FixedUpdate()
   {
      EnemyMove();
   }

   private void EnemyMove()
   {
      switch (enemyStates)
      {
         case EnemyStates.Idle : Idle(); break;
         case EnemyStates.Patrol : Patrol(); break;
         case EnemyStates.Chase : Chase(); break;
         case EnemyStates.Suspicious : Suspicious(); break;
      }
   }

   private void Idle()
   {
      _idleTimer -= Time.deltaTime;
      
      if(_collisionDetection.IsBallDetected(out _targetBall)) GoToSuspiciousState();
      if (_collisionDetection.IsPlayerDetected(facingDirection, out _playerTransform)) enemyStates = EnemyStates.Chase;
      
      if (_idleTimer <= 0)
      {
         _idleTimer = 0f;
         enemyStates = EnemyStates.Patrol;
      }
   }
   
   private void Patrol()
   {
      Move(patrolSpeed);

      if(_collisionDetection.IsBallDetected(out _targetBall)) GoToSuspiciousState();
      
      if (_collisionDetection.IsGrounded() == false || _collisionDetection.IsWallDetected(facingDirection))
         GoToIdleState();

      if (_collisionDetection.IsPlayerDetected(facingDirection , out _playerTransform)) enemyStates = EnemyStates.Chase;
   }

   private void Chase()
   {
      Move(chaseSpeed);

      if(ShouldChangeDirection(_playerTransform)) ChangeDirection();
      
      if (Vector2.Distance(transform.position, _playerTransform.position) <= playerCatchDistance)
      {
         _rb.ResetVelocity();
         PlayerGetCaught();
      }

      if (_collisionDetection.IsPlayerDetected(facingDirection, out _playerTransform) == false)
      {
         GoToIdleState();
      }
   }
   
   private void Suspicious()
   {
      _suspiciousTimer -= Time.deltaTime;
      
      Vector3 targetPosition = _targetBall.transform.position;

      if(ShouldChangeDirection(_targetBall.transform)) ChangeDirection();
      
      if (Vector3.Distance(_rb.position, targetPosition) > 3f) 
      {
         Vector3 newPosition =
            Vector3.MoveTowards(_rb.position, targetPosition, 10 * Time.deltaTime);

         _rb.MovePosition(newPosition);
      }
      else
      {
         if (_suspiciousTimer <= 0)
            _suspiciousTimer = Random.Range(minSuspiciousTime, maxSuspiciousTime);
         
         if (_suspiciousTimer < .2f)
         {
            Destroy(_targetBall);
            enemyStates = EnemyStates.Patrol;
         }
      }
   }

   protected virtual void PlayerGetCaught()
   {
      // WHEN PLAYER CAUGHT
   }
   
   private void GoToIdleState()
   {
      _rb.ResetVelocity();
      ChangeDirection();
         
      _idleTimer = Random.Range(minIdleTime, maxIdleTime);
      enemyStates = EnemyStates.Idle;
   }

   private void GoToSuspiciousState()
   {
      _rb.ResetVelocity();
      enemyStates = EnemyStates.Suspicious;
   }
   
   private void ChangeDirection()
   {
      facingDirection *= -1;
      Vector3 scale = transform.localScale;
      scale.x *= -1f;
      transform.localScale = scale;
   }

   private bool ShouldChangeDirection(Transform t)
   {
      if (t.position.x > transform.position.x && facingDirection == -1) return true; 
      if (t.position.x < transform.position.x && facingDirection == 1) return true;
      
      return false;
   }
   
   private void Move(float s)
   {
      _rb.linearVelocity = facingDirection * s * Vector3.right;
   }
}
