using UnityEngine;

public class Key : MonoBehaviour, IInteractable
{
    public bool CanInteract()
    {
        return true;
    }

    public string GetDisplayName()
    {
        throw new System.NotImplementedException();
    }

    public void Interact()
    {
        if (!CanInteract())
            return;

        PickUpKey();
    }

    public bool Interact(GameObject interactor)
    {
        throw new System.NotImplementedException();
    }

    void PickUpKey()
    {
        PlayerKeys.hasKey = true;   
        Destroy(gameObject);        
    }
}
