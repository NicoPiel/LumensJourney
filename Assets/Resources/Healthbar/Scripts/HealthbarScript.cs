using Unity.Burst;
using UnityEngine;

namespace Resources.Healthbar.Scripts
{
    [BurstCompile]
    public class HealthbarScript : MonoBehaviour
    {
        // Start is called before the first frame update
        public void ChangeHealthBar(int currentHealth, int maxHealth)
        {
            Destroy(GameObject.Find("Healthbar"));
            foreach (Transform child in transform) {
                Destroy(child.gameObject);
            }
            var obj = UnityEngine.Resources.Load<GameObject>("Healthbar/Prefab/Heart");
            if (obj == null)
            {
                Debug.Log("Heart not found");
            }
            float x = 0;
            float y= 0;
            for (int i = currentHealth; i != 0; i--)
            {
                var newHeart = Instantiate(obj);
                newHeart.GetComponent<RectTransform>().SetParent(this.transform);
                newHeart.GetComponent<RectTransform>().anchoredPosition = new Vector3(x,y,0);
                x += 50;
            }
            obj = UnityEngine.Resources.Load<GameObject>("Healthbar/Prefab/EmptyHeart");
            for (int i = maxHealth - currentHealth; i != 0; i--)
            {
                var newHeart = Instantiate(obj);
                newHeart.GetComponent<RectTransform>().SetParent(this.transform);
                newHeart.GetComponent<RectTransform>().anchoredPosition = new Vector3(x,y,0);
                x += 50;
            }
        
        }
    }
}
