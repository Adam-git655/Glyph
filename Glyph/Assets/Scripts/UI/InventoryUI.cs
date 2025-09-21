// InventoryUI.cs
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    [Header("Wiring")]
    [Tooltip("Slot prefab (must have InventorySlotUI)")]
    [SerializeField] private GameObject _slotPrefab;

    [Tooltip("Parent RectTransform (the panel with VerticalLayoutGroup)")]
    [SerializeField] private RectTransform _parent;

    [Tooltip("Optional sprite to show for empty slots")]
    [SerializeField] private Sprite _emptySprite;

    [Tooltip("If true, show a fixed number of slots (inventory capacity). If false, show only current items.)")]
    [SerializeField] private bool _showFixedCount = true;

    // how many fixed slots to show (if <=0 will use InventoryManager.GetMaxSlots())
    [Tooltip("Optional override for number of slots to show when Show Fixed Count is true. Use 0 to use InventoryManager.GetMaxSlots().")]
    [SerializeField] private int _fixedSlotCountOverride = 0;

    private readonly List<InventorySlotUI> _slotUIs = new List<InventorySlotUI>();
    private bool _subscribed = false;

    private void Awake()
    {
        if (_slotPrefab == null || _parent == null)
        {
            Debug.LogError("InventoryUI: Slot Prefab or Parent is not assigned in the inspector. Disabling InventoryUI.");
            enabled = false;
            return;
        }
    }

    private void OnEnable()
    {
        TrySubscribe();
    }

    private void Start()
    {
        TrySubscribe();
    }

    private void OnDisable()
    {
        if (_subscribed && InventoryManager._instance != null)
        {
            InventoryManager._instance.OnInventoryChanged -= RebuildUI;
            _subscribed = false;
            Debug.Log("InventoryUI: unsubscribed from InventoryManager.");
        }
    }

    private void TrySubscribe()
    {
        if (_subscribed) return;

        if (InventoryManager._instance == null)
        {
            Debug.Log("InventoryUI: InventoryManager._instance is null. Will retry subscription (Start will retry).");
            return;
        }

        InventoryManager._instance.OnInventoryChanged -= RebuildUI;
        InventoryManager._instance.OnInventoryChanged += RebuildUI;
        _subscribed = true;
        Debug.Log("InventoryUI: subscribed to InventoryManager.OnInventoryChanged");

        RebuildUI();
    }

    private void RebuildUI()
    {
        if (InventoryManager._instance == null)
        {
            Debug.LogWarning("InventoryUI.RebuildUI called but InventoryManager._instance is null.");
            ClearSlots();
            return;
        }

        var slots = InventoryManager._instance.GetSlots();
        if (slots == null)
        {
            Debug.LogWarning("InventoryUI.RebuildUI: InventoryManager returned null slots.");
            ClearSlots();
            return;
        }

        int desired;
        if (_showFixedCount)
        {
            if (_fixedSlotCountOverride > 0)
                desired = _fixedSlotCountOverride;
            else
                desired = InventoryManager._instance != null ? InventoryManager._instance.GetMaxSlots() : slots.Count;
        }
        else
        {
            desired = slots.Count;
        }

        // ensure we have exactly `desired` slot UI elements
        EnsureSlotCount(desired);

        Debug.Log($"InventoryUI: Rebuilding UI. Inventory slots: {slots.Count}, UI slots: {_slotUIs.Count}");

        // Fill UI slots using the inventory list. If there are fewer actual items than UI slots,
        // the remaining slots are shown as empty.
        for (int i = 0; i < _slotUIs.Count; i++)
        {
            InventorySlotUI ui = _slotUIs[i];
            if (i < slots.Count)
            {
                var s = slots[i];
                if (s != null && s._item != null)
                {
                    ui.SetItemSprite(s._item._icon, s._item._stackable, s._count, _emptySprite);
                }
                else
                {
                    ui.SetItemSprite(_emptySprite, false, 0, _emptySprite);
                }
            }
            else
            {
                // no item in this slot index -> show empty
                ui.SetItemSprite(_emptySprite, false, 0, _emptySprite);
            }
        }
    }

    private void EnsureSlotCount(int desired)
    {
        // expand pool
        while (_slotUIs.Count < desired)
        {
            GameObject go = Instantiate(_slotPrefab, _parent);
            go.transform.localScale = Vector3.one;
            InventorySlotUI slotUi = go.GetComponent<InventorySlotUI>();
            if (slotUi == null)
            {
                Debug.LogError("InventoryUI: slot prefab missing InventorySlotUI component! Destroying instance.");
                Destroy(go);
                break;
            }
            _slotUIs.Add(slotUi);
        }

        // shrink pool if needed
        while (_slotUIs.Count > desired)
        {
            int last = _slotUIs.Count - 1;
            InventorySlotUI rem = _slotUIs[last];
            _slotUIs.RemoveAt(last);
            if (rem != null)
                Destroy(rem.gameObject);
        }
    }

    private void ClearSlots()
    {
        for (int i = 0; i < _slotUIs.Count; i++)
            if (_slotUIs[i] != null) Destroy(_slotUIs[i].gameObject);
        _slotUIs.Clear();
    }

#if UNITY_EDITOR
    [ContextMenu("Rebuild UI (Editor)")]
    private void RebuildUIEditor()
    {
        RebuildUI();
    }
#endif
}
