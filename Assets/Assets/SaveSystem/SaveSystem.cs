using System;
using System.Collections;
using System.Collections.Generic;
using Assets.Player.Script;
using Core;
using UnityEngine;
using UnityEngine.Windows;

public class SaveSystem : MonoBehaviour
{
    private string _saveFilePath;
    #region Stuff to save

    private PlayerScript _playerScript;
    
    #endregion
    public void Awake()
    {
        _saveFilePath = Application.persistentDataPath + "/save.json";
        _playerScript = GameObject.FindWithTag("Player").GetComponent<PlayerScript>();
        GameManager.GetGenerator().onDungeonGenerated.AddListener(OnDungeonGenerated);
    }

    public void LoadSave()
    {
        if (File.Exists(_saveFilePath))
        {
            Save saveToLoad = JsonUtility.FromJson<Save>(_saveFilePath);
            LoadSaveGameObject(saveToLoad);
        }
    }

    public void CreateSave()
    {
        Save save = CreateSaveGameObject();
        string json = JsonUtility.ToJson(save);
        System.IO.File.WriteAllText(_saveFilePath, json);
        
        
        
    }

    public Save CreateSaveGameObject()
    {
        Save save = new Save();

        save._lightShards = _playerScript.GetLightShardAmount();
        save._smithProgress = 0; //TODO
        
        return save;
    }

    public void LoadSaveGameObject(Save load)
    {
        _playerScript.PlayerChangeLightShards(load._lightShards);
    }

    
    
    public void OnApplicationQuit()
    {
        CreateSave();
    }

    public void OnDungeonGenerated()
    {
        CreateSave();
    }
}
