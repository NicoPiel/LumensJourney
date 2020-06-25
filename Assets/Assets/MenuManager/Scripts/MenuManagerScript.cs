using DialogueSystem.Scripts;
using UnityEngine;

namespace Assets.MenuManager.Scripts
{
    public class MenuManagerScript : MonoBehaviour
    {
        public GameObject bankMenu;
        public GameObject dialogueMenu;
        public GameObject pauseMenu;
        
        private string _currentMenu;
        // Start is called before the first frame update
        void Start()
        {
            _currentMenu = null;
        }

        // Update is called once per frame
 
    
        public void LoadMenu(string menu)
        {
            switch (menu)
            {
                case "BankMenu":
                    _currentMenu = "BankMenu";
                    bankMenu.SetActive(true);
                    break;
                case "DialogueMenu":
                    _currentMenu = "DialogueMenu";
                    DialogueMenu.ShowDialogueWindow();
                    break;
                case "PauseMenu":
                    _currentMenu = "PauseMenu";
                    break;
                default:
                    break;
            }
        }

        public void UnloadCurrentMenu()
        {
            switch (_currentMenu)
            {
                case "BankMenu":
                    bankMenu.SetActive(false);
                    _currentMenu = null;
                    break;
                case "DialogueMenu":
                    DialogueMenu.HideDialogueWindow();
                    _currentMenu = null;
                    break;
                case "PauseMenu":
                    _currentMenu = null;
                    break;
                default:
                    Debug.Log("_currentMenu false");
                    break;
            }
        }
    }
}
