using System;
using System.IO;
using UnityEngine;

public class DataFile
{
    private string _fileName;
    private string code = "god";
 
    public DataFile(string fileName)
    {
        _fileName = fileName;
    }
    
    public void SaveDataToFile(SaveData saveData)
    {
        Debug.Log(Application.persistentDataPath);
        
        string fullPath = Path.Combine(Application.persistentDataPath, _fileName);
        Directory.CreateDirectory(Path.GetDirectoryName(fullPath) ?? throw new InvalidOperationException());

        string data = JsonUtility.ToJson(saveData);
        string encryptData = DataEncryptDecrypt(data);
        
        using (FileStream fs = new FileStream(fullPath, FileMode.Create))
        {
            using (StreamWriter w = new StreamWriter(fs))
            {
                w.Write(encryptData);
            }
        }
    }

    public SaveData LoadDataFromFile()
    {
        string fullPath = Path.Combine(Application.persistentDataPath, _fileName);
        SaveData d = null;

        if (File.Exists(fullPath))
        {
            string data = "";

            using (FileStream fs = new FileStream(fullPath, FileMode.Open))
            {
                using (StreamReader r = new StreamReader(fs))
                {
                    data = r.ReadToEnd();
                }
            }
            
            string decryptData = DataEncryptDecrypt(data);
            d = JsonUtility.FromJson<SaveData>(decryptData);
        }
        
        return d;
    }
    
    private string DataEncryptDecrypt(string data)
    {
        string result = "";

        for (int i = 0; i < data.Length; i++)
        {
            result += (char)(data[i] ^ code[i % code.Length]);
        }

        return result;
    }
}
