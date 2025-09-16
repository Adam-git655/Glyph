using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class AbilityPickup : MonoBehaviour
{
    [Tooltip("The Ability ScriptableObject that this pickup gives.")]
    public Ability _ability;

    [Tooltip("Optional display name for UI / debug")]
    public string _displayName;

    void Reset()
    {
        var c = GetComponent<Collider2D>();
        if (c != null) c.isTrigger = true;
    }

    public Ability GetAbility()
    {
        return _ability;
    }

    public string GetDisplayName()
    {
        if (!string.IsNullOrEmpty(_displayName)) return _displayName;
        return _ability != null ? _ability.name : "Unknown Ability";
    }
}
