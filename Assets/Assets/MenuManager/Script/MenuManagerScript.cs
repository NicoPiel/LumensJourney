using DG.Tweening;
using UnityEngine;

namespace Assets.MenuManager.Script
{
    public class MenuManagerScript : MonoBehaviour
    {
        public GameObject bankMenu;
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
                    AnimateUi(bankMenu);
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
                    AnimateUi(bankMenu);
                    _currentMenu = null;
                    break;
                default:
                    Debug.Log("_currentMenu false");
                    break;
            }
        }

        private void AnimateUi(GameObject menu)
        {
            var rect = menu.GetComponent<RectTransform>();
            Sequence sequence = DOTween.Sequence().SetAutoKill(false);

            if (rect != null)
            {
                if (menu.activeSelf)
                {
                    sequence.Rewind();
                    menu.SetActive(false);
                }
                else
                {
                    menu.SetActive(true);
                    sequence.Append(rect.DOScale(0f, 1f));
                    sequence.Append(rect.DOScale(1f, 1f));
                }
            }
            else
            {
                Debug.LogWarning($"{menu.name} doesn't have a RectTransform.");
            }

        }
    }
}
