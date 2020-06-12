using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Unity.Burst;

namespace Assets.Items.Scripts
{
    [BurstCompile]
    public class GameItem
    {
    
        public Dictionary<string, int> ValueIncreases {get; set;}
        public string ItemName {get; set;}

        public const string PathToItemFile = "Assets/Assets/Items/Data/items.xml";
        
        public GameItem()
        {
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
            XElement rootNode = XElement.Load(PathToItemFile);
            var items =
                from xElement in rootNode.Elements("item")
                where (string) xElement.Attribute("name") == itemName
                select xElement;

        
            foreach (XElement itemData in items)
            {
                item.ItemName = itemName;
                var values = itemData.Elements("Values").Elements();

                foreach (XElement v in values)
                {
   
                    var attribute = v.Attribute("name").Value;
                    var val = int.Parse(v.Value);
                    item.ValueIncreases.Add(attribute, val);
                }
            }
            
            return item;
        }

        public static List<string> GetItemNames()
        {
            XElement rootNode = XElement.Load(PathToItemFile);
            var items =
                from xElement in rootNode.Elements("item")
                select xElement.Attribute("name")?.ToString();

            return items.ToList();
        }

    }
}
