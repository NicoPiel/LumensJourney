using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using UnityEngine;
using UnityEngine.Events;
using Utility;

public class PickUpScript : MonoBehaviour
{
    private GameItem _item;
    private bool _entered;
    private PlayerScript _player;
    
    public void SetPickUpItem(string itemName)
    {
        _item = GameItem.ConstructItem(itemName);
        SpriteRenderer re = transform.Find("Item")?.GetComponent<SpriteRenderer>();
        re.sprite = Resources.Load<Sprite>("Items/Tiles/" + _item.ItemName);
    }

    public void Start()
    {
        _player = GameObject.FindWithTag("Player").GetComponent<PlayerScript>();
    }

    public void Update()
    {
        if (_entered && Input.GetKeyDown(KeyCode.E)){
            _player.AddToInventory(_item);
            Tooltip.HideTooltip_Static();
            Destroy(gameObject);
        }
    }

    public void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            StringBuilder n = new StringBuilder();
            n.Append(_item.ItemName + "\n");

            foreach (var stuff in _item.ValueIncreasments)
            {
                n.Append(stuff.Key + " " + stuff.Value + "\n");
            }

            Tooltip.ShowTooltip_Static(n.ToString());
            _entered = true;
        }
    }
    

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            Tooltip.HideTooltip_Static();
            _entered = false;
        }
    }


    public void OnCollisionStay2D(Collision2D other)
    {
        
    }

    public void OnPickUp()
    {
        //TODO
    }
}
