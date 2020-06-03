using System;
using System.Collections;
using System.Collections.Generic;
using Core;
using TMPro;
using UnityEngine;

public class BankMenuScript : MonoBehaviour
{
    public TMP_Text shardsOnPlayer;
    public TMP_Text shardsInBank;
    void Start()
    {
        shardsOnPlayer.text = GameManager.GetPlayerScript().GetLightShardAmount().ToString();
        shardsInBank.text = "0";
    }

    private void OnEnable()
    { 
        shardsOnPlayer.text = GameManager.GetPlayerScript().GetLightShardAmount().ToString();
    }
}
