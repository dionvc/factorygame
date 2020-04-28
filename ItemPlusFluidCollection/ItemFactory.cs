using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFML.Graphics;

namespace EngineeringCorpsCS
{
    class ItemFactory
    {
        Dictionary<string, Item> itemCollection;
        TextureAtlases textureAtlases;

        public ItemFactory(TextureAtlases textureAtlases)
        {
            itemCollection = new Dictionary<string, Item>();

            this.textureAtlases = textureAtlases;
        }

        
        public Dictionary<string, Item> GetItems()
        {
            Item item = CreatePineSapling();
            itemCollection.Add(item.name, item);
            item = CreateWood();
            itemCollection.Add(item.name, item);
            item = CreateGreenhouse();
            itemCollection.Add(item.name, item);
            return itemCollection;
        }

        //Define items prototypes here:
        private Item CreatePineSapling()
        {
            IntRect bounds;
            StaticSprite itemIcon = new StaticSprite(textureAtlases.GetTexture("woodItem", out bounds), bounds, Drawable.DrawLayer.Item);
            return new Item("Pine Sapling", itemIcon, "pineTree1", 100);
        }

        private Item CreateWood()
        {
            IntRect bounds;
            StaticSprite itemIcon = new StaticSprite(textureAtlases.GetTexture("woodItem", out bounds), bounds, Drawable.DrawLayer.Item);
            return new Item("Wood", itemIcon, null, 100);
        }

        private Item CreateGreenhouse()
        {
            IntRect bounds;
            StaticSprite itemIcon = new StaticSprite(textureAtlases.GetTexture("woodItem", out bounds), bounds, Drawable.DrawLayer.Item);
            return new Item("Greenhouse", itemIcon, "greenhouse", 100);
        }
    }
}
