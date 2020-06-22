using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using Core;
using Unity.Burst;
using UnityEngine;

namespace Assets.Items.Scripts
{
    [BurstCompile]
    public class GameItem
    {
    
        public Dictionary<string, int> ValueIncreases {get; set;}
        public string ItemName {get; set;}
        
        public static string persistentPathToItemFile;
        
        public GameItem()
        {
            persistentPathToItemFile = GameManager.persistentItemFilePath;
            
            ValueIncreases = new Dictionary<string, int>();
            ItemName = null;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is GameItem)) return false;
            
            var other = (GameItem) obj;
            return other.ItemName == ItemName;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
        
        public static GameItem ConstructItem(string itemName)
        {
            var item = new GameItem();
            XElement rootNode = XElement.Load(persistentPathToItemFile);
            var items =
                from xElement in rootNode.Elements("item")
                where xElement.Attribute("name")?.Value == itemName
                select xElement;

            foreach (XElement itemData in items)
            {
                item.ItemName = itemName;
                var values = itemData.Elements("Values").Elements();

                foreach (XElement v in values)
                {
                    XAttribute queryName = v.Attribute("name");

                    if (queryName == null) continue;
                    
                    var attribute = queryName.Value;
                    var val = int.Parse(v.Value);
                    item.ValueIncreases.Add(attribute, val);
                }
            }
            
            return item;
        }

        public static List<string> GetItemNames()
        {
            XElement rootNode = XElement.Load(persistentPathToItemFile);
            var items =
                from xElement in rootNode.Elements("item")
                select xElement.Attribute("name")?.ToString();

            return items.ToList();
        }

    }
}
