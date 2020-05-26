using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class HealthbarScript : MonoBehaviour
{
    // Start is called before the first frame update
    public void ChangeHealthBar(int currentHealth, int maxHealth)
    {
        Destroy(GameObject.Find("Healthbar"));
        foreach (Transform child in transform) {
            GameObject.Destroy(child.gameObject);
        }
        var obj = Resources.Load<GameObject>("Healthbar/Prefab/Heart");
        if (obj == null)
        {
            Debug.Log("Heart not found");
        }
        float x = 25;
        float y= -29;
        for (int i = currentHealth; i != 0; i--)
        {
            var newHeart = Instantiate(obj);
            newHeart.GetComponent<RectTransform>().SetParent(this.transform);
            newHeart.GetComponent<RectTransform>().anchoredPosition = new Vector3(x,y,0);
            x += 50;
        }
        
    }
}
