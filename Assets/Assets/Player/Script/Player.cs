using System.Collections.Generic;
using Assets.Items.Scripts;
using Unity.Burst;

namespace Assets.Player.Script
{
    [BurstCompile]
    public class Player {
        private string Playername {get; set;}
        public Dictionary<string, int> playerstats { get; set; }
        public Inventory Inventory { get; set;}

        public Player(string playername, int maxHealth = 5, int attackDamage = 5, int defense = 5)
        {
            Playername = playername;
            Inventory = new Inventory();
            playerstats = new Dictionary<string, int>();
            ConstructPlayerStats(maxHealth, attackDamage, defense);
        }

        private void ConstructPlayerStats(int maxHealth, int attackDamage, int defense)
        {
            playerstats.Add("MaxHealth",  maxHealth);
            playerstats.Add("CurrentHealth", maxHealth);
            playerstats.Add("AttackDamage", attackDamage);
            playerstats.Add("FireDamage", 0);
            playerstats.Add("IceDamage", 0);
            playerstats.Add("Defense", defense);
            playerstats.Add("ElementalDefense", 0);
            playerstats.Add("MaxLightLevel", 1000);
            playerstats.Add("CurrentLightLevel", 1000);
        }
        private void OnItemChange(GameItem item, bool removed)
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
}
