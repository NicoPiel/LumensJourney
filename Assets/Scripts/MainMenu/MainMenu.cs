using Core;
using UnityEngine;

namespace MainMenu
{
    public class MainMenu : MonoBehaviour
    {
        public void NewGame()
        {
            GameManager.NewGame_Static();
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