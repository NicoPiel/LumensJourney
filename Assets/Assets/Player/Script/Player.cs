using System;
using System.Collections.Generic;
using Assets.Items.Scripts;
using Core;
using Unity.Burst;

namespace Assets.Player.Script
{

    [BurstCompile]
    public class Player
    {
        private string _playername;
        private Dictionary<string, int> _playerStats;
        private Inventory _inventory;
        public Player(string playername, int maxHealth = 5, int attackDamage = 1, int defense = 5)
        {
            _playername = playername;
            _inventory = new Inventory();
            _playerStats = new Dictionary<string, int>();
            ConstructPlayerStats(maxHealth, attackDamage, defense);

            GameManager.GetEventHandler().onItemAddedToInventory.AddListener((item) => OnItemChange(item, false));
            GameManager.GetEventHandler().onPlayerStatChanged.AddListener((key)=>
            {
                if (key == "MaxHealth")
                {
                    HealPlayerFull();
                }
                
            });
        }
        private void ConstructPlayerStats(int maxHealth, int attackDamage, int defense)
        {
            _playerStats.Add("MaxHealth", maxHealth);
            _playerStats.Add("CurrentHealth", maxHealth);
            _playerStats.Add("AttackDamage", attackDamage);
            _playerStats.Add("Defense", defense);
            _playerStats.Add("Speed", 4);
            _playerStats.Add("MaxLightLevel", 1000);
            _playerStats.Add("CurrentLightLevel", 1000);
        }
        public void PlayerChangeLightLevel(int lightlevel)
        {
            _playerStats["CurrentLightLevel"] += lightlevel;
            if (_playerStats["CurrentLightLevel"] < 0) _playerStats["CurrentLightLevel"] = 0;
            GameManager.GetEventHandler().onPlayerLightLevelChanged.Invoke();
        }
        
        
        #region CurrentHealth
        public int GetPlayerCurrentHealth()
        {
            return _playerStats["CurrentHealth"];
        }
        public void SetPlayerCurrentHealth(int value)
        {
            _playerStats["CurrentHealth"] = value;
            GameManager.GetEventHandler().onPlayerLifeChanged.Invoke();
        }
        public void PlayerTakeHeal(int heal)
        {
            _playerStats["CurrentHealth"] += heal;
            GameManager.GetEventHandler().onPlayerLifeChanged.Invoke();
            GameManager.GetEventHandler().onPlayerTakeHeal.Invoke();
        }
        
        public void HealPlayerFull()
        {
            _playerStats["CurrentHealth"] = _playerStats["MaxHealth"];
            GameManager.GetEventHandler().onPlayerLifeChanged.Invoke();
        }
        #endregion
        
        #region MaxHealth
        public int GetPlayerMaxHealth()
        {
            return _playerStats["MaxHealth"];
        }
        public void SetPlayerMaxLevel(int value)
        {
            _playerStats["MaxHealth"] = value;
            GameManager.GetEventHandler().onPlayerStatChanged.Invoke("MaxHealth");
        }
        #endregion
        
        #region AttackDamage
        public int GetPlayerDamage()
        {
            return _playerStats["AttackDamage"];
        }

        public void SetPlayerDamage(int value)
        {
            _playerStats["AttackDamage"] = value;
        }
        #endregion
        
        #region Speed

        public int GetPlayerSpeed()
        {
            return _playerStats["Speed"];
        }

        #endregion
        
        #region LightLevel
        public float GetPlayerLightLevel()
        {
            return _playerStats["CurrentLightLevel"] / (float) _playerStats["MaxLightLevel"];
        }
        public int GetPlayerCurrentLightValue()
        {
            return _playerStats["CurrentLightLevel"];
        }

        public int GetPlayerMaxLightValue()
        {
            return _playerStats["MaxLightLevel"];
        }
        #endregion
        
        
        
        public void KillPlayer()
        {
            GameManager.GetEventHandler().onPlayerDied.Invoke();
        }

        public Inventory GetInventory()
        {
            return _inventory;
        }

        
        
        
        //Playerstat default Getter/Setter
        public int GetPlayerStat(string key)
        {
            return _playerStats[key];
        }
        public int ChangePlayerStat(string key, int changeValue)
        {
            return _playerStats[key] += changeValue;
        }
        public int SetPlayerStat(string key, int setValue)
        {
            return _playerStats[key] += setValue;
        }

        
        public void OnItemChange(GameItem item, bool removed)
        {
            foreach (var itemStat in item.ValueIncreases)
            {
                if (removed)
                {
                    if (_playerStats.ContainsKey(itemStat.Key))
                    {
                        _playerStats[itemStat.Key] -= itemStat.Value;
                        GameManager.GetEventHandler().onPlayerStatChanged.Invoke(itemStat.Key);
                    }
                }
                else
                {
                    if (_playerStats.ContainsKey(itemStat.Key))
                    { 
                        _playerStats[itemStat.Key] += itemStat.Value;
                        GameManager.GetEventHandler().onPlayerStatChanged.Invoke(itemStat.Key);
                    }
                    else
                    {
                        _playerStats.Add(itemStat.Key, itemStat.Value);
                        GameManager.GetEventHandler().onPlayerStatChanged.Invoke(itemStat.Key);
                    }
                }
            }
        }
        
        

    }
}
 