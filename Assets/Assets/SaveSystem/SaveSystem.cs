using System.Collections.Generic;
using Core;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Events;
#if UNITY_WINRT
using File = UnityEngine.Windows.File;
#else
using File = System.IO.File;

#endif

namespace Assets.SaveSystem
{
    public class SaveSystem : MonoBehaviour
    {
        public UnityEvent onBeforeSave;
        public UnityEvent onGameSaved;
        
        private string _saveFilePath;

        #region Stuff to save

        public int BankedShards { get; set; }
        public int ShardsOnPlayer { get; set; }
        public Dictionary<string, Dictionary<string, bool>> DialogueFlags { get; private set; }

        #endregion


        public void Awake()
        {
            _saveFilePath = Application.persistentDataPath + "/save.json";
            
            DialogueFlags = new Dictionary<string, Dictionary<string, bool>>();
            BankedShards = 0;
            ShardsOnPlayer = 0;
            
            onBeforeSave = new UnityEvent();
            onGameSaved = new UnityEvent();
        }

        private void Start()
        {
            GameManager.GetGameManager().onNewGameStarted.AddListener(CreateSave);
        }

        public bool SaveExists()
        {
            return File.Exists(_saveFilePath);
        }

        public bool DeleteSave()
        {
            if (!File.Exists(_saveFilePath)) return false;
            
            File.Delete(_saveFilePath);

            return !File.Exists(_saveFilePath);
        }

        public void LoadSave()
        {
            if (File.Exists(_saveFilePath))
            {
                var saveToLoad = JsonConvert.DeserializeObject<Save>(File.ReadAllText(_saveFilePath));
                LoadSaveGameObject(saveToLoad);
            }
        }

        public void CreateSave()
        {
            onBeforeSave.Invoke();
            
            //Debug.Log(DialogueFlags.Count);
            Save save = CreateSaveGameObject();
            var json = JsonConvert.SerializeObject(save, Formatting.Indented);

            File.WriteAllText(_saveFilePath, json);

            Debug.Log($"Saved to {_saveFilePath}");
            
            onBeforeSave.Invoke();
        }

        private Save CreateSaveGameObject()
        {
            var save = new Save
            {
                LightShard = ShardsOnPlayer,
                BankedLightShards = BankedShards,
                SmithProgress = 0,
                Flags = DialogueFlags
            };

            return save;
        }

        private void LoadSaveGameObject(Save load)
        {
            ShardsOnPlayer = load.LightShard;
            BankedShards = load.BankedLightShards;
            if (load.Flags?.Count != 0)
                DialogueFlags = load.Flags;
        }

        private void OnApplicationQuit()
        {
            CreateSave();
        }

        public void SaveFlags(Dictionary<string, Dictionary<string, bool>> dict)
        {
            DialogueFlags = dict;
        }
    }
}