using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Core;
using Unity.Burst;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Healthbar.Scripts
{
    [BurstCompile]
    public class HealthbarScript : MonoBehaviour
    {
        public GameObject heart;
        public GameObject newHeart;
        public Sprite fullHeart;
        public Sprite emptyHeart;
        private Dictionary<int, Image> hearts;
      
        

        public void Start()
        {
            Debug.Log("HealthBarLoaded");
            hearts = new Dictionary<int, Image>();
            GameManager.GetPlayerScript().onPlayerLifeChanged.AddListener(ChangeHearts);
            ChangeMaxHearts();
            ChangeHearts();
        }

        // Start is called before the first frame update
        private async void ChangeHealthBar()
        {
            var currentHealth = GameManager.GetPlayerScript().GetPlayerCurrentHealth();
            var maxHealth = GameManager.GetPlayerScript().GetPlayerMaxHealth();

            foreach (Transform child in transform)
            {
                Destroy(child.gameObject);
            }

            var obj = heart;

            float x = 0;
            float y = 0;

            for (int i = currentHealth; i != 0; i--)
            {
                var newHeart = Instantiate(obj);
                newHeart.GetComponent<RectTransform>().SetParent(this.transform);
                newHeart.GetComponent<RectTransform>().anchoredPosition = new Vector3(x, y, 0);
                x += 100;
            }

            obj = newHeart;
            for (int i = maxHealth - currentHealth; i != 0; i--)
            {
                var newHeart = Instantiate(obj);
                newHeart.GetComponent<RectTransform>().SetParent(this.transform);
                newHeart.GetComponent<RectTransform>().anchoredPosition = new Vector3(x, y, 0);
                x += 100;
            }

            await Task.Yield();
        }

        private void ChangeMaxHearts()
        {
            var maxHealth = GameManager.GetPlayerScript().GetPlayerMaxHealth();
            
            foreach (Transform child in transform)
            {
                Destroy(child.gameObject);
            }
            float x = 11f;
            float y = -20f;
            for (int i = 1; i <= maxHealth; i++)
            {
                var heartSlot = Instantiate(heart);
                heartSlot.name = "Herz";
                var rect = heartSlot.GetComponent<RectTransform>();
                rect.SetParent(transform);
                rect.anchoredPosition = new Vector3(x, y, 0);
                
                
                var image = heartSlot.GetComponent<Image>();
                hearts.Add(i, image);
                x += 100;
            }
        }

        private void ChangeHearts()
        {
            var currentHealth = GameManager.GetPlayerScript().GetPlayerCurrentHealth();
            for (int i = 1; i <= hearts.Count; i++)
            {
                if (currentHealth >= i)
                {
                    hearts[i].sprite = fullHeart;
                }
                else
                {
                    hearts[i].sprite = emptyHeart;
                }
            }
        }
    }
}