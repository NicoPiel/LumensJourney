using System;
using Core;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.UI.PauseMenu.Scripts
{
    public class PauseMenu : MonoBehaviour
    {
        private static PauseMenu _instance;
        
        private RectTransform _pauseMenuTransform;
        private bool _canUseMenu;

        private void Awake()
        {
            _instance = this;
        }

        private void Start()
        {
            _pauseMenuTransform = GetComponent<RectTransform>();
        }

        #region Pause menu
        
        /// <summary>
        /// Resumes the game and closes the pause menu.
        /// </summary>
        public static void Resume()
        {
            if (!GameManager.GetGameManager().Paused && !_instance._canUseMenu) return;

            _instance._canUseMenu = false;
            
            Time.timeScale = 1.0f;
            GameManager.GetPlayer().transform.Find("PlayerUI").gameObject.SetActive(true);

            _instance._pauseMenuTransform.LeanMoveX(-600, 0.5f).setOnComplete(() =>
            {
                GameManager.GetGameManager().Paused = false;
                _instance._canUseMenu = true;
            });
        }
        
        /// <summary>
        /// Resumes the game and closes the pause menu.
        /// </summary>
        public void ResumeOnClick()
        {
            if (!GameManager.GetGameManager().Paused && !_canUseMenu) return;
            
            _canUseMenu = false;
            
            Time.timeScale = 1.0f;
            GameManager.GetPlayer().transform.Find("PlayerUI").gameObject.SetActive(true);
            
            _pauseMenuTransform.LeanMoveX(-600, 0.5f).setOnComplete(() =>
            {
                GameManager.GetGameManager().Paused = false;
                _canUseMenu = true;
            });
        }

        /// <summary>
        /// Pauses the game and opens the pause menu.
        /// </summary>
        public static void Pause()
        {
            if (GameManager.GetGameManager().Paused && !_instance._canUseMenu) return;

            _instance._canUseMenu = false;

            GameManager.GetPlayer().transform.Find("PlayerUI").gameObject.SetActive(false);
            _instance._pauseMenuTransform.LeanMoveX(0, 0.5f).setOnComplete(() =>
            {
                Time.timeScale = 0.0f;
                _instance._canUseMenu = true;
                GameManager.GetGameManager().Paused = true;
            });
        }

        /// <summary>
        /// Quits the application.
        /// If in editor, ends play mode.
        /// </summary>
        public void Quit()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }

        #endregion
    }
}