using System;
using Assets.Items.Scripts;
using Assets.Player.Script;
using Core;
using Unity.Burst;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.UI.ItemBar.Scripts
{
    [BurstCompile]
    public class ItemBarScript : MonoBehaviour
    {
        private static ItemBarScript _instance;
        [SerializeField] private GameObject ItemIcon;

        public void Awake()
        {
            _instance = this;
        }

        private void Start()
        {
            GameManager.GetPlayerScript().onItemAddedToPlayerInventory.AddListener(UpdateItemBar);
        }

        private void UpdateItemBar()
        {
            //Debug.Log("Updating ItemBar");
            
            foreach (Transform child in transform) {
                Destroy(child.gameObject);
            }
            
            var x = 33f;
            var y = -40f;
            
            foreach(GameItem item in GameManager.GetPlayerScript().GetPlayerInventory().Items)
            {
                GameObject itemIcon = Instantiate(ItemIcon, transform);
                itemIcon.transform.Find("ItemBackground").transform.Find("ItemSprite").GetComponent<Image>().sprite = item.ItemSprite;
                itemIcon.GetComponent<RectTransform>().anchoredPosition =  new Vector3(x, y, 0);
                x += 102f;
            }
        }
    }
}
