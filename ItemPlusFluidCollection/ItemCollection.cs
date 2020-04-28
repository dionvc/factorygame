using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EngineeringCorpsCS
{
    class ItemCollection
    {
        Dictionary<string, Item> itemCollection;
        ItemFactory itemFactory;

        public ItemCollection(TextureAtlases textureAtlases)
        {
            itemFactory = new ItemFactory(textureAtlases);
            itemCollection = itemFactory.GetItems();
        }

        public Item GetItem(string itemName)
        {
            return itemCollection[itemName];
        }
        
        public Dictionary<string, Item> GetItemCollection()
        {
            return itemCollection;
        }
    }
}
