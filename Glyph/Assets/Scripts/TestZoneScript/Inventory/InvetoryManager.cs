using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager _instance { get; private set; }

    [Header("Capacity")]
    [SerializeField] private int _maxSlots = 20;

    [Header("Starting")]
    [SerializeField] private List<InventoryItem> _startingItems = new List<InventoryItem>();

    private List<InventorySlot> _slots = new List<InventorySlot>();

    public event Action OnInventoryChanged;
    public event Action<InventoryItem, int> OnItemAdded; 
    public event Action<InventoryItem, int> OnItemRemoved; 
    public bool IsPaused { get; private set; } = false;

    private void Awake()
    {
        if (_instance != null && _instance != this) { Destroy(gameObject); return; }
        _instance = this;
        DontDestroyOnLoad(gameObject);

        foreach (var it in _startingItems)
        {
            if (it != null) AddItem(it, it._defaultAmount, false);
        }
    }

    public bool AddItem(InventoryItem item, int amount = 1, bool notify = true)
    {
        if (item == null || amount <= 0) return false;

        if (item._stackable)
        {
            var slot = _slots.FirstOrDefault(s => s._item == item);
            if (slot != null)
            {
                slot._count = Mathf.Min(item._maxStack, slot._count + amount);
                if (notify) { OnItemAdded?.Invoke(item, amount); OnInventoryChanged?.Invoke(); }
                return true;
            }
        }

        if (_slots.Count >= _maxSlots) return false;

        _slots.Add(new InventorySlot(item, Mathf.Min(amount, item._maxStack)));
        if (notify) { OnItemAdded?.Invoke(item, amount); OnInventoryChanged?.Invoke(); }
        return true;
    }

    public bool RemoveItem(InventoryItem item, int amount = 1)
    {
        if (item == null || amount <= 0) return false;

        var slot = _slots.FirstOrDefault(s => s._item == item);
        if (slot == null) return false;

        if (slot._count > amount)
        {
            slot._count -= amount;
            OnItemRemoved?.Invoke(item, amount);
            OnInventoryChanged?.Invoke();
            return true;
        }
        else
        {
            int removed = slot._count;
            _slots.Remove(slot);
            OnItemRemoved?.Invoke(item, removed);
            OnInventoryChanged?.Invoke();
            return true;
        }
    }

    public int GetCoinCount()
    {
        return _slots.Where(s => s._item._itemType == InventoryItemType.Coin).Sum(s => s._count);
    }

    public bool HasKey(string keyId)
    {
        return _slots.Any(s => s._item._itemType == InventoryItemType.Key && s._item._id == keyId && s._count > 0);
    }

    public bool ConsumeKey(string keyId)
    {
        var slot = _slots.FirstOrDefault(s => s._item._itemType == InventoryItemType.Key && s._item._id == keyId);
        if (slot == null) return false;
        RemoveItem(slot._item, 1);
        return true;
    }

    public List<Ability> GetAbilitiesFromInventory()
    {
        return _slots.Where(s => s._item._itemType == InventoryItemType.Letter && s._item._ability != null)
                     .Select(s => s._item._ability).ToList();
    }

    public IReadOnlyList<InventorySlot> GetSlots() => _slots.AsReadOnly();

    public void SetPaused(bool paused)
    {
        if (IsPaused == paused) return;
        IsPaused = paused;
        if (paused)
        {
            Time.timeScale = 0f;
        }
        else
        {
            Time.timeScale = 1f;
        }
    }
}
