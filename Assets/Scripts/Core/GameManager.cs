using System.Collections;
using System.IO;
using Assets.MenuManager.Scripts;
using Assets.Player.Script;
using Assets.ProceduralGeneration.Core;
using Assets.SaveSystem;
using Assets.UI.PauseMenu.Scripts;
using Assets.UI.PlayerUI.Scripts;
using DialogueSystem.Scripts;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions;
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

        #region Inspector variables

        [SerializeField] private SaveSystem saveSystem;
        [SerializeField] private Generator generator;
        [SerializeField] private GameObject unityPlayerObject;
        [SerializeField] private PlayerScript playerScript;
        [SerializeField] private Camera mainCamera;
        [SerializeField] private MenuManagerScript menuManagerScript;
        [SerializeField] private DialogueManager dialogueManager;
        [SerializeField] private EventHandler eventHandler;

        #endregion

        #region Public variables

        public static int CurrentLevel { get; set; }
        public static int MaxLevels { get; set; } = 5;
        public int StoryStoneProgression { get; set; }
        public int DiaryProgression { get; set; }
        public int RunsCompleted { get; set; }

        public static string streamingItemFilePath;
        public static string streamingDialogueFilePath;
        public static string streamingObjectsFilePath;

        #endregion

        #region Private variables

        private GameObject _canvas;
        private Camera _camera;

        private AudioClip _menuSound;

        private bool _ingame = false;
        public bool Paused { get; set; } = false;
        
        public static bool isPressingInteractButton;
        public static bool cameFromGuardian;
        public static bool playerDied;

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
            CurrentLevel = 0;

            streamingDialogueFilePath = Application.streamingAssetsPath+ "/XML/dialogues.xml";
            streamingItemFilePath = Application.streamingAssetsPath + "/XML/items.xml";
            streamingObjectsFilePath = Application.streamingAssetsPath + "/XML/storyobjects.xml";
        }

        /// <summary>
        /// Add event listeners here.
        /// </summary>
        private void Start()
        {
            
            menuManagerScript = GetComponentInChildren<MenuManagerScript>();
            eventHandler.onPlayerSpawned.AddListener(OnPlayerSpawned);
            eventHandler.onHubEntered.AddListener(() =>
            {
                HubEntered();
            });

            cameFromGuardian = false;
            isPressingInteractButton = false;
            playerDied = false;
        }

        /// <summary>
        /// Use this to listen for global input.
        /// </summary>
        private void Update()
        {
            var escape = Input.GetButtonDown("Escape");
            var e = Input.GetButtonDown("Interact");
            
            // Pause Menu
            if (escape)
            {
                if (!_ingame) return;

                if (Paused)
                {
                    PauseMenu.Resume();
                }
                else
                {
                    PauseMenu.Pause();
                }
            }

            if (e)
            {
                isPressingInteractButton = true;
            }
            else isPressingInteractButton = false;
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

            eventHandler.onNewGameStarted?.Invoke();
        }

        /// <summary>
        /// Loads a game from file using the <see cref="saveSystem"/>.
        /// </summary>
        private void LoadGame()
        {
            saveSystem.LoadSave();
            Setup();
            eventHandler.onGameLoaded.Invoke();
        }

        /// <summary>
        /// Loads the Hub scene, then proceeds to instantiate the player.
        /// Also adds listeners for events that are dungeon-specific.
        /// </summary>
        private void Setup()
        {
            SceneManager.LoadScene("Hub");

            ModifyGenerator();
            InstantiatePlayer();

            SceneManager.sceneLoaded += (scene, mode) =>
            {
                if (scene.name == "Dungeon")
                {
                    eventHandler.onPlayerLightLevelChanged.AddListener(OnPlayerLightLevelChanged);
                }
            };

            _ingame = true;
        }

        private void HubEntered()
        {
            CurrentLevel = 0;
            Debug.Log("Hub Loaded.");
            Debug.Assert(CurrentLevel == 0, "CurrentLevel is not 0");
            GetPlayerScript().ResetPlayer();

            unityPlayerObject.transform.position = new Vector3(-2,-2,0);

            SceneManager.sceneLoaded -= GenerateDungeon;

            if (cameFromGuardian)
            {
                var runsCompleted = GetSaveSystem().RunsCompleted;

                StartCoroutine(runsCompleted == 1
                    ? FadeTextInAndOut("You just met The Guardian for the first time.")
                    : FadeTextInAndOut($"You have met the Guardian {runsCompleted} times."));

                cameFromGuardian = false;
            }

            if (playerDied)
            {
                StartCoroutine(FadeTextInAndOut("You Died."));
                playerScript.UnfreezeControls();
                playerDied = false;
            }
        }

        /// <summary>
        /// Modifies the generator's properties and adds listeners.
        /// </summary>
        private void ModifyGenerator()
        {
            if (generator == null) return;
            generator.name = "DungeonGenerator";
            eventHandler.onDungeonGenerated.AddListener(OnDungeonGenerated);
        }

        /// <summary>
        /// Instantiates player and related objects, such as some UI elements.
        /// Also finds and sets the <see cref="playerScript"/>
        /// </summary>
        private void InstantiatePlayer()
        {
            _canvas = GetMenuManagerScript().pauseMenu;

            unityPlayerObject = Instantiate(unityPlayerObject, new Vector3(-2, -2, 0), Quaternion.identity, gameObject.transform);
            unityPlayerObject.name = "Player";
            if (unityPlayerObject != null) _camera = unityPlayerObject.transform.Find("MainCamera").GetComponent<Camera>();
            if (unityPlayerObject != null) playerScript = unityPlayerObject.GetComponent<PlayerScript>();
            eventHandler.onPlayerSpawned.Invoke();
        }

        #endregion

        #region Event subscriptions

        /// <summary>
        /// Does work whenever the player loses or gains light.
        /// </summary>
        private void OnPlayerLightLevelChanged()
        {
            if (!_ingame || Camera.main == null || unityPlayerObject == null) return;

            var playerLight = unityPlayerObject.transform.Find("PlayerLight").GetComponent<Light2D>();
            playerLight.intensity = 1;
            playerLight.pointLightOuterRadius = 6 * GetPlayer().GetPlayerLightLevel() + 2;

            var volume = _camera.GetComponent<Volume>();

            ChromaticAberration chromaticAberration;
            LensDistortion lensDistortion;

            volume.profile.TryGet(out chromaticAberration);
            volume.profile.TryGet(out lensDistortion);

            chromaticAberration.intensity.value = 0.7f * GetPlayer().GetPlayerLightLevel();
            lensDistortion.intensity.value = 0.1f;

            unityPlayerObject.transform.Find("PlayerLight").GetComponent<LightFlickerEffect>().effectEnabled = true;
        }

        /// <summary>
        /// Does work whenever a new level is generated.
        /// </summary>
        private void OnDungeonGenerated()
        {
            CurrentLevel++;

            var text = PlayerUiScript.GetPlayerUiScript().GetLevelTooltip().GetComponent<TMP_Text>();
            if (text != null)
            {
                text.text = $"Level {CurrentLevel}";
                text.gameObject.SetActive(true);
            }

            playerScript.GetPlayer().HealPlayerFull();

            StartCoroutine(FadeLevelTextInAndOut());
        }

        /// <summary>
        /// Does work when the player is spawned.
        /// </summary>
        private void OnPlayerSpawned()
        {
            _camera = GetUnityPlayerObject().transform.Find("MainCamera").GetComponent<Camera>();
        }

        private void OnRunCompleted()
        {
            saveSystem.RunsCompleted++;
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
            var text = PlayerUiScript.GetPlayerUiScript().GetLevelTooltip().GetComponent<TMP_Text>();

            if (text == null) yield break;

            text.gameObject.SetActive(true);

            StartCoroutine(TextFade.FadeTextToFullAlpha(3f*Time.timeScale, text));
            yield return new WaitForSeconds(3f*Time.timeScale);
            StartCoroutine(TextFade.FadeTextToZeroAlpha(2f*Time.timeScale, text));
            yield return new WaitForSeconds(2f*Time.timeScale);

            text.gameObject.SetActive(false);
        }

        /// <summary>
        /// Uses an alpha effect to fade text (Level 1, 2, 3, etc) in and out.
        /// </summary>
        /// <returns></returns>
        public static IEnumerator FadeTextInAndOut(string text, float fadeInTime = 3f, float fadeOutTime = 2f)
        {
            var textObject = PlayerUiScript.GetPlayerUiScript().GetLevelTooltip().GetComponent<TMP_Text>();

            if (textObject == null) yield break;

            textObject.text = text;

            textObject.gameObject.SetActive(true);

            _instance.StartCoroutine(TextFade.FadeTextToFullAlpha(fadeInTime*Time.timeScale, textObject));
            yield return new WaitForSeconds(fadeInTime*Time.timeScale);
            _instance.StartCoroutine(TextFade.FadeTextToZeroAlpha(fadeOutTime*Time.timeScale, textObject));
            yield return new WaitForSeconds(fadeOutTime*Time.timeScale);

            textObject.gameObject.SetActive(false);
        }

        public static void GenerateNextLevel()
        {
            if (CurrentLevel < MaxLevels)
            {
                if (SceneManager.GetActiveScene().name != "Dungeon")
                {
                    SceneManager.LoadScene("Dungeon");

                    SceneManager.sceneLoaded += GenerateDungeon;
                }
                else
                {
                    GenerateDungeon(SceneManager.GetActiveScene());
                }
            }
            else
            {
                GetGenerator().DestroyDungeon();
                StartLastLevel();
            }
        }

        public static void StartLastLevel()
        {
            _instance.StartCoroutine(FadeTextInAndOut("The End"));
            SceneManager.LoadScene("LastLevel");
            GetUnityPlayerObject().transform.position = new Vector2(8, 1);
        }

        private static void GenerateDungeon(Scene scene, LoadSceneMode mode = LoadSceneMode.Single)
        {
            if (scene.name == "Dungeon")
            {
                var numberOfRooms = Random.Range(15, 30);
                _instance.generator.Generate(numberOfRooms);
            }
        }

        #endregion

        #region Getters

        public static GameObject GetUnityPlayerObject()
        {
            return _instance.unityPlayerObject;
        }

        public static PlayerScript GetPlayerScript()
        {
            return _instance.playerScript;
        }

        public static Generator GetGenerator()
        {
            return _instance.generator;
        }

        public static Player GetPlayer()
        {
            return _instance.playerScript.GetPlayer();
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

        public static EventHandler GetEventHandler()
        {
            return _instance.eventHandler;
        }

        public static string GetItemFilePath()
        {
            return Application.persistentDataPath + "/items.xml";
        }

        #endregion
    }
}