using UnityEngine;

public class Key : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerKeys.hasKey = true;   // Player gets the key
            Debug.Log("Picked up the key!");
            Destroy(gameObject);        // Remove key from the scene
        }
    }
}
