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
        public int Lightshard { get; set; }
        public Inventory()
        {
            Items = new List<GameItem>();
            Lightshard = GameManager.GetSaveSystem().ShardsOnPlayer;
        }
        public void AddItem(GameItem item)
        {
            Items.Add(item);
        }
        
        public void RemoveItem(GameItem item)
        {
            Items.Remove(item);
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
