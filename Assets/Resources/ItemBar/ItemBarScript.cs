using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemBarScript : MonoBehaviour
{
    private static ItemBarScript _instance;

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
                    GameObject.Destroy(child.gameObject);
        }
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
            newItem.GetComponentInChildren<RawImage>().texture = Resources.Load<Texture>("Items/Tiles/" + item.ItemName);
            newItem.GetComponent<RectTransform>().SetParent(transform);
            newItem.GetComponent<RectTransform>().anchoredPosition =  new Vector3(x, y, 0);
            x += 47f;
        }
    }
}
