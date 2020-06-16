using System.Collections.Generic;
using Assets.Items.Scripts;
using Unity.Burst;

namespace Assets.Player.Script
{
    [BurstCompile]
    public class Player {
        private string Playername {get; set;}
        public Dictionary<string, int> PlayerStats { get; set; }
        public Inventory Inventory { get; set;}

        public Player(string playername, int maxHealth = 5, int attackDamage = 5, int defense = 5)
        {
            Playername = playername;
            Inventory = new Inventory();
            PlayerStats = new Dictionary<string, int>();
            ConstructPlayerStats(maxHealth, attackDamage, defense);
        }

        private void ConstructPlayerStats(int maxHealth, int attackDamage, int defense)
        {
            PlayerStats.Add("MaxHealth",  maxHealth);
            PlayerStats.Add("CurrentHealth", maxHealth);
            PlayerStats.Add("AttackDamage", attackDamage);
            PlayerStats.Add("FireDamage", 0);
            PlayerStats.Add("IceDamage", 0);
            PlayerStats.Add("Defense", defense);
            PlayerStats.Add("ElementalDefense", 0);
            PlayerStats.Add("MaxLightLevel", 1000);
            PlayerStats.Add("CurrentLightLevel", 1000);
        }
        
        private void OnItemChange(GameItem item, bool removed)
        {
            foreach (var itemStat in item.ValueIncreases)
            {
                if (removed)
                    PlayerStats[itemStat.Key] -= itemStat.Value;
                else
                {
                    PlayerStats[itemStat.Key] += itemStat.Value;
                }
                
            }
        }
    }
}
