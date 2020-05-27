using Unity.Burst;
using UnityEngine;

namespace Resources.PlayerUI
{
    [BurstCompile]
    public class PlayerUiScript : MonoBehaviour
    {
        public GameObject healthbar;

        public GameObject itembar;

        public GameObject lightbar;
        // Start is called before the first frame update
        void Start()
        {
            var hb = Instantiate(healthbar, transform);
            hb.GetComponent<RectTransform>().anchoredPosition = new Vector3(-675, 470, 0);

            var ib = Instantiate(itembar, transform);
            ib.GetComponent<RectTransform>().anchoredPosition = new Vector3(-760, 400, 0);

            var lb = Instantiate(lightbar, transform);
            lb.GetComponent<RectTransform>().anchoredPosition = new Vector3(-920, -440, 0);
        }

        // Update is called once per frame
        void Update()
        {
        
        }
    }
}
