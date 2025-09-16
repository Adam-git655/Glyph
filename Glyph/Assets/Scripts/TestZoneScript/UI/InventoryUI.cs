using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    [Header("UI refs")]
    public GameObject _rootPanel; 
    public Transform _slotsContainer; // parent with slot item UI elements
    public GameObject _slotPrefab; // simple prefab with Image + Text
    public TextMeshProUGUI _coinText;
    public TextMeshProUGUI _keyText;

    private List<GameObject> _slotUIs = new List<GameObject>();

    private void Awake()
    {
        if (_rootPanel != null) _rootPanel.SetActive(false);

        if (InventoryManager._instance != null)
        {
            InventoryManager._instance.OnInventoryChanged += RefreshUI;
        }

        // make some initial slot UI placeholders
        for (int i = 0; i < 20; i++)
        {
            var go = Instantiate(_slotPrefab, _slotsContainer);
            _slotUIs.Add(go);
            go.SetActive(false);
        }
    }

    private void OnDestroy()
    {
        if (InventoryManager._instance != null)
            InventoryManager._instance.OnInventoryChanged -= RefreshUI;
    }

    // call from Input action to toggle
    public void Toggle()
    {
        bool open = !(_rootPanel != null && _rootPanel.activeSelf);
        SetOpen(open);
    }

    public void SetOpen(bool open)
    {
        if (_rootPanel == null) return;
        _rootPanel.SetActive(open);
        InventoryManager._instance?.SetPaused(open);
        RefreshUI();
    }

    public void RefreshUI()
    {
        if (InventoryManager._instance == null || _rootPanel == null) return;

        _coinText.text = InventoryManager._instance.GetCoinCount().ToString();

        var keys = InventoryManager._instance.GetSlots();
        var keyList = new List<string>();
        foreach (var s in keys)
            if (s._item != null && s._item._itemType == InventoryItemType.Key)
                keyList.Add($"{s._item._displayName} x{s._count}");
        _keyText.text = keyList.Count == 0 ? "None" : string.Join("\n", keyList);

        var allSlots = InventoryManager._instance.GetSlots();
        for (int i = 0; i < _slotUIs.Count; i++)
        {
            var ui = _slotUIs[i];
            if (i < allSlots.Count)
            {
                ui.SetActive(true);
                var slot = allSlots[i];
                var img = ui.transform.Find("Icon").GetComponent<Image>();
                var txt = ui.transform.Find("Count").GetComponent<TextMeshProUGUI>();
                img.sprite = slot._item._icon;
                txt.text = slot._count > 1 ? slot._count.ToString() : "";
            }
            else ui.SetActive(false);
        }
    }
}
