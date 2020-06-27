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
        [SerializeField] private GameObject heart;
        [SerializeField] private Sprite full;
        [SerializeField] private Sprite firstFull;
        [SerializeField] private Sprite firstEmpty;
        [SerializeField] private Sprite empty;
        private Dictionary<int, Image> _hearts;

        private void Start()
        {
            //Debug.Log("HealthBarLoaded");
            ChangeMaxHearts();
            ChangeHearts();

            GameManager.GetEventHandler().onPlayerLifeChanged.AddListener(ChangeHearts);
            GameManager.GetEventHandler().onPlayerStatChanged.AddListener((key) =>
            {
                //Debug.Log($"HealthbarScript received: {key}");
                if (key == "MaxHealth")
                {
                    ChangeMaxHearts();
                }
            });
        }

        private void ChangeMaxHearts()
        {
            _hearts = new Dictionary<int, Image>();
            //Debug.Log("Trying to Change MaxHealth");
            var maxHealth = GameManager.GetPlayer().GetPlayerMaxHealth();
            //Debug.Log($"MaxHealth will be {maxHealth}");
            foreach (Transform child in transform)
            {
                Destroy(child.gameObject);
            }

            var x = 40f;
            var y = -30f;
            for (var i = 1; i <= maxHealth; i++)
            {
                //Debug.Log("heartSlot should be created");
                GameObject heartSlot = Instantiate(heart);
                heartSlot.name = "Herz";
                var rectTrans = heartSlot.GetComponent<RectTransform>();
                rectTrans.SetParent(transform);
                rectTrans.anchoredPosition = new Vector3(x, y, 0);
                rectTrans.localScale = new Vector3(1.5f, 1.5f, 1.5f);
                rectTrans.sizeDelta = new Vector2(50, 35);

                var image = heartSlot.GetComponent<Image>();
                _hearts.Add(i, image);
                x += 75;
            }

            
            ChangeHearts();
        }

        private void ChangeHearts()
        {
            var currentHealth = GameManager.GetPlayer().GetPlayerCurrentHealth();
            for (var i = 1; i <= _hearts.Count; i++)
            {
                if (currentHealth >= i)
                {
                    _hearts[i].sprite = i == 1 ? firstFull : full;
                }
                else
                {
                    _hearts[i].sprite = i == 1 ? firstEmpty : empty;
                }
            }
        }
    }
}