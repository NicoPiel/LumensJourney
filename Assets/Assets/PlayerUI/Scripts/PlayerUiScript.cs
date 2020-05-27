using Unity.Burst;
using UnityEngine;

namespace Assets.PlayerUI.Scripts
{
    [BurstCompile]
    public class PlayerUiScript : MonoBehaviour
    {
        public GameObject healthbar;

        public GameObject itembar;

        public GameObject lightbar;

        public GameObject lightShardCounter;

        // Start is called before the first frame update
        void Start()
        {
            var hb = Instantiate(healthbar, transform);
            hb.GetComponent<RectTransform>().anchoredPosition = new Vector3(300, -80, 0);

            var ib = Instantiate(itembar, transform);
            ib.GetComponent<RectTransform>().anchoredPosition = new Vector3(-760, 400, 0);

            var lb = Instantiate(lightbar, transform);
            lb.GetComponent<RectTransform>().anchoredPosition = new Vector3(40, 90, 0);

            var lsc = Instantiate(lightShardCounter, transform);
            lsc.GetComponent<RectTransform>().anchoredPosition = new Vector3(-120,-40,0);
            
        }

        // Update is called once per frame
        void Update()
        {
        }
    }
}