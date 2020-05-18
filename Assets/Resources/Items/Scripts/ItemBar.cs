using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemBar : MonoBehaviour
{
    
    public void CreateItemBar(Inventory inv)
    {
        GameObject itemBar = gameObject;

        float x = -6.3f;
        float y = 4.1f;
        foreach(var item in inv.Items)
        {
            var obj = Resources.Load<GameObject>("Items/Prefabs/"+item.ItemName);
            if (obj == null)
            {
                Debug.Log("Object could not be loaded");
            }
            var newItem = Instantiate(obj);
            newItem.transform.parent = itemBar.transform;
            newItem.transform.position = new Vector3(x,y,0);
            x += 0.6f;
        }
    }
}
