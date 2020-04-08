using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EngineeringCorpsCS
{
    class ItemFactory
    {
        Dictionary<string, Item> itemCollection;

        public ItemFactory()
        {
            itemCollection = new Dictionary<string, Item>();
        }

        
        public Dictionary<string, Item> GetItems()
        {
            //Add items to dictionary here
            return itemCollection;
        }

        //Define items prototypes here:
    }
}
