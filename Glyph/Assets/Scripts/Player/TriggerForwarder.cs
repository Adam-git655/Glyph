using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class TriggerForwarder : MonoBehaviour
{
    private PlayerInteractor _owner;

    public void Initialize(PlayerInteractor owner)
    {
        _owner = owner;
        var col = GetComponent<Collider2D>();
        if (col != null && !col.isTrigger)
        {
            col.isTrigger = true;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (_owner == null) return;
        _owner.HandleTriggerEnter(other);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (_owner == null) return;
        _owner.HandleTriggerExit(other);
    }

    private void OnDestroy()
    {
        _owner = null;
    }
}
