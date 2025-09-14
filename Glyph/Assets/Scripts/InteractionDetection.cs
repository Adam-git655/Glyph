using Unity.VisualScripting;
using UnityEngine;

public class InteractionDetection : MonoBehaviour
{
    private IInteractable interactableInRange = null;
    public GameObject interactionText;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        interactionText.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            interactableInRange?.Interact();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out IInteractable interactable) && interactable.CanInteract())
        {
            interactableInRange = interactable;
            interactionText.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out IInteractable interactable) && interactable == interactableInRange)
        {
            interactableInRange = null;
            interactionText.SetActive(false);
        }
    }
}
