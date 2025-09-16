using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class ItemPickup : MonoBehaviour
{
    [Tooltip("InventoryItem (SO) to give")]
    public InventoryItem _item;

    [Tooltip("Override how many this pickup gives (uses InventoryItem.defaultAmount if <=0)")]
    public int _amount = 0;

    void Reset()
    {
        var col = GetComponent<Collider2D>();
        if (col != null) col.isTrigger = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        var pc = other.GetComponent<PlayerController>();
        if (pc == null) return;

        int give = _amount > 0 ? _amount : (_item != null ? _item._defaultAmount : 1);

        bool added = InventoryManager._instance != null && InventoryManager._instance.AddItem(_item, give);
        if (added)
        {
            Destroy(gameObject);
        }
        else
        {
            Debug.Log("ItemPickup: Inventory full or failed to add item.");
        }
    }
}
