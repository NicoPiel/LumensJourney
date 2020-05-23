using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUpScript : MonoBehaviour
{
    // Start is called before the first frame update
    public void CreatePickUp(string item)
    {
        SpriteRenderer sr = gameObject.GetComponentInChildren<SpriteRenderer>();
        sr.sprite = Resources.Load<Sprite>("Items/Tiles/" + item);
    }
}
