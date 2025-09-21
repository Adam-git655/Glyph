[System.Serializable]
public class InventorySlot
{
    public InventoryItem _item;
    public int _count;

    public InventorySlot(InventoryItem item, int count)
    {
        this._item = item;
        this._count = count;
    }
}
