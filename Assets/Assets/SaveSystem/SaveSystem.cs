using System.Collections.Generic;
using Core;
using Newtonsoft.Json;
using UnityEngine;
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

        #endregion


        public void Awake()
        {
            _saveFilePath = Application.persistentDataPath + "/save.json";
            DialogueFlags = new Dictionary<string, Dictionary<string, bool>>();
        }

        private void Start()
        {
            BankedShards = 0;
            ShardsOnPlayer = 0;

            GameManager.GetGameManager().onNewGameStarted.AddListener(CreateSave);
        }

        public bool SaveExists()
        {
            return File.Exists(_saveFilePath);
        }

        public bool DeleteSave()
        {
            if (File.Exists(_saveFilePath))
            {
                File.Delete(_saveFilePath);

                if (!File.Exists(_saveFilePath))
                {
                    return true;
                }
            }

            return false;
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
            Debug.Log(DialogueFlags.Count);
            Save save = CreateSaveGameObject();
            var json = JsonConvert.SerializeObject(save);

            File.WriteAllText(_saveFilePath, json);

            Debug.Log($"Saved to {_saveFilePath}");
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
            if (load.Flags.Count != 0)
                DialogueFlags = load.Flags;
        }

        private void OnApplicationQuit()
        {
            CreateSave();
        }

        public void SaveFlags(Dictionary<string, Dictionary<string, bool>> dic)
        {
            DialogueFlags = dic;
        }
    }
}