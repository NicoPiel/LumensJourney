using ProceduralGeneration.Core;
using UnityEditor.U2D.Animation;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Experimental.Rendering.Universal;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;
using Utility;

namespace Core
{
    public class GameManager : MonoBehaviour
    {
        // Events
        public UnityEvent newGameStarted;
        
        private static GameManager _instance;
        private Generator _generator;
        [SerializeField]
        private GameObject player;

        private GameObject _canvas;
        private Camera _camera;

        private AudioClip _menuSound;

        private bool _ingame = false;
        private bool _paused = false;

        private void Start()
        {
            DontDestroyOnLoad(this);
            _instance = this;
            
            _menuSound = Resources.Load<AudioClip>("Audio/Clicks/click3");
            
            // Events
            newGameStarted = new UnityEvent();
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

                if (_generator != null)
                {
                    _generator.onDungeonGenerated.AddListener(() =>
                    {
                        player = GameObject.FindWithTag("Player");

                        _camera = Camera.main;
                        _canvas = GameObject.Find("PauseMenu");
                        _canvas.gameObject.SetActive(false);

                        _ingame = true;
                    });

                    _generator.Generate();
                }
            };
            
            newGameStarted?.Invoke();
        }

        private void OnPlayerLightLevelChanged()
        {
            if (_ingame)
            {
                var playerLight = player.transform.Find("PlayerLight").gameObject.GetComponent<Light2D>();
                var playerScript = player.GetComponent<PlayerScript>();
                
                playerLight.pointLightOuterRadius = 6 * playerScript.GetPlayerLightLevel() + 2;
                
                ChromaticAberration chrom;
                _camera.GetComponent<Volume>().profile.TryGet(out chrom);

                chrom.intensity.value *= playerScript.GetPlayerLightLevel();
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

        public void PlayMenuSound()
        {
            var audioSource = GetComponent<AudioSource>();
            audioSource.clip = _menuSound;
            audioSource.Play();
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