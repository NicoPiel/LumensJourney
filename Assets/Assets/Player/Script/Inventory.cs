using System.Collections.Generic;
using Assets.ItemBar;
using Assets.ItemBar.Scripts;
using Assets.Items.Scripts;
using Unity.Burst;

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
  
        }
        public void AddItem(GameItem item)
        {
            Items.Add(item);
            ItemBarScript.UpdateItemBar_Static(this);
        }
        public void AddItem(string item)
        {
            Items.Add(GameItem.ConstructItem(item));
            ItemBarScript.UpdateItemBar_Static(this);
        }
        public void RemoveItem(GameItem item)
        {
            Items.Remove(item);
        }
        public void RemoveItem(string item)
        {
            Items.Remove(GameItem.ConstructItem(item));
        }
    
        public GameItem GetItem(string itemname)
        {
            return Items.Find(item => item.ItemName == itemname);
        }
    

    }
}
