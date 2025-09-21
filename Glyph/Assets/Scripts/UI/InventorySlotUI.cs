// InventorySlotUI.cs
using UnityEngine;
using UnityEngine.UI;
#if TMP_PRESENT
using TMPro;
#endif

public class InventorySlotUI : MonoBehaviour
{
    [Header("UI refs (assign in prefab)")]
    [SerializeField] private Image _backgroundImage; // optional background image (root)
    [SerializeField] private Image _iconImage;       // child Icon image (required)
#if UNITY_EDITOR
    [SerializeField] private UnityEngine.UI.Text _countText; // legacy Text (optional)
    [SerializeField] private TMPro.TextMeshProUGUI _countTextTMP; // TMP (optional)
#else
    [SerializeField] private UnityEngine.UI.Text _countText;
    [SerializeField] private TMPro.TextMeshProUGUI _countTextTMP;
#endif

    /// <summary>
    /// Set icon & stack text. If item is null, will render empty state.
    /// </summary>
    public void SetItemSprite(Sprite sprite, bool isStackable, int count, Sprite emptySprite = null)
    {
        if (_iconImage == null) return;

        if (sprite != null)
        {
            _iconImage.sprite = sprite;
            _iconImage.enabled = true;
        }
        else
        {
            if (emptySprite != null)
            {
                _iconImage.sprite = emptySprite;
                _iconImage.enabled = true;
            }
            else
            {
                _iconImage.enabled = false;
            }
        }

        // Show count only for stackable items and if count > 1
        string countStr = (isStackable && count > 1) ? count.ToString() : "";
        if (_countTextTMP != null)
            _countTextTMP.text = countStr;
        if (_countText != null)
            _countText.text = countStr;
    }
}
