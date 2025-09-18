/*using UnityEngine;

public class Door : MonoBehaviour
{
    public bool isOpen = false;

    void OnTriggerEnter2D(Collider2D other)
    {
        Animator animator = GetComponent<Animator>();
        if (other.CompareTag("Player") && PlayerKeys.hasKey)
        {
            animator.SetTrigger("open");
            isOpen = true;
            Debug.Log("Door Opened!");
            
            // Example: Disable door sprite & collider
            GetComponent<SpriteRenderer>().enabled = false;
            GetComponent<Collider2D>().enabled = false;
        }
    }
}*/
using UnityEngine;

public class Door : MonoBehaviour
{
    private Animator animator;
    private Collider2D doorCollider;
    private bool opened = false;

    void Start()
    {
        animator = GetComponent<Animator>();
        doorCollider = GetComponent<Collider2D>();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (opened) return; // only open once
        if (!other.CompareTag("Player")) return;

        if (PlayerKeys.hasKey)
        {
            opened = true;
            PlayerKeys.hasKey = false;   // consume key if you want
            animator.SetTrigger("Open"); // play open animation
            Debug.Log("Door opening...");

            // disable collider after animation (optional fallback if you don’t use animation events)
            float animLength = animator.GetCurrentAnimatorStateInfo(0).length;
            Invoke(nameof(DisableCollider), animLength);
        }
        else
        {
            Debug.Log("Door locked. Find the key.");
        }
    }

    void DisableCollider()
    {
        if (doorCollider != null)
        {
            doorCollider.enabled = false;
            Debug.Log("Door collider disabled, player can pass.");
        }
    }
}


