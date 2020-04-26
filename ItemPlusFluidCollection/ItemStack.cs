using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EngineeringCorpsCS
{
    /// <summary>
    /// Represents a stack of items for inventories (players, machines) with a pointer to the underlying item
    /// </summary>
    class ItemStack
    {
        /// <summary>
        /// The underlying item
        /// </summary>
        public Item item { get; protected set; }
        public int count { get; set; }

        public ItemStack(Item item, int count)
        {
            this.item = item;
            this.count = count;
        }
    }
}
