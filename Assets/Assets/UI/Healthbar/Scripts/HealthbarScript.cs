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
            //Debug.Log("HealthBarLoaded");
            ChangeMaxHearts();
            ChangeHearts();

            GameManager.GetPlayerScript().onPlayerLifeChanged.AddListener(ChangeHearts);
            GameManager.GetPlayerScript().onPlayerStatChanged.AddListener((key) =>
            {
                Debug.Log($"HealthbarScript received: {key}");
                if (key == "MaxHealth")
                {
                    ChangeMaxHearts();
                }
            });
        }

        private void ChangeMaxHearts()
        {
            hearts = new Dictionary<int, Image>();
            Debug.Log("Trying to Change MaxHealth");
            var maxHealth = GameManager.GetPlayerScript().GetPlayerMaxHealth();
            Debug.Log($"MaxHealth will be {maxHealth}");
            foreach (Transform child in transform)
            {
                Destroy(child.gameObject);
            }

            var x = 40f;
            var y = -30f;
            for (var i = 1; i <= maxHealth; i++)
            {
                Debug.Log("heartSlot should be created");
                GameObject heartSlot = Instantiate(heart);
                heartSlot.name = "Herz";
                var rectTrans = heartSlot.GetComponent<RectTransform>();
                rectTrans.SetParent(transform);
                rectTrans.anchoredPosition = new Vector3(x, y, 0);
                rectTrans.localScale = new Vector3(1.5f, 1.5f, 1.5f);
                rectTrans.sizeDelta = new Vector2(50, 35);

                var image = heartSlot.GetComponent<Image>();
                hearts.Add(i, image);
                x += 75;
            }

            
            ChangeHearts();
        }

        private void ChangeHearts()
        {
            Debug.Log("Hearts should be filled now");
            var currentHealth = GameManager.GetPlayerScript().GetPlayerCurrentHealth();
            for (var i = 1; i <= hearts.Count; i++)
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