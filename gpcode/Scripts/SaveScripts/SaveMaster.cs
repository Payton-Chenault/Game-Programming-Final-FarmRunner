using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using GlobalConstantValues;

[System.Serializable]
[AddComponentMenu("Masters/Save Master")]
public class SaveMaster : MonoBehaviour
{
    #region Variable Declaration
    private GameMaster _gameMaster;
    [SerializeField][HideInInspector] private int _easyModeHighScore = 0;
    [SerializeField][HideInInspector] private int _mediumModeHighScore = 0;
    [SerializeField][HideInInspector] private int _hardModeHighScore = 0;
    [SerializeField][HideInInspector] private int _insaneModeHighScore = 0;
    [SerializeField][HideInInspector] private bool _easyModeUnlocked = true;
    [SerializeField][HideInInspector] private bool _mediumModeUnlocked = false;
    [SerializeField][HideInInspector] private bool _hardModeUnlocked = false;
    [SerializeField][HideInInspector] private bool _insaneModeUnlocked = false;
    private string _saveData;
    private string _saveDataPath;
    #endregion
    
    #region Save Initialization
    void Start() 
    {
        _gameMaster = GlobalMasterCreationReadonly.GameMaster;
        _saveDataPath = GlobalSaveMasterReadonly.SAVE_DATA_PATH;
        CheckForSaveFile();
        Debug.Log(_saveDataPath);
        LoadGameFromJSON();
    }

    public void CheckForSaveFile() 
    {
        if(!System.IO.File.Exists(_saveDataPath))
        {
            System.IO.Directory.CreateDirectory(GlobalSaveMasterReadonly.SAVE_DATA_DIRECTORY);
            System.IO.File.Create(GlobalSaveMasterReadonly.SAVE_DATA_DIRECTORY + GlobalSaveMasterReadonly.SAVE_DATA_FILE).Close();
            SaveGameToJSON();
        }
    }

    public void LoadGameFromJSON() 
    {
        try 
        {
            string[] dataToLoad = System.IO.File.ReadAllLines(_saveDataPath);
            JsonUtility.FromJsonOverwrite(dataToLoad[0], this);
        }
        catch (FileNotFoundException e) 
        {
            Debug.LogError($"_saveData was not successfully loaded, as it couldnt find the save file\nException Dump: {e}");
        }
    }
    
    public void SaveGameToJSON() 
    {
        try 
        {
            _saveData = JsonUtility.ToJson(this);
            System.IO.File.WriteAllText(_saveDataPath, _saveData);
        }
        catch (FileNotFoundException e) 
        {
            Debug.LogError($"_saveData was not successfully saved, as it couldnt find the save file\nException Dump: {e}");
        }
    }
    #endregion

    #region Updating Methods
    public void UpdateHighScore(int diff, int score) 
    {
        switch(diff) 
        {
            case 1: if(score > _easyModeHighScore) _easyModeHighScore = score; break;
            case 2: if(score > _mediumModeHighScore) _mediumModeHighScore = score; break;
            case 3: if(score > _hardModeHighScore) _hardModeHighScore = score; break;
            case 4: if(score > _insaneModeHighScore) _insaneModeHighScore = score; break;
            default: throw new ApplicationException("Difficulity Not Valid In UpdateHighScore()");
        }

    }

    public void UpdateGameCompletion(int diffBeat) {
        switch(diffBeat) {
            case 1: _mediumModeUnlocked = true; break;
            case 2: _hardModeUnlocked = true; break; 
            case 3: _insaneModeUnlocked = true; break;
            case 4: break;
            default: throw new ApplicationException("Invalid Difficulty in UpdateGameCompletion()");
        }
    }

    void Update() {
        UpdateHighScore(_gameMaster.GetDifficulty(), _gameMaster.GetScore());
        SaveGameToJSON();
    }
    #endregion

    public bool GetGameCompletion(int diffBeat) =>
        diffBeat switch {
            1 => _easyModeUnlocked, 
            2 => _mediumModeUnlocked,
            3 => _hardModeUnlocked,
            4 => _insaneModeUnlocked,
            _ => throw new ApplicationException("Invalid Difficulty In GetGameCompletion()")
        };

    public int GetHighscore(int difficulty) =>
        difficulty switch {
            1 => _easyModeHighScore,
            2 => _mediumModeHighScore,
            3 => _hardModeHighScore,
            4 => _insaneModeHighScore,
            _ => throw new ApplicationException("Invalid Difficulty in GetHighScore()")
        };

    public void ResetData() {
        (_easyModeHighScore, _mediumModeHighScore, _hardModeHighScore, _insaneModeHighScore) = (0,0,0,0);
        (_easyModeUnlocked, _mediumModeUnlocked, _hardModeUnlocked, _insaneModeUnlocked) = (true, false, false, false);
        SaveGameToJSON();
        Debug.LogWarning($"Player Data Reset At: {_saveDataPath}");
    }


}