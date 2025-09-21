using System.Collections.Generic;
using UnityEngine;

public class PlayerInteractor : MonoBehaviour
{
    private readonly List<IInteractable> _nearby = new List<IInteractable>();

    private Collider2D _externalCollider;
    private TriggerForwarder _forwarder;

    private bool _usingExternal = false;

    public IInteractable CurrentInteractable
    {
        get
        {
            if (_nearby.Count == 0) return null;
            return _nearby[_nearby.Count - 1];
        }
    }

    public void SetInteractionCollider(Collider2D collider)
    {
        if (_externalCollider == collider) return; 

        if (_forwarder != null)
        {
            if (_forwarder.gameObject != null)
                Destroy(_forwarder);
            _forwarder = null;
            _externalCollider = null;
            _usingExternal = false;
        }

        if (collider == null)
        {
            _usingExternal = false;
            _externalCollider = null;
            return;
        }

        if (!collider.isTrigger)
        {
            Debug.LogWarning($"[PlayerInteractor] Assigned interaction collider '{collider.gameObject.name}' is not set to Is Trigger. Setting it to trigger automatically.");
            collider.isTrigger = true;
        }

        _forwarder = collider.gameObject.GetComponent<TriggerForwarder>();
        if (_forwarder == null)
        {
            _forwarder = collider.gameObject.AddComponent<TriggerForwarder>();
        }
        _forwarder.Initialize(this);

        _externalCollider = collider;
        _usingExternal = true;
    }

    public void HandleTriggerEnter(Collider2D other)
    {
        IInteractable interactable = FindInteractableOnCollider(other);
        if (interactable != null)
        {
            if (!_nearby.Contains(interactable))
            {
                _nearby.Add(interactable);
                Debug.Log($"[PlayerInteractor] Added interactable: {GetSourceName(interactable, other)}");
            }
        }
        else
        {
            Debug.Log($"[PlayerInteractor] No IInteractable found on '{other.gameObject.name}' or its parents/children.");
        }
    }

    public void HandleTriggerExit(Collider2D other)
    {
        IInteractable interactable = FindInteractableOnCollider(other);
        if (interactable != null)
        {
            if (_nearby.Remove(interactable))
            {
                Debug.Log($"[PlayerInteractor] Removed interactable: {GetSourceName(interactable, other)}");
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (_usingExternal) return; // ignore if external collider is used
        Debug.Log($"[PlayerInteractor] OnTriggerEnter2D (self) with '{other.gameObject.name}'");
        HandleTriggerEnter(other);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (_usingExternal) return;
        Debug.Log($"[PlayerInteractor] OnTriggerExit2D (self) with '{other.gameObject.name}'");
        HandleTriggerExit(other);
    }

    public void TryInteract()
    {
        var interactable = CurrentInteractable;
        if (interactable == null)
        {
            Debug.Log("[PlayerInteractor] No interactable in range.");
            return;
        }

        bool success = false;
        try
        {
            success = interactable.Interact(gameObject);
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"[PlayerInteractor] Exception when calling Interact(): {ex}");
        }

        Debug.Log($"[PlayerInteractor] Interact attempted on '{interactable.GetDisplayName()}' -> success: {success}");

        if (success)
        {
            _nearby.Remove(interactable);
        }
    }

    private IInteractable FindInteractableOnCollider(Collider2D other)
    {
        if (other == null) return null;

        if (other.TryGetComponent<IInteractable>(out var interactable)) return interactable;

        interactable = other.GetComponentInParent<IInteractable>();
        if (interactable != null) return interactable;

        interactable = other.GetComponentInChildren<IInteractable>();
        return interactable;
    }

    private string GetSourceName(IInteractable interactable, Collider2D other)
    {
        var comp = interactable as Component;
        return comp != null ? comp.gameObject.name : other.gameObject.name;
    }

    private void OnDestroy()
    {
        if (_forwarder != null)
        {
            if (_forwarder.gameObject != null)
                Destroy(_forwarder);
            _forwarder = null;
        }
    }
}
