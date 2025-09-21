// ItemPickup.cs
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class ItemPickup : MonoBehaviour, IInteractable
{
    [Tooltip("InventoryItem (SO) to give")]
    [SerializeField] private InventoryItem _item;

    [Tooltip("Override how many this pickup gives (uses InventoryItem._defaultAmount if <=0)")]
    [SerializeField] private int _amount = 0;

    void Reset()
    {
        var col = GetComponent<Collider2D>();
        if (col != null) col.isTrigger = true;
    }

    public string GetDisplayName()
    {
        return _item != null ? _item._displayName : "Item";
    }

    public bool Interact(GameObject interactor)
    {
        if (_item == null)
        {
            Debug.LogWarning("ItemPickup.Interact: no InventoryItem assigned.");
            return false;
        }

        int give = _amount > 0 ? _amount : _item._defaultAmount;
        bool added = InventoryManager._instance != null && InventoryManager._instance.AddItem(_item, give);

        if (added)
        {
            Debug.Log($"ItemPickup: trying to add {_item?._displayName} (type {_item?._itemType}) ability:{_item?._ability}");
            Destroy(gameObject);
            return true;
        }
        else
        {
            Debug.Log("ItemPickup: Inventory full or failed to add item.");
            return false;
        }
    }

    public void Interact()
    {
        throw new System.NotImplementedException();
    }

    public bool CanInteract()
    {
        throw new System.NotImplementedException();
    }
}
