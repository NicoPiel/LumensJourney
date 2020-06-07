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
            loadGameButton.SetActive(false);
            newGameButton.SetActive(false);
            Cursor.SetCursor(SpriteToTexture(cursorSprite), Vector2.zero, CursorMode.Auto);
            if (File.Exists(Application.persistentDataPath + "/save.json"))
            {
               loadGameButton.SetActive(true);
            }
            else
            {
                newGameButton.SetActive(true);
            }
        }
        
        public void NewGame()
        {
            GameManager.NewGame_Static();
        }

        public void LoadGame()
        {
            GameManager.LoadGame_Static();
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