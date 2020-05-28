using System.Collections;
using System.Collections.Generic;
using Assets.Player.Script;
using UnityEngine;

[System.Serializable]
public class Save
{
    public int _lightShards {get; set;}
    public int _smithProgress {get; set;}
    // Start is called before the first frame update
    public Save()
    {
        _lightShards = 0;
        _smithProgress = 0;
    }
}
