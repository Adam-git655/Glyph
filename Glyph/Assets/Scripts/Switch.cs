using UnityEngine;

public class Switch : MonoBehaviour, IInteractable
{
    [SerializeField] private SwitchDoor doorToOpen;
    bool isSwitchFlipped = false;
    private SpriteRenderer spriteRenderer;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.color = Color.red;
    }

    public bool CanInteract()
    {
        if (!isSwitchFlipped)
            return true;
        else
            return false;
    }

    public void Interact()
    {
        if (!CanInteract())
            return;

        if (doorToOpen != null)
        {
            isSwitchFlipped = true;
            spriteRenderer.color = Color.green;
            doorToOpen.TriggerOpen();
        }
    }
}
