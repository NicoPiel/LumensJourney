using System.Collections.Generic;
using Core;
using Unity.Burst;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.UI.Healthbar.Scripts
{
    [BurstCompile]
    public class HealthbarScript : MonoBehaviour
    {
        public GameObject heart;
        public Sprite full;
        public Sprite firstfull;
        public Sprite firstempty;
        public Sprite empty;
        private Dictionary<int, Image> hearts;
      
        

        public void Start()
        {
            Debug.Log("HealthBarLoaded");
            hearts = new Dictionary<int, Image>();
            ChangeMaxHearts();
            ChangeHearts();
            
            GameManager.GetPlayerScript().onPlayerLifeChanged.AddListener(ChangeHearts);
            
        }

        private void ChangeMaxHearts()
        {
            var maxHealth = GameManager.GetPlayerScript().GetPlayerMaxHealth();
            
            foreach (Transform child in transform)
            {
                Destroy(child.gameObject);
            }
            float x = 40f;
            float y = -30f;
            for (int i = 1; i <= maxHealth; i++)
            {
                var heartSlot = Instantiate(heart);
                heartSlot.name = "Herz";
                var rectTrans = heartSlot.GetComponent<RectTransform>();
                rectTrans.SetParent(transform);
                rectTrans.anchoredPosition = new Vector3(x, y, 0);
                rectTrans.localScale = new Vector3(1.5f, 1.5f,1.5f);
                rectTrans.sizeDelta = new Vector2(50, 35);
              


                var image = heartSlot.GetComponent<Image>();
                hearts.Add(i, image);
                x += 75;
            }
        }

        private void ChangeHearts()
        {
            var currentHealth = GameManager.GetPlayerScript().GetPlayerCurrentHealth();
            for (int i = 1; i <= hearts.Count; i++)
            {
                if (currentHealth >= i)
                {
                    hearts[i].sprite = i == 1 ? firstfull : full;
                }
                else
                {
                    hearts[i].sprite = i == 1 ? firstempty : empty;
                }
                
                
            }
        }
    }
}