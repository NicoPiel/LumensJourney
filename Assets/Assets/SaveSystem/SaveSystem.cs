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
        
        
        
        private string _saveFilePath;

        #region Stuff to save

        public int BankedShards { get; set; }
        public int ShardsOnPlayer { get; set; }
        public Dictionary<string, Dictionary<string, bool>> DialogueFlags { get; private set; }
        public int StoryStoneProgression { get; set; }
        public int DiaryProgression { get; set; }
        public int RunsCompleted { get; set; }

        #endregion


        public void Awake()
        {
            _saveFilePath = Application.persistentDataPath + "/save.json";
            DialogueFlags = new Dictionary<string, Dictionary<string, bool>>();
            BankedShards = 0;
            ShardsOnPlayer = 0;
            StoryStoneProgression = 1;
            DiaryProgression = 1;
            RunsCompleted = 0;
        }

        private void Start()
        {
            GameManager.GetEventHandler().onNewGameStarted.AddListener(CreateSave);
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
            GameManager.GetEventHandler().onBeforeSave.Invoke();
            
            //Debug.Log(DialogueFlags.Count);
            Save save = CreateSaveGameObject();
            var json = JsonConvert.SerializeObject(save, Formatting.Indented);

            File.WriteAllText(_saveFilePath, json);

            Debug.Log($"Saved to {_saveFilePath}");
            
            GameManager.GetEventHandler().onGameSaved.Invoke();
        }

        private Save CreateSaveGameObject()
        {
            var save = new Save
            {
                LightShardSave = ShardsOnPlayer,
                BankedLightShardsSave = BankedShards,
                FlagsSave = DialogueFlags,
                StoryStoneProgressionSave = this.StoryStoneProgression,
                DiaryProgressionSave = this.DiaryProgression,
                RunsCompletedSave = this.RunsCompleted
            };

            return save;
        }

        private void LoadSaveGameObject(Save load)
        {
            ShardsOnPlayer = load.LightShardSave;
            BankedShards = load.BankedLightShardsSave;
            if (load.FlagsSave?.Count != 0)
                DialogueFlags = load.FlagsSave;
            StoryStoneProgression = load.StoryStoneProgressionSave;
            DiaryProgression = load.DiaryProgressionSave;
            RunsCompleted = load.RunsCompletedSave;
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