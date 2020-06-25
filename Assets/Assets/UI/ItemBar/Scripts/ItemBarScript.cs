using Assets.Player.Script;
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

        public static void UpdateItemBar_Static(Inventory inv)
        {
            _instance.UpdateItemBar(inv);
        }

        private void UpdateItemBar(Inventory inv)
        {
            foreach (Transform child in transform) {
                Destroy(child.gameObject);
            }
            float x = 33f;
            float y = -40f;
            foreach(var item in inv.Items)
            {
                var itemIcon = Instantiate(ItemIcon, transform);
                itemIcon.transform.Find("ItemBackground").transform.Find("ItemSprite").GetComponent<Image>().sprite = item.ItemSprite;
                itemIcon.GetComponent<RectTransform>().anchoredPosition =  new Vector3(x, y, 0);
                x += 102f;
            }
        }
    }
}
