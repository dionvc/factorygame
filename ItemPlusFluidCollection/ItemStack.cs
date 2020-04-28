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
        public int count { get; protected set; }

        public ItemStack(Item item, int count)
        {
            this.item = item;
            this.count = count;
        }

        public ItemStack Subtract(int count)
        {
            this.count -= count;
            if (this.count <= 0)
            {
                return null;
            }
            else
            {
                return this;
            }
        }

        /// <summary>
        /// Adds to item stack, if there was too much, then it returns the excess
        /// </summary>
        /// <param name="count"></param>
        /// <returns></returns>
        public int Add(int count)
        {
            this.count += count;
            if(this.count > item.maxStack)
            {
                int leftover = this.count - item.maxStack;
                this.count = item.maxStack;
                return leftover;
            }
            return 0;
        }

        public void SetCount(int count)
        {
            this.count = count;
        }
    }
}
