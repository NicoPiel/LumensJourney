using System;
using ProceduralGeneration.Core;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Core
{
    public class GameManager : MonoBehaviour
    {
        private static GameManager _instance;
        private Generator _generator;
        private Player _player;

        private void Start()
        {
            DontDestroyOnLoad(this);
            _instance = this;
        }

        public static void NewGame_Static()
        {
            _instance.NewGame();
        }

        private void NewGame()
        {
            //StartCoroutine(Utility.Methods.LoadYourAsyncScene("PCGTestScene"));
            SceneManager.LoadScene("PCGTestScene");
            SceneManager.sceneLoaded += (scene, loadSceneMode) =>
            {
                _generator = GameObject.FindWithTag("Generator")?.GetComponent<Generator>();
                if (_generator != null) _generator.Generate();
            };

        }
        
        
    }
}
