using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class HealthbarScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        ChangeHealthBar(5);
    }

    public void ChangeHealthBar(int currentHealth)
    {
        Destroy(GameObject.Find("Healthbar"));
        foreach (Transform child in transform) {
            GameObject.Destroy(child.gameObject);
        }
        var obj = Resources.Load<GameObject>("Healthbar/Prefab/Heart");
        if (obj == null)
        {
            Debug.Log("ja dann sip mir doch einen");
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
