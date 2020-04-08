using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFML.Graphics;

namespace EngineeringCorpsCS
{
    /// <summary>
    /// An underlying item.  For each item variation, this should only be created once.
    /// Used to store item info in a lightweight way.
    /// </summary>
    class Item
    {
        string name;
        string placeResult;
        public StaticSprite itemSprite { get; protected set; }

        public Item(string name, StaticSprite itemSprite, string placeResult)
        {
            this.name = name;
            this.itemSprite = itemSprite;
            this.placeResult = placeResult;
        }
    }
}
