using Unity.VisualScripting;
using UnityEngine;

public static class Global
{
    public static string AssignUniqueID(GameObject obj)
    {
        return $"{obj.name}_{obj.transform.position.x}_{obj.transform.position.y}";
    }
}
