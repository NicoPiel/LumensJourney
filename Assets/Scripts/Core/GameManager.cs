using System.Collections;
using Assets.Player.Script;
using Assets.PlayerUI.Scripts;
using Assets.ProceduralGeneration.Core;
using Assets.SaveSystem;
using TMPro;
using Unity.Burst;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Experimental.Rendering.Universal;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;
using Utility;

namespace Core
{
    [BurstCompile]
    public class GameManager : MonoBehaviour
    {
        // Events
        public UnityEvent onNewGameStarted;
        public UnityEvent onGameLoaded;
        public UnityEvent onPlayerSpawned;
        public UnityEvent onNewLevel;

        public int CurrentLevel { get; set; }

        private static GameManager _instance;
        [SerializeField] private Generator generator;
        [SerializeField] private GameObject player;
        [SerializeField] private SaveSystem saveSystem;
        private GameObject _canvas;
        private Camera _camera;
        

        private AudioClip _menuSound;

        private bool _ingame = false;
        private bool _paused = false;

        private void Awake()
        {
            DontDestroyOnLoad(this);
            _instance = this;

            _menuSound = UnityEngine.Resources.Load<AudioClip>("Clicks/click3");

            // Events
            onNewGameStarted = new UnityEvent();
            onNewLevel = new UnityEvent();

            CurrentLevel = 0;
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
            Setup();

            onNewGameStarted?.Invoke();
        }

        public static void LoadGame_Static()
        {
            _instance.LoadGame();
        }

        private void LoadGame()
        {
            Setup();

            saveSystem.LoadSave();
            onGameLoaded.Invoke();
        }

        private void Setup()
        {
            StartCoroutine(Methods.LoadYourSceneAsync("Hub"));
            generator.onDungeonGenerated.AddListener(OnDungeonGenerated);

            InstantiateGenerator();
            InstantiatePlayer();

            _canvas = GameObject.Find("PauseMenu");
            _canvas.gameObject.SetActive(false);
            
            SceneManager.sceneLoaded += (scene, mode) =>
            {
                if (scene.name == "Dungeon")
                {
                    player.GetComponent<PlayerScript>().onPlayerLightLevelChanged.AddListener(OnPlayerLightLevelChanged);
                }
            };
            
            _ingame = true;
        }

        private void OnPlayerLightLevelChanged()
        {
            Debug.Log("Light change event Game Manager.");

            if (!_ingame || Camera.main == null || player == null) return;

            var playerScript = player.GetComponent<PlayerScript>();

            var playerLight = player.transform.Find("PlayerLight").GetComponent<Light2D>();
            playerLight.intensity = 1;
            playerLight.pointLightOuterRadius = 6 * playerScript.GetPlayerLightLevel() + 2;

            var volume = _camera.GetComponent<Volume>();

            ChromaticAberration chromaticAberration;
            LensDistortion lensDistortion;

            volume.profile.TryGet(out chromaticAberration);
            volume.profile.TryGet(out lensDistortion);

            chromaticAberration.intensity.value = 0.5f * playerScript.GetPlayerLightLevel();
            lensDistortion.intensity.value = 0.1f;

            player.transform.Find("PlayerLight").GetComponent<LightFlickerEffect>().effectEnabled = true;
        }

        private void OnDungeonGenerated()
        {
            _camera = Camera.main;
            _canvas = transform.Find("PauseMenu").gameObject;
            _canvas.gameObject.SetActive(false);

            CurrentLevel += 1;

            var text = PlayerUiScript.GetPlayerUiScript().GetTooltip().GetComponent<TMP_Text>();
            if (text != null)
            {
                text.text = $"Level {CurrentLevel}";
                text.gameObject.SetActive(true);
            }

            StartCoroutine(FadeLevelTextInAndOut());
        }
        
        private void InstantiateGenerator()
        {
            generator.name = "DungeonGenerator";
        }

        private void InstantiatePlayer()
        {
            GameObject pauseMenu = Instantiate(UnityEngine.Resources.Load<GameObject>("PauseMenu"), new Vector3(0, 0, 0), Quaternion.identity, gameObject.transform);
            pauseMenu.name = "PauseMenu";
            //GameObject playerUi = Instantiate(UnityEngine.Resources.Load<GameObject>("PlayerUI"), new Vector3(0, 0, 0), Quaternion.identity, gameObject.transform);
            //playerUi.name = "PlayerUI";
            player = Instantiate(player, new Vector3(-2, -2, 0), Quaternion.identity, gameObject.transform);
            player.name = "Player";
            onPlayerSpawned.Invoke();
        }

        private void Pause()
        {
            Time.timeScale = 0.0f;
            _canvas = transform.Find("PauseMenu").gameObject;
            if (_canvas != null) _canvas.SetActive(true);
            _camera = Camera.main;
            if (_camera != null) _camera.GetComponent<AudioListener>().enabled = false;
            _paused = true;
        }

        private void Resume()
        {
            Time.timeScale = 1.0f;
            _canvas = transform.Find("PauseMenu").gameObject;
            if (_canvas != null) _canvas.SetActive(false);
            _camera = Camera.main;
            if (_camera != null) _camera.GetComponent<AudioListener>().enabled = true;
            _paused = false;
        }

        public void ResumeOnClick()
        {
            Time.timeScale = 1.0f;
            _canvas = transform.Find("PauseMenu").gameObject;
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

        private IEnumerator FadeLevelTextInAndOut()
        {
            var text = PlayerUiScript.GetPlayerUiScript().GetTooltip().GetComponent<TMP_Text>();

            StartCoroutine(TextFade.FadeTextToFullAlpha(3f, text));
            yield return new WaitForSeconds(3f);
            StartCoroutine(TextFade.FadeTextToZeroAlpha(2f, text));
            yield return new WaitForSeconds(2f);

            if (text != null) text.gameObject.SetActive(false);
        }

        public void Quit()
        {
            StartCoroutine(Methods.LoadYourSceneAsync("MainMenu"));
        }

        public static GameObject GetPlayer()
        {
            return _instance.player;
        }

        public static Generator GetGenerator()
        {
            return _instance.generator;
        }
        public static SaveSystem GetSaveSystem()
        {
            return _instance.saveSystem;
        }

        public static GameManager GetGameManager()
        {
            return _instance;
        }
    }
}