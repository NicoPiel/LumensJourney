using System.Collections.Generic;
using Assets.Items.Scripts;
using Assets.UI.ItemBar.Scripts;
using Core;
using Unity.Burst;
using UnityEditor;

namespace Assets.Player.Script
{
    [BurstCompile]
    public class Inventory
    {
        public List<GameItem> Items { get; set;}
        private int _lightshard;
        public Inventory()
        {
            Items = new List<GameItem>();
            _lightshard = GameManager.GetSaveSystem().ShardsOnPlayer;
            
            GameManager.GetEventHandler().onPlayerLightShardsChanged.AddListener(OnPlayerLightShardsChanged);
        }
        public void AddItem(GameItem item)
        {
            Items.Add(item);
            GameManager.GetEventHandler().onItemAddedToInventory.Invoke(item);
            GameManager.GetEventHandler().onInventoryChanged.Invoke();
        }
        
        public void RemoveItem(GameItem item)
        {
            Items.Remove(item);
        }
        
        //LightShards
        public int GetLightShardAmount()
        {
            return _lightshard;
        }
        public void PlayerSetLightShards(int lightShards)
        {
            _lightshard = lightShards;
            GameManager.GetEventHandler().onPlayerLightShardsChanged.Invoke();
        }
        public void PlayerChangeLightShards(int lightShards)
        {
            _lightshard += lightShards;
            GameManager.GetEventHandler().onPlayerLightShardsChanged.Invoke();
        }
        private void OnPlayerLightShardsChanged()
        {
            GameManager.GetSaveSystem().ShardsOnPlayer = GetLightShardAmount();
        }
        
        
        

        public GameItem GetItem(string itemname)
        {
            return Items.Find(item => item.ItemName == itemname);
        }

        public bool IsEmpty()
        {
            return Items.Count == 0;
        }
    }
}
