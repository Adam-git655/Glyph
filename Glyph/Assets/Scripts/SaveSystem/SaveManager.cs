using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance { get; private set; }

    [SerializeField] private string fileName;
    private DataFile _dataFile;
    
    private List<ISaveables> _saveables;
    private SaveData _data;

    
    private void Awake()
    {
        Instance = this;
        
        _dataFile = new DataFile(fileName);
    }
    
    private void Start()
    {
       GetISaveables();
       
       if(_data == null) NewGame();
    }

    private void Update() // REMOVE THIS LATER AND CALL THIS METHODS AT RIGHT PLACE LIKE CHECKPOINTS
    {
        if(Input.GetKeyDown(KeyCode.P)) Save();
        if(Input.GetKeyDown(KeyCode.O)) Load();
    }

    public void NewGame()
    {
        print("New Game!");
        
        _data = new SaveData();
    }
    
    public void Save()
    {
        print("Save Game!");
        
        foreach (var s in _saveables)
        {
            s.Save(_data);
        }
        
        _dataFile.SaveDataToFile(_data);
    }

    public void Load()
    {
        print("Load Game!");
        
        _data = _dataFile.LoadDataFromFile();

        if (_data == null) return;
        
        foreach (var s in _saveables)
        {
            s.Load(_data);
        }
    }

    private void GetISaveables()
    {
        // CALL THIS WHENEVER CHANGE SCENE JUST SO GET ALL SAVEABLES IN CURRENT SCENE
        
        _saveables = new List<ISaveables>();

        List<ISaveables> saveables = FindObjectsByType<MonoBehaviour>(FindObjectsInactive.Exclude,FindObjectsSortMode.None).
                OfType<ISaveables>().ToList();

        print("count : " + saveables.Count);
        foreach (var s in saveables)
        {
            _saveables.Add(s);
        }
        
        print("saveablesLength : " + _saveables.Count);
    }
}
