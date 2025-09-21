using UnityEngine;

[System.Serializable]
public class SaveData
{
    public PlayerData pData;
    
    public SaveData()
    {
        pData = new PlayerData();
    }
    
    [System.Serializable]
    public class PlayerData
    {
        public Vector3 position;
        public Vector3 rotation;

        public PlayerData()
        {
            // PASS DEFAULT VALUES
            position = Vector3.zero;
            rotation = Vector3.zero;
        }
    }    
}
