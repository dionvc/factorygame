using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFML.Graphics;

namespace EngineeringCorpsCS
{
    class Furnace: EntityPhysical
    {
        StaticSprite furnace;
        StaticSprite furnaceShadow;
        public Furnace(string name, StaticSprite furnaceShadow, StaticSprite furnace)
        {
            this.name = name;
            this.furnace = furnace;
            this.furnaceShadow = furnaceShadow;
            drawArray = new Drawable[] { furnaceShadow, furnace };
        }

        public override void Update(EntityCollection entityCollection, ItemCollection itemCollection)
        {
            
        }

        public override Entity Clone()
        {
            Furnace clone = new Furnace(this.name, this.furnaceShadow.Clone(), this.furnace.Clone());
            clone.drawingBox = new BoundingBox(this.drawingBox);
            clone.collisionBox = new BoundingBox(this.collisionBox);
            clone.selectionBox = new BoundingBox(this.selectionBox);
            clone.collisionMask = this.collisionMask;
            clone.minable = this.minable;
            clone.miningProps = this.miningProps;
            clone.mapColor = new Color(this.mapColor);
            clone.emissionPerSecond = this.emissionPerSecond;
            return clone;
        }
    }
}
