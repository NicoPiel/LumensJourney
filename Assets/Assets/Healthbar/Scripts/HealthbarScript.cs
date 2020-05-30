using System;
using System.Threading.Tasks;
using Core;
using Unity.Burst;
using UnityEngine;

namespace Assets.Healthbar.Scripts
{
    [BurstCompile]
    public class HealthbarScript : MonoBehaviour
    {
        public GameObject heart;
        public GameObject newHeart;
        public void Start()
        {
            GameManager.GetPlayerScript().onPlayerLifeChanged.AddListener(ChangeHealthBar);
            ChangeHealthBar();
        }

        // Start is called before the first frame update
        public async void ChangeHealthBar()
        {
            var currentHealth = GameManager.GetPlayerScript().GetPlayerCurrentHealth();
            var maxHealth = GameManager.GetPlayerScript().GetPlayerMaxHealth();
            
            foreach (Transform child in transform) {
                Destroy(child.gameObject);
            }

            var obj = heart;
            
            float x = 0;
            float y= 0;
            
            for (int i = currentHealth; i != 0; i--)
            {
                var newHeart = Instantiate(obj);
                newHeart.GetComponent<RectTransform>().SetParent(this.transform);
                newHeart.GetComponent<RectTransform>().anchoredPosition = new Vector3(x,y,0);
                x += 100;
            }

            obj = newHeart;
            for (int i = maxHealth - currentHealth; i != 0; i--)
            {
                var newHeart = Instantiate(obj);
                newHeart.GetComponent<RectTransform>().SetParent(this.transform);
                newHeart.GetComponent<RectTransform>().anchoredPosition = new Vector3(x,y,0);
                x += 100;
            }

            await Task.Yield();
        }
    }
}
