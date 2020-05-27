using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LightShardDisplayScript : MonoBehaviour
{
    private TMP_Text _textbox;

    public void Start()
    {
        _textbox = gameObject.GetComponentInChildren<TMP_Text>();
    }

    public void UpdateLightShardDisplay(int shardChange)
    {
        _textbox.text = shardChange.ToString();
    }
}
