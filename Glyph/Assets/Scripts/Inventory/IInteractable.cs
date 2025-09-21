using UnityEngine;

public interface IInteractable
{
    string GetDisplayName();

    bool Interact(GameObject interactor);

    void Interact();

    bool CanInteract();
}
