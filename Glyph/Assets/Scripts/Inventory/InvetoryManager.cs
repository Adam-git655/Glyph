using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager _instance { get; private set; }

    [Header("Capacity")]
    [SerializeField] private int _maxSlots = 20;

    [Header("Starting")]
    [SerializeField] private List<InventoryItem> _startingItems = new List<InventoryItem>();

    [Header("Debug")]
    [Tooltip("When true, dumps inventory to the Console after add/remove.")]
    [SerializeField] private bool _debugLogOnChange = true;

    private List<InventorySlot> _slots = new List<InventorySlot>();

    public event Action OnInventoryChanged;
    public event Action<InventoryItem, int> OnItemAdded;
    public event Action<InventoryItem, int> OnItemRemoved;

    public bool IsPaused { get; private set; } = false;

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this;
        DontDestroyOnLoad(gameObject);

        foreach (var it in _startingItems)
        {
            if (it != null)
                AddItem(it, it._defaultAmount, false);
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            DumpInventory();
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
                int previous = slot._count;
                slot._count = Mathf.Min(item._maxStack, slot._count + amount);
                int added = slot._count - previous;
                if (notify)
                {
                    OnItemAdded?.Invoke(item, added);
                    OnInventoryChanged?.Invoke();
                }
                if (_debugLogOnChange) DumpInventory();
                return true;
            }
        }

        if (_slots.Count >= _maxSlots) return false;

        int addAmount = Mathf.Min(amount, item._maxStack);
        _slots.Add(new InventorySlot(item, addAmount));

        if (notify)
        {
            OnItemAdded?.Invoke(item, addAmount);
            OnInventoryChanged?.Invoke();
        }

        if (_debugLogOnChange) DumpInventory();

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
            if (_debugLogOnChange) DumpInventory();
            return true;
        }
        else
        {
            int removed = slot._count;
            _slots.Remove(slot);
            OnItemRemoved?.Invoke(item, removed);
            OnInventoryChanged?.Invoke();
            if (_debugLogOnChange) DumpInventory();
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
        return _slots
            .Where(s => s._item != null && s._item._itemType == InventoryItemType.Letter && s._item._ability != null)
            .SelectMany(s =>
            {
                var list = new List<Ability>();
                for (int i = 0; i < s._count; i++) list.Add(s._item._ability);
                return list;
            })
            .ToList();
    }

    public IReadOnlyList<InventorySlot> GetSlots() => _slots.AsReadOnly();

    public void SetPaused(bool paused)
    {
        if (IsPaused == paused) return;
        IsPaused = paused;
        if (paused) Time.timeScale = 0f;
        else Time.timeScale = 1f;
    }

    public int GetMaxSlots()
    {
        return _maxSlots;
    }

    [ContextMenu("Dump Inventory")]
    public void DumpInventory()
    {
        var sb = new StringBuilder();
        sb.AppendLine($"Inventory Dump - slots {_slots.Count}/{_maxSlots}");

        if (_slots.Count == 0)
        {
            sb.AppendLine("  (empty)");
            Debug.Log(sb.ToString());
            return;
        }

        for (int i = 0; i < _slots.Count; i++)
        {
            var s = _slots[i];
            if (s == null || s._item == null)
            {
                sb.AppendLine($"  [{i}] EMPTY SLOT");
                continue;
            }

            var item = s._item;
            string abilityInfo = item._ability != null ? $"Ability: {item._ability.DisplayName}" : "Ability: (none)";
            sb.AppendLine($"  [{i}] {item._displayName} (id:{item._id}) type:{item._itemType} count:{s._count} stackable:{item._stackable} {abilityInfo}");
        }

        Debug.Log(sb.ToString());
    }
}
