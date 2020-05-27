using Resources.ProceduralGeneration.Core;
using Unity.Mathematics;
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
        public UnityEvent onNewGameStarted;
        public UnityEvent onNewLevel;


        private static GameManager _instance;
        public static Generator Generator { get; set; }

        public static GameObject Player { get; set; }

        private GameObject _canvas;
        private Camera _camera;

        private AudioClip _menuSound;

        private bool _ingame = false;
        private bool _paused = false;

        private void Start()
        {
            DontDestroyOnLoad(this);
            _instance = this;

            _menuSound = UnityEngine.Resources.Load<AudioClip>("Audio/Clicks/click3");

            // Events
            onNewGameStarted = new UnityEvent();
            onNewLevel = new UnityEvent();
        }

        private void Update()
        {
            // Pause Menu
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
            StartCoroutine(Methods.LoadYourSceneAsync("Hub"));

            SceneManager.sceneLoaded += (scene, loadSceneMode) =>
            {
                if (scene.name == "Hub")
                {
                    Generator = LoadDungeonGenerator().GetComponent<Generator>();
                    Player = InstantiatePlayer();

                    _canvas = GameObject.Find("PauseMenu");
                    _canvas.gameObject.SetActive(false);

                    Generator.onDungeonGenerated.AddListener(() =>
                    {
                        _camera = Camera.main;
                        //_canvas = GameObject.Find("PauseMenu");
                        //_canvas.gameObject.SetActive(false);

                        _ingame = true;
                    });
                }
            };

            onNewGameStarted?.Invoke();
        }

        private static GameObject LoadDungeonGenerator()
        {
            GameObject generator = GameObject.FindWithTag("Generator");

            if (generator == null)
            {
                generator = Instantiate(UnityEngine.Resources.Load<GameObject>("ProceduralGeneration/DungeonGenerator"), new Vector3(0, 0, 0), quaternion.identity);
                generator.name = "DungeonGenerator";
            }

            return generator;
        }

        private void OnPlayerLightLevelChanged()
        {
            if (_ingame && Camera.main != null && Player != null)
            {
                var playerScript = Player.GetComponent<PlayerScript>();
                
                var playerLight = Player.transform.Find("PlayerLight").GetComponent<Light2D>();
                playerLight.intensity = 1;
                playerLight.pointLightOuterRadius = 6 * playerScript.GetPlayerLightLevel() + 2;

                var volume = _camera.GetComponent<Volume>();

                ChromaticAberration chromaticAberration;
                LensDistortion lensDistortion;

                volume.profile.TryGet(out chromaticAberration);
                volume.profile.TryGet(out lensDistortion);

                chromaticAberration.intensity.value *= playerScript.GetPlayerLightLevel();
                lensDistortion.intensity.value = 0.1f;
            }
        }

        private GameObject InstantiatePlayer()
        {
            GameObject pauseMenu = Instantiate(UnityEngine.Resources.Load<GameObject>("UI/PauseMenu"), new Vector3(0, 0, 0), Quaternion.identity, gameObject.transform);
            pauseMenu.name = "PauseMenu";
            GameObject playerUi = Instantiate(UnityEngine.Resources.Load<GameObject>("PlayerUI/PlayerUI"), new Vector3(0, 0, 0), Quaternion.identity, gameObject.transform);
            playerUi.name = "PlayerUI";
            GameObject player = Instantiate(UnityEngine.Resources.Load<GameObject>("Player/Player"), new Vector3(0, 0, 0), Quaternion.identity, gameObject.transform);
            player.name = "Player";
            return player;
        }

        private void Pause()
        {
            Time.timeScale = 0.0f;
            _canvas.SetActive(true);
            _camera.GetComponent<AudioListener>().enabled = false;
            _paused = true;
        }

        private void Resume()
        {
            Time.timeScale = 1.0f;
            _canvas.SetActive(false);
            _camera.GetComponent<AudioListener>().enabled = true;
            _paused = false;
        }

        public void ResumeOnClick()
        {
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
            StartCoroutine(Methods.LoadYourSceneAsync("MainMenu"));
        }
    }
}