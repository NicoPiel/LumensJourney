using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemBarScript : MonoBehaviour
{
    
    /*public void CreateItemBar(Inventory inv)
    {
        float x = 33f;
        float y = -32f;
        foreach(var item in inv.Items)
        {
            var obj = Resources.Load<GameObject>("ItemBar/Prefabs/"+item.ItemName);
            if (obj == null)
            {
                Debug.Log("Object could not be loaded");
            }
            var newItem = Instantiate(obj);
            newItem.GetComponent<RectTransform>().SetParent(transform);
            newItem.GetComponent<RectTransform>().anchoredPosition =  new Vector3(x, y, 0);
            x += 47f;
        }
    }*/
    
    public void CreateItemBar(Inventory inv)
    {
        float x = 33f;
        float y = -32f;
        foreach(var item in inv.Items)
        {
            var obj = Resources.Load<GameObject>("ItemBar/Prefabs/ItemIcon");
            if (obj == null)
            {
                Debug.Log("Object could not be loaded");
            }
            
            var newItem = Instantiate(obj);
            newItem.GetComponent<RawImage>().texture = Resources.Load<Texture>("Items/Tiles/" + item.ItemName);
            newItem.GetComponent<RectTransform>().SetParent(transform);
            newItem.GetComponent<RectTransform>().anchoredPosition =  new Vector3(x, y, 0);
            x += 47f;
        }
    }
}
