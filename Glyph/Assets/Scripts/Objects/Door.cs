using UnityEngine;

public abstract class Door : MonoBehaviour
{
    [SerializeField] private BoxCollider2D doorCollider;
    [SerializeField] private Vector3 openOffset = new (0, -10f, 0); // move door up by 3 units
    [SerializeField] private float openSpeed = 3f;

    private Vector3 closedPosition;
    private Vector3 targetPosition;
    private bool isOpening = false;

    protected virtual void Start()
    {
        closedPosition = transform.position;
        targetPosition = closedPosition;
    }

    protected virtual void Update()
    {
        if (isOpening)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, openSpeed * Time.deltaTime);
        }
    }

    protected void OpenDoor()
    {
        isOpening = true;
        doorCollider.enabled = false;
        targetPosition = closedPosition + openOffset;
    }
}