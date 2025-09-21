using UnityEngine;

public interface ISaveables
{
    void Save(SaveData data);
    void Load(SaveData data);
}
