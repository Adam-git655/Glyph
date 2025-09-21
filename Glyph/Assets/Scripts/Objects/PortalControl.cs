using System.Xml.Serialization;
using Unity.VisualScripting;
using UnityEngine;

public class PortalControl : MonoBehaviour
{
    public static PortalControl _Instance;

    [SerializeField] private GameObject _portalA, _portalB;
    [SerializeField] private Transform _portalASpawnPoint, _portalBSpawnPoint;
    [SerializeField] private GameObject clone;

    private Collider2D _portalACollider, _portalBCollider;

    private void Start()
    {
        _Instance = this;
        _portalACollider = _portalA.GetComponent<Collider2D>();
        _portalBCollider = _portalB.GetComponent<Collider2D>();
    }

    public void CreateClone(string locationToCreate)
    {
        if (locationToCreate == "A")
        {
            var instantiatedClone = Instantiate(clone, _portalASpawnPoint.position, Quaternion.identity);
            instantiatedClone.gameObject.name = "Clone";
        }
        else if (locationToCreate == "B")
        {
            var instantiatedClone = Instantiate(clone, _portalBSpawnPoint.position, Quaternion.identity);
            instantiatedClone.gameObject.name = "Clone";
        }
    }

    public void DisableCollider(string colliderToDisable)
    {
        if (colliderToDisable == "B")
        {
            _portalBCollider.enabled = false;
        }
        else if (colliderToDisable == "A")
        {
            _portalACollider.enabled = false;
        }
    }

    public void EnableCollider()
    {
        _portalACollider.enabled = true;
        _portalBCollider.enabled = true;
    }
}
