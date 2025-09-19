using UnityEngine;

public class Key : MonoBehaviour, IInteractable
{
    public bool CanInteract()
    {
        return true;
    }

    public void Interact()
    {
        if (!CanInteract())
            return;

        PickUpKey();
    }

    void PickUpKey()
    {
        PlayerKeys.hasKey = true;   // Player gets the key
        Destroy(gameObject);        // Remove key from the scene
    }
}
