using System.Collections;
using System.Security.Cryptography;
using Assets.MenuManager.Scripts;
using Assets.Player.Script;
using Assets.ProceduralGeneration.Core;
using Assets.SaveSystem;
using Assets.UI.PlayerUI.Scripts;
using DialogueSystem.Scripts;
using TMPro;
using Unity.Burst;
using UnityEditor;
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
        // Singleton instance of the game manager
        private static GameManager _instance;
        
        #region Events

        // Events
        public UnityEvent onNewGameStarted;
        public UnityEvent onGameLoaded;
        public UnityEvent onPlayerSpawned;
        public UnityEvent onNewLevel;

        #endregion

        #region Inspector variables

        [SerializeField] private Generator generator;
        [SerializeField] private GameObject player;
        [SerializeField] private PlayerScript playerScript;
        [SerializeField] private Camera mainCamera;
        [SerializeField] private SaveSystem saveSystem;
        [SerializeField] private MenuManagerScript menuManagerScript;
        [SerializeField] private DialogueManager dialogueManager;

        #endregion

        #region Public variables

        public int CurrentLevel { get; set; }

        #endregion

        #region Private variables

        private GameObject _canvas;
        private Camera _camera;


        private AudioClip _menuSound;

        private bool _ingame = false;
        private bool _paused = false;

        #endregion

        #region UnityEvent functions

        /// <summary>
        /// Setup events here.
        /// </summary>
        private void Awake()
        {
            DontDestroyOnLoad(this);
            _instance = this;

            _menuSound = Resources.Load<AudioClip>("Clicks/click3");

            // Events
            onNewGameStarted = new UnityEvent();
            onNewLevel = new UnityEvent();

            CurrentLevel = 0;
        }

        /// <summary>
        /// Add event listeners here.
        /// </summary>
        private void Start()
        {
            menuManagerScript = GetComponentInChildren<MenuManagerScript>();
            dialogueManager = GetComponent<DialogueManager>();
            
            onPlayerSpawned.AddListener(OnPlayerSpawned);
        }
        
        /// <summary>
        /// Use this to listen for global input.
        /// </summary>
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

        #endregion

        #region Static instance methods

        public static void LoadNewGameCutscene_Static()
        {
            _instance.LoadNewGameCutscene();
        }

        /// <summary>
        /// Uses the GameManager's singleton to start a new game.
        /// <see cref="StartNewGame"/>
        /// </summary>
        public static void NewGame_Static()
        {
            _instance.StartNewGame();
        }
        
        /// <summary>
        /// Uses the GameManager's singleton to load a game from file.
        /// <see cref="LoadGame"/>
        /// </summary>
        public static void LoadGame_Static()
        {
            _instance.LoadGame();
        }
        
        #endregion

        #region Game setup

        private void LoadNewGameCutscene()
        {
            StartCoroutine(Methods.LoadYourSceneAsync("NewGameCutscene"));
        }

        /// <summary>
        /// Starts a new game and invokes the onNewGameStarted event.
        /// </summary>
        private void StartNewGame()
        {
            Setup();

            onNewGameStarted?.Invoke();
        }

        /// <summary>
        /// Loads a game from file using the <see cref="saveSystem"/>.
        /// </summary>
        private void LoadGame()
        {
            Setup();

            saveSystem.LoadSave();
            onGameLoaded.Invoke();
        }

        /// <summary>
        /// Loads the Hub scene, then proceeds to instantiate the player.
        /// Also adds listeners for events that are dungeon-specific.
        /// </summary>
        private void Setup()
        {
            StartCoroutine(Methods.LoadYourSceneAsync("Hub"));
            
            ModifyGenerator();
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
        
        /// <summary>
        /// Modifies the generator's properties and adds listeners.
        /// </summary>
        private void ModifyGenerator()
        {
            if (generator == null) return;
            generator.name = "DungeonGenerator";
            generator.onDungeonGenerated.AddListener(OnDungeonGenerated);
        }
        
        /// <summary>
        /// Instantiates player and related objects, such as some UI elements.
        /// Also finds and sets the <see cref="playerScript"/>
        /// </summary>
        private void InstantiatePlayer()
        {
            GameObject pauseMenu = Instantiate(Resources.Load<GameObject>("PauseMenu"), new Vector3(0, 0, 0), Quaternion.identity, gameObject.transform);
            pauseMenu.name = "PauseMenu";
            player = Instantiate(player, new Vector3(-2, -2, 0), Quaternion.identity, gameObject.transform);
            player.name = "Player";
            if (player != null) _camera = player.transform.Find("MainCamera").GetComponent<Camera>();
            if (player != null) playerScript = player.GetComponent<PlayerScript>();
            onPlayerSpawned.Invoke();
        }

        #endregion

        #region Event subscriptions

        /// <summary>
        /// Does work whenever the player loses or gains light.
        /// </summary>
        private void OnPlayerLightLevelChanged()
        {
            if (!_ingame || Camera.main == null || player == null) return;

            var playerLight = player.transform.Find("PlayerLight").GetComponent<Light2D>();
            playerLight.intensity = 1;
            playerLight.pointLightOuterRadius = 6 * playerScript.GetPlayerLightLevel() + 2;

            var volume = _camera.GetComponent<Volume>();

            ChromaticAberration chromaticAberration;
            LensDistortion lensDistortion;

            volume.profile.TryGet(out chromaticAberration);
            volume.profile.TryGet(out lensDistortion);

            chromaticAberration.intensity.value = 0.7f * playerScript.GetPlayerLightLevel();
            lensDistortion.intensity.value = 0.1f;

            player.transform.Find("PlayerLight").GetComponent<LightFlickerEffect>().effectEnabled = true;
        }

        /// <summary>
        /// Does work whenever a new level is generated.
        /// </summary>
        private void OnDungeonGenerated()
        {
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

        /// <summary>
        /// Does work when the player is spawned.
        /// </summary>
        private void OnPlayerSpawned()
        {
            _camera = GetPlayer().transform.Find("MainCamera").GetComponent<Camera>();
        }

        #endregion

        #region Pause menu

        /// <summary>
        /// Pauses the game and opens the pause menu.
        /// </summary>
        private void Pause()
        {
            Time.timeScale = 0.0f;
            _canvas = transform.Find("PauseMenu").gameObject;
            if (_canvas != null)
            {
                _canvas.SetActive(true);
                GetPlayer().transform.Find("PlayerUI").gameObject.SetActive(false);
            }
            if (_camera != null) _camera.GetComponent<AudioListener>().enabled = false;
            _paused = true;
        }

        /// <summary>
        /// Resumes the game and closes the pause menu.
        /// </summary>
        private void Resume()
        {
            Time.timeScale = 1.0f;
            _canvas = transform.Find("PauseMenu").gameObject;
            if (_canvas != null)
            {
                _canvas.SetActive(false);
                GetPlayer().transform.Find("PlayerUI").gameObject.SetActive(true);
            }
            if (_camera != null) _camera.GetComponent<AudioListener>().enabled = true;
            _paused = false;
        }

        /// <summary>
        /// Resumes the game when a button is clicked and closes the pause menu.
        /// </summary>
        public void ResumeOnClick()
        {
            Time.timeScale = 1.0f;
            _canvas = transform.Find("PauseMenu").gameObject;
            if (_canvas != null)
            {
                _canvas.SetActive(false);
                GetPlayer().transform.Find("PlayerUI").gameObject.SetActive(true);
            }
            if (_camera != null) _camera.GetComponent<AudioListener>().enabled = true;
            _paused = false;
        }

        #endregion

        #region Global sounds

        /// <summary>
        /// Plays a menu sound.
        /// Use this as an event function.
        /// </summary>
        public void PlayMenuSound()
        {
            var audioSource = GetComponent<AudioSource>();
            audioSource.clip = _menuSound;
            audioSource.Play();
        }

        #endregion
        
        #region Utility

        /// <summary>
        /// Uses an alpha effect to fade text (Level 1, 2, 3, etc) in and out.
        /// </summary>
        /// <returns></returns>
        private IEnumerator FadeLevelTextInAndOut()
        {
            var text = PlayerUiScript.GetPlayerUiScript().GetTooltip().GetComponent<TMP_Text>();

            StartCoroutine(TextFade.FadeTextToFullAlpha(3f, text));
            yield return new WaitForSeconds(3f);
            StartCoroutine(TextFade.FadeTextToZeroAlpha(2f, text));
            yield return new WaitForSeconds(2f);

            if (text != null) text.gameObject.SetActive(false);
        }

        #endregion

        #region Getters

        public static GameObject GetPlayer()
        {
            return _instance.player;
        }

        public static PlayerScript GetPlayerScript()
        {
            return _instance.playerScript;
        }

        public static Generator GetGenerator()
        {
            return _instance.generator;
        }
        
        public static SaveSystem GetSaveSystem()
        {
            return _instance.saveSystem;
        }

        public static MenuManagerScript GetMenuManagerScript()
        {
            return _instance.menuManagerScript;
        }

        public static DialogueManager GetDialogueManager()
        {
            return _instance.dialogueManager;
        }
        
        public static Camera GetMainCamera()
        {
            return _instance._camera;
        }

        public static GameManager GetGameManager()
        {
            return _instance;
        }

        #endregion

        #region Quit

        /// <summary>
        /// Quits the application.
        /// If in editor, ends play mode.
        /// </summary>
        public void Quit()
        {
            Time.timeScale = 1.0f;
            _canvas = transform.Find("PauseMenu").gameObject;
            
            if (_canvas != null)
            {
                _canvas.SetActive(false);
                Destroy(GetPlayer());
            }
            
            if (_camera != null) _camera.GetComponent<AudioListener>().enabled = true;
            _paused = false;
            
            SceneManager.LoadScene("MainMenu");
        }

        #endregion
    }
}