using System.IO;
using Core;
using Unity.Burst;
using UnityEngine;
using UnityEngine.UI;

namespace MainMenu
{
    [BurstCompile]
    public class MainMenu : MonoBehaviour
    {
        public Sprite cursorSprite;
        public GameObject newGameButton;
        public GameObject loadGameButton;

        private void Start()
        {
            Cursor.SetCursor(SpriteToTexture(cursorSprite), Vector2.zero, CursorMode.Auto);
            SwapButtons();
        }

        public void NewGame()
        {
            GameManager.LoadNewGameCutscene_Static();
        }

        public void LoadGame()
        {
            GameManager.LoadGame_Static();
        }

        public void DeleteSave()
        {
            if (GameManager.GetSaveSystem().DeleteSave())
            {
                SwapButtons();
                Debug.Log("Successfully deleted save file.");
            }
            else Debug.Log("Couldn't delete save file.");
        }

        private void SwapButtons()
        {
            if (GameManager.GetSaveSystem().SaveExists())
            {
                loadGameButton.SetActive(true);
                newGameButton.SetActive(false);
            }
            else
            {
                newGameButton.SetActive(true);
                loadGameButton.SetActive(false);
            }
        }

        public void PlayMenuSound()
        {
            var audioSource = GetComponent<AudioSource>();
            audioSource.clip = UnityEngine.Resources.Load<AudioClip>("Clicks/click3");
            audioSource.Play();
        }

        private static Texture2D SpriteToTexture(Sprite sprite)
        {
            var croppedTexture = new Texture2D( (int)sprite.rect.width, (int)sprite.rect.height );
            
            var pixels = sprite.texture.GetPixels(  (int)sprite.textureRect.x, 
                (int)sprite.textureRect.y, 
                (int)sprite.textureRect.width, 
                (int)sprite.textureRect.height );
            
            croppedTexture.SetPixels( pixels );
            croppedTexture.Apply();

            return croppedTexture;
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