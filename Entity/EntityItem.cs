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
        public EntityItem(string name, StaticSprite sprite)
        {
            this.name = name;
            itemSprite = sprite;
            drawArray = new Drawable[] { sprite };
        }
        public override Entity Clone()
        {
            EntityItem clone = new EntityItem(this.name, itemSprite.Clone());
            clone.selectionBox = this.selectionBox;
            clone.drawingBox = this.drawingBox;
            clone.collisionBox = this.collisionBox;
            clone.collisionMask = Base.CollisionLayer.Item;
            return clone;
        }
    }
}
