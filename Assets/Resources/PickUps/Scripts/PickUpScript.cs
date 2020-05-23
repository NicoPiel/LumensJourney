using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUpScript : MonoBehaviour
{
    private GameItem _item;
    
    public void SetPickUpItem(string itemName)
    {
        _item = GameItem.ConstructItem(itemName);
        SpriteRenderer re = transform.Find("Item")?.GetComponent<SpriteRenderer>();
        re.sprite = Resources.Load<Sprite>("Items/Tiles/" + _item.ItemName);
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
