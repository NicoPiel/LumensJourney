﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Unity.Burst;

namespace Assets.Items.Scripts
{
    [BurstCompile]
    public class GameItem
    {
    
        public Dictionary<string, int> ValueIncreasments {get; set;}
        public string ItemName {get; set;}
        public GameItem()
        {
            ValueIncreasments = new Dictionary<string, int>();
            ItemName = null;
        }
    
    
        public override bool Equals(object obj)
        {
            if (obj is GameItem)
            {
                GameItem other = (GameItem) obj;

                return other.ItemName == ItemName;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

    
    
    
    
    
    
        public static GameItem ConstructItem(string itemName)
        {
            GameItem item = new GameItem();
            XElement root = XElement.Load("Assets/Resources/Items/Data/items.xml");
            IEnumerable<XElement> items =
                from el in root.Elements("item")
                where (string) el.Attribute("name") == itemName
                select el;

        
            foreach (var itemData in items)
            {
                item.ItemName = itemName;
                var values = itemData.Elements("Values").Elements();

                foreach (var v in values)
                {
   
                    string attribute = v.Attribute("name").Value;
                    int val = Int32.Parse(v.Value);
                    item.ValueIncreasments.Add(attribute, val);
                }
            }
            return item;
        }

    }
}