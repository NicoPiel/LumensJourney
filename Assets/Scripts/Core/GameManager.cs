using ProceduralGeneration.Core;
using UnityEngine;
using UnityEngine.SceneManagement;
using Utility;

namespace Core
{
    public class GameManager : MonoBehaviour
    {
        private static GameManager _instance;
        private Generator _generator;
        private Player _player;

        private GameObject _canvas;
        private Camera _camera;

        private bool _ingame = false;
        private bool _paused = false;

        private void Start()
        {
            DontDestroyOnLoad(this);
            _instance = this;
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (_ingame)
                {
                    if (_paused)
                    {
                        Resume();
                    }
                    else
                    {
                        Pause();
                    }
                }
            }
        }

        private void Pause()
        {
            Time.timeScale = 0.0f;
            _canvas.SetActive(true);
            _camera.GetComponent<AudioListener>().enabled = false;
            _paused = true;
        }
        
        private void Resume() {
            Time.timeScale = 1.0f;
            _canvas.SetActive(false);
            _camera.GetComponent<AudioListener>().enabled = true;
            _paused = false;
        }
        
        public void ResumeOnClick() {
            Time.timeScale = 1.0f;
            _canvas = GameObject.Find("PauseMenu");
            if (_canvas != null) _canvas.SetActive(false);
            _camera = Camera.main;
            if (_camera != null) _camera.GetComponent<AudioListener>().enabled = true;
            _paused = false;
        }

        public static void NewGame_Static()
        {
            _instance.NewGame();
        }

        private void NewGame()
        {
            StartCoroutine(Methods.LoadYourAsyncScene("PCGTestScene"));
            
            SceneManager.sceneLoaded += (scene, loadSceneMode) =>
            {
                _generator = GameObject.FindWithTag("Generator")?.GetComponent<Generator>();
                if (_generator != null) _generator.Generate();

                _camera = Camera.main;
                _canvas = GameObject.Find("PauseMenu");
                _canvas.gameObject.SetActive(false);

                _ingame = true;
            };
        }
        
        public void Quit()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
    }
}