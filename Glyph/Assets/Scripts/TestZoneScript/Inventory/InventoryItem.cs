using UnityEngine;

public enum InventoryItemType { Coin, Key, Letter, Misc }

[CreateAssetMenu(menuName = "Inventory/Item")]
public class InventoryItem : ScriptableObject
{
    [Header("Base")]
    public string _id; 
    public string _displayName;
    public Sprite _icon;
    public InventoryItemType _itemType = InventoryItemType.Misc;

    [Header("Stacking")]
    public bool _stackable = true;
    public int _maxStack = 999;

    [Header("Letter (ability)")]
    public Ability _ability; 

    [Header("Gameplay")]
    [Tooltip("How many this pickup gives by default (useful for coins).")]
    public int _defaultAmount = 1;
}
