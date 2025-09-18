using UnityEngine;

public class LockedDoor : Door
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && PlayerKeys.hasKey)
        {
            OpenDoor();
        }
    }
}
