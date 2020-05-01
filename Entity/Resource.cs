using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EngineeringCorpsCS
{
    class Resource:Entity
    {
        int resourceCount = 100;
        StaticSprite resourceSprite;
        public Resource(string name, StaticSprite resource)
        {
            this.name = name;
            this.resourceSprite = resource;
            drawArray = new Drawable[] { resource };
        }
        public override void OnMined(Player player, ItemCollection itemCollection, EntityCollection entityCollection)
        {
            if (miningProps.results != null)
            {
                for (int i = 0; i < miningProps.results.Length; i++)
                {
                    player.InsertIntoInventory(new ItemStack(itemCollection.GetItem(miningProps.results[i]), miningProps.counts[i]), true);
                }
            }
            this.resourceCount--;
            if(this.resourceCount <= 0)
            {
                entityCollection.DestroyInstance(this);
            }
        }
        public override Entity Clone()
        {
            Resource clone = new Resource(this.name, this.resourceSprite.Clone());
            clone.drawingBox = new BoundingBox(this.drawingBox);
            clone.collisionBox = new BoundingBox(this.collisionBox);
            clone.selectionBox = new BoundingBox(this.selectionBox);
            clone.collisionMask = this.collisionMask;
            clone.minable = this.minable;
            clone.miningProps = this.miningProps;
            clone.tileAligned = this.tileAligned;
            clone.tileHeight = this.tileHeight;
            clone.tileWidth = this.tileWidth;
            clone.mapColor = new SFML.Graphics.Color(this.mapColor);
            return clone;
        }
    }
}
