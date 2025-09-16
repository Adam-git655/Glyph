using System.Collections.Generic;
using UnityEngine;

public class PortalScript : MonoBehaviour
{
    [HideInInspector] public Transform linkedPortal;
    private HashSet<GameObject> _justTeleported = new HashSet<GameObject>();

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (_justTeleported.Contains(other.gameObject))
            return;

        if (linkedPortal != null &&
            linkedPortal.TryGetComponent<PortalScript>(out var destScript))
        {
            destScript._justTeleported.Add(other.gameObject);
        }
        other.transform.position = linkedPortal.position;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        _justTeleported.Remove(other.gameObject);
    }
}
