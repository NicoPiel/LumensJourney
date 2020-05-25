using Core;
using UnityEngine;

namespace MainMenu
{
    public class MainMenu : MonoBehaviour
    {
        public Sprite cursorSprite;

        private void Start()
        {
            Cursor.SetCursor(SpriteToTexture(cursorSprite), Vector2.zero, CursorMode.Auto);
        }
        
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

        public void PlayMenuSound()
        {
            var audioSource = GetComponent<AudioSource>();
            audioSource.clip = Resources.Load<AudioClip>("Audio/Clicks/click3");
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
    }
}