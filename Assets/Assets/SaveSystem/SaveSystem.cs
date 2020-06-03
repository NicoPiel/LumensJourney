using System;
using Assets.Player.Script;
using Core;
using UnityEngine;
using UnityEngine.Windows;

namespace Assets.SaveSystem
{
    public class SaveSystem : MonoBehaviour
    {
        private string _saveFilePath;
        public int BankedShards { get; set; }

        #region Stuff to save

        private PlayerScript _playerScript;

        #endregion

        public void Awake()
        {
            _saveFilePath = Application.persistentDataPath + "/save.json";
            
        }

        private void Start()
        {
            BankedShards = 0;
            GameManager.GetGenerator().onDungeonGenerated.AddListener(OnDungeonGenerated);
            GameManager.GetGameManager().onGameLoaded.AddListener(LoadSave);
            GameManager.GetGameManager().onNewGameStarted.AddListener(CreateSave);
        }

        public void LoadSave()
        {
            _playerScript = GameManager.GetPlayer().GetComponent<PlayerScript>();

            if (File.Exists(_saveFilePath))
            {
                var saveToLoad = JsonUtility.FromJson<Save>(System.IO.File.ReadAllText(_saveFilePath));
                LoadSaveGameObject(saveToLoad);
            }
        }

        public void CreateSave()
        {
            Save save = CreateSaveGameObject();
            var json = JsonUtility.ToJson(save);
            System.IO.File.WriteAllText(_saveFilePath, json);
            
            Debug.Log(json);
            
            Debug.Log($"Saved to {_saveFilePath}");
        }

        private Save CreateSaveGameObject()
        {
            _playerScript = GameManager.GetPlayer().GetComponent<PlayerScript>();

            var save = new Save
            {
                lightShard = _playerScript.GetLightShardAmount(),
                bankedLightShards = BankedShards,
                smithProgress = 0 //TODO
            };

            return save;
        }

        private void LoadSaveGameObject(Save load)
        {
            _playerScript.PlayerSetLightShards(load.lightShard);
            BankedShards = load.bankedLightShards;
        }
        
        private void OnApplicationQuit()
        {
            CreateSave();
        }

        private void OnDungeonGenerated()
        {
            CreateSave();
        }
    }
}