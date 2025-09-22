using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public Transform PointA;
    public Transform PointB;
    public float moveSpeed = 2f;

    private Animator animator;

    private Vector3 nextPosition;

    private GameObject targetPlayer;

    void Start()
    {
        animator = GetComponent<Animator>();
        nextPosition = PointB.position;
    }

    void Update()
    {
        transform.position = Vector2.MoveTowards(transform.position, new(nextPosition.x, transform.position.y), moveSpeed * Time.deltaTime);

        if (Mathf.Approximately(transform.position.x, nextPosition.x))
        {
            nextPosition = (nextPosition == PointA.position) ? PointB.position : PointA.position;
        }

        // Flip sprite based on movement direction
        if (nextPosition.x > transform.position.x && transform.localScale.x < 0)
        {
            Flip();
        }
        else if (nextPosition.x < transform.position.x && transform.localScale.x > 0)
        {
            Flip();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            targetPlayer = collision.gameObject;
            animator.SetTrigger("IsAttacking");
        }
    }

    public void OnAttackHit()
    {
        if (targetPlayer != null)
        {
            targetPlayer.GetComponent<PlayerController>().Die();
        }
    }

    void Flip()
    {
        Vector3 localScale = transform.localScale;
        localScale.x *= -1; // Invert X scale to flip
        transform.localScale = localScale;
    }
}

