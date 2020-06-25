using System;
using System.Collections;
using System.Collections.Generic;
using Core;
using UnityEngine;
using UnityEngine.Events;

public class HubLoaderScript : MonoBehaviour
{
    
    // Start is called before the first frame update

    void Start()
    {
        GameManager.GetGameManager().onHubEntered.Invoke();
        Debug.Log("Hub Loaded.");
    }
}
