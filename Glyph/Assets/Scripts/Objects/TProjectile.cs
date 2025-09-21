using UnityEngine;

public class TProjectile : MonoBehaviour
{
    public LayerMask breakableLayer;
    public float lifetime = 2f;
    public float rotationSpeed = 360f; 

    void Start()
    {
        Destroy(gameObject, lifetime); 
    }

    void Update()
    {
        transform.Rotate(0, 0, -rotationSpeed * Time.deltaTime);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (((1 << collision.gameObject.layer) & breakableLayer) != 0)
        {
            Destroy(collision.gameObject); // Break the object
        }
        Destroy(gameObject); // Destroy projectile
    }
}
