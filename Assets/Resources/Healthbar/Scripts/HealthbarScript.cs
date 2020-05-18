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

    void ChangeHealthBar(int currentHealth)
    {
        var obj = Resources.Load<GameObject>("Healthbar/Prefab/Heart");
        GameObject hearts= new GameObject("Hearts");
        hearts.transform.parent = this.transform;
        if (obj == null)
        {
            Debug.Log("ja dann sip mir doch einen");
        }
        float x = -5.1f;
        float y = 4.7f;
        for (int i = currentHealth; i != 0; i--)
        {
            var newHeart = Instantiate(obj);
            newHeart.transform.parent = hearts.transform;
            newHeart.transform.position = new Vector3(x,y,0);
            x += 0.5f;
        }
        
    }
}
