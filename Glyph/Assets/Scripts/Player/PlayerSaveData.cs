using UnityEngine;

public class PlayerSaveData : MonoBehaviour , ISaveables
{
    public void Save(SaveData data)
    {
        data.pData.position = transform.position;
        data.pData.rotation = transform.localEulerAngles;
    }

    public void Load(SaveData data)
    {
        transform.position = data.pData.position;
        transform.localEulerAngles = data.pData.rotation;
    }
}
