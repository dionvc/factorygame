using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EngineeringCorpsCS.ItemPlusFluidCollection
{
    class ItemCollection
    {
        Dictionary<string, Item> itemCollection;
        ItemFactory itemFactory;

        public ItemCollection()
        {
            itemFactory = new ItemFactory();
            itemCollection = itemFactory.GetItems();
        }

        public Item GetItem(string itemName)
        {
            return itemCollection[itemName];
        }
    }
}
