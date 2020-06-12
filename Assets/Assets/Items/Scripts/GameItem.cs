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
            XElement rootNode = XElement.Load("Assets/Resources/Items/Data/items.xml");
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
                    item.ValueIncreasments.Add(attribute, val);
                }
            }
            
            return item;
        }

    }
}
