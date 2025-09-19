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
            if (interactableInRange is Obstacle pushableObstacle)
            {
                if (pushableObstacle.IsGrabbed())
                    interactionText.SetActive(true);
                else
                    interactionText.SetActive(false);
            }

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
            if (interactable is Obstacle pushableObstacle && pushableObstacle.IsGrabbed())
                return;

            interactableInRange = null;
            interactionText.SetActive(false);
        }
    }
}
