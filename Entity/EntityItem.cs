using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EngineeringCorpsCS
{
    class EntityItem: Entity
    {
        StaticSprite itemSprite;
        public string itemName;
        public EntityItem(string name, string itemName, StaticSprite sprite)
        {
            this.name = name;
            this.itemName = itemName;
            itemSprite = sprite;
            drawArray = new Drawable[] { sprite };
        }
        public override Entity Clone()
        {
            EntityItem clone = new EntityItem(this.name, this.itemName, itemSprite.Clone());
            clone.selectionBox = this.selectionBox;
            clone.drawingBox = this.drawingBox;
            clone.collisionBox = this.collisionBox;
            clone.collisionMask = Base.CollisionLayer.Item;
            return clone;
        }
    }
}
