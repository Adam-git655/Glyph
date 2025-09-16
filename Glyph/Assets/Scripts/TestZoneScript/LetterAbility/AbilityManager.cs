using System;
using System.Collections.Generic;
using UnityEngine;

public class AbilityManager : MonoBehaviour
{
    [Tooltip("Drag your Ability ScriptableObjects here.")]
    [SerializeField] private List<Ability> _abilities = new List<Ability>();

    [Header("Inventory")]
    [Tooltip("Maximum number of abilities the player can hold.")]
    [SerializeField] private int _maxAbilities = 3;

    private int _currentIndex = 0;

    public event Action OnAbilitiesChanged;
    public event Action<int> OnCurrentIndexChanged;

    private void Awake()
    {
        _abilities.RemoveAll(a => a == null);
        if (_abilities.Count == 0)
            Debug.LogWarning("AbilityManager has no abilities assigned!");
        _currentIndex = Mathf.Clamp(_currentIndex, 0, Mathf.Max(0, _abilities.Count - 1));
    }

    public void CyclePrev()
    {
        if (_abilities.Count == 0) return;
        int start = _currentIndex;
        do
        {
            _currentIndex = (_currentIndex - 1 + _abilities.Count) % _abilities.Count;
        } while (_abilities[_currentIndex] == null && _currentIndex != start);
        OnCurrentIndexChanged?.Invoke(_currentIndex);
    }

    public void CycleNext()
    {
        if (_abilities.Count == 0) return;
        int start = _currentIndex;
        do
        {
            _currentIndex = (_currentIndex + 1) % _abilities.Count;
        } while (_abilities[_currentIndex] == null && _currentIndex != start);
        OnCurrentIndexChanged?.Invoke(_currentIndex);
    }

    public void UseCurrentAbility(Vector2 characterPosition, Vector2 mousePosition)
    {
        var ability = GetCurrentAbility();
        if (ability != null)
            ability.Use(characterPosition, mousePosition);
    }

    public float GetCurrentRange()
    {
        var ability = GetCurrentAbility();
        return ability != null ? ability.Range : 0f;
    }

    private Ability GetCurrentAbility()
    {
        if (_abilities == null || _abilities.Count == 0)
            return null;

        _currentIndex = Mathf.Clamp(_currentIndex, 0, _abilities.Count - 1);
        return _abilities[_currentIndex];
    }

    public bool AddAbility(Ability ability)
    {
        if (ability == null)
        {
            Debug.LogWarning("AbilityManager.AddAbility: ability is null");
            return false;
        }

        if (_abilities.Contains(ability))
        {
            Debug.Log($"AbilityManager: already have {ability.name}");
            return false;
        }

        if (_abilities.Count >= _maxAbilities)
        {
            Debug.Log("AbilityManager: ability inventory full");
            return false;
        }

        _abilities.Add(ability);
        _currentIndex = Mathf.Clamp(_currentIndex, 0, _abilities.Count - 1);
        Debug.Log($"AbilityManager: picked up {ability.name}");
        OnAbilitiesChanged?.Invoke();
        OnCurrentIndexChanged?.Invoke(_currentIndex);
        return true;
    }

    public bool RemoveAbility(Ability ability)
    {
        if (ability == null) return false;
        bool removed = _abilities.Remove(ability);
        _currentIndex = Mathf.Clamp(_currentIndex, 0, Mathf.Max(0, _abilities.Count - 1));
        OnAbilitiesChanged?.Invoke();
        OnCurrentIndexChanged?.Invoke(_currentIndex);
        return removed;
    }

    public IReadOnlyList<Ability> GetAbilities() => _abilities.AsReadOnly();

    public void SetCurrentIndex(int index)
    {
        if (_abilities.Count == 0) return;  
        _currentIndex = Mathf.Clamp(index, 0, _abilities.Count - 1);
        OnCurrentIndexChanged?.Invoke(_currentIndex);
    }

    private void OnEnable()
    {
        if (InventoryManager._instance != null)
            InventoryManager._instance.OnInventoryChanged += RebuildAbilitiesFromInventory;
    }

    private void OnDisable()
    {
        if (InventoryManager._instance != null)
            InventoryManager._instance.OnInventoryChanged -= RebuildAbilitiesFromInventory;
    }

    private void RebuildAbilitiesFromInventory()
    {
        var abilityList = InventoryManager._instance.GetAbilitiesFromInventory();
        _abilities = new List<Ability>(abilityList);
        _currentIndex = Mathf.Clamp(_currentIndex, 0, Mathf.Max(0, _abilities.Count - 1));
        OnAbilitiesChanged?.Invoke();
        OnCurrentIndexChanged?.Invoke(_currentIndex);
    }


    public int GetCurrentIndex() => _currentIndex;
}
