using UnityEngine;

public abstract class Ability : ScriptableObject
{
    [Header("UI")]
    public Sprite Icon;
    public string DisplayName = "Ability";
    
    public float Range;
    public abstract void Use(Vector2 characterPosition, Vector2 mousePosition);
}