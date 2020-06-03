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
    private BankScript _bankScript;

  

    private void OnEnable()
    {
        if (_bankScript == null)
        {
            Debug.Log("bank");
            _bankScript =  GameObject.Find("Bank").GetComponent<BankScript>();
            _bankScript.onLightShardsStoredInBank.AddListener(OnLightShardsChanged);
        }
        
        shardsOnPlayer.text = GameManager.GetPlayerScript().GetLightShardAmount().ToString();
        shardsInBank.text = _bankScript.GetStoredLightShards().ToString();
    }

    private void OnLightShardsChanged()
    {
        shardsOnPlayer.text = GameManager.GetPlayerScript().GetLightShardAmount().ToString();
        shardsInBank.text = _bankScript.GetStoredLightShards().ToString();
    }

    public void OnStoredButtonPressed()
    {
        _bankScript.StoreAllLightShardsInBank();
    }
}
