using System.Collections;
using UnityEngine;

public class FallingPlatform : MonoBehaviour
{
    private Rigidbody2D _rb;
    private void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            _rb.bodyType = RigidbodyType2D.Dynamic;
            Destroy(gameObject, 3f);
        }
    }
}
