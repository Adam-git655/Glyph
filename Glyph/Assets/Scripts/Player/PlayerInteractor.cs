using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class PlayerInteractor : MonoBehaviour
{
    private readonly List<IInteractable> _nearby = new List<IInteractable>();

    public IInteractable CurrentInteractable
    {
        get
        {
            if (_nearby.Count == 0) return null;
            return _nearby[_nearby.Count - 1];
        }
    }

    public void TryInteract()
    {
        var interactable = CurrentInteractable;
        if (interactable == null) return;

        bool success = interactable.Interact(gameObject);
        if (success)
        {
            _nearby.Remove(interactable);
        }
        else
        {
            //show UI feedback here
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent(out IInteractable interactable))
        {
            _nearby.Add(interactable);
            Debug.Log($"Press E to pick up {interactable.GetDisplayName()}");
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.TryGetComponent(out IInteractable interactable))
        {
            _nearby.Remove(interactable);
        }
    }
}
