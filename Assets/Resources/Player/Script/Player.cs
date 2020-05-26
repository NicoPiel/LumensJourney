﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player {
    private string Playername {get; set;}
    
    public Dictionary<string, int> playerstats { get; set; }
    public Inventory Inventory { get; set;}
    
    public float LightLevel { get; set; }
    
    // Start is called before the first frame update
    public Player(string playername, int maxHealth = 20, int attackDamage = 5, int defense = 5)
    {
        Playername = playername;
        Inventory = new Inventory();
        playerstats = new Dictionary<string, int>();
        ConstructDictionary(maxHealth, attackDamage, defense);
    }

    public void ConstructDictionary(int maxHealth, int attackDamage, int defense)
    {
        playerstats.Add("MaxHealth",  maxHealth);
        playerstats.Add("CurrentHealth", maxHealth);
        playerstats.Add("AttackDamage", attackDamage);
        playerstats.Add("FireDamage", 0);
        playerstats.Add("IceDamage", 0);
        playerstats.Add("Defense", defense);
        playerstats.Add("ElementalDefense", 0);
    }
    public void OnItemChange(GameItem item, bool removed)
    {
        foreach (var itemStat in item.ValueIncreasments)
        {
            if (removed)
                playerstats[itemStat.Key] -= itemStat.Value;
            else
            {
                playerstats[itemStat.Key] += itemStat.Value;
            }
                
        }

      
    }
}
