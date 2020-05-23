using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUpScript : MonoBehaviour
{
    private GameItem _item;
    
    public void SetPickUpItem(string itemName)
    {
        _item = GameItem.ConstructItem(itemName);
        SpriteRenderer sr = gameObject.GetComponentInChildren<SpriteRenderer>();
        sr.sprite = Resources.Load<Sprite>("Items/Tiles/" + _item.ItemName);
    }

    public void OnInfoZoneEnter()
    {
        //TODO
    }

    public void OnPickUp()
    {
        //TODO
    }
}
