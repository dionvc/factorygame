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
            item = CreateCoalOre();
            itemCollection.Add(item.name, item);
            item = CreateCopperOre();
            itemCollection.Add(item.name, item);
            item = CreateIronOre();
            itemCollection.Add(item.name, item);
            item = CreateFurnace();
            itemCollection.Add(item.name, item);
            return itemCollection;
        }

        //Define items prototypes here:
        private Item CreatePineSapling()
        {
            IntRect bounds;
            StaticSprite itemIcon = new StaticSprite(textureAtlases.GetTexture("woodItem", out bounds), bounds, Drawable.DrawLayer.Item);
            return new Item("Pine Sapling", itemIcon, "Pine Tree 1", 100);
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
            StaticSprite itemIcon = new StaticSprite(textureAtlases.GetTexture("greenhouseItem", out bounds), bounds, Drawable.DrawLayer.Item);
            return new Item("Greenhouse", itemIcon, "Greenhouse", 100);
        }

        private Item CreateCoalOre()
        {
            IntRect bounds;
            StaticSprite itemIcon = new StaticSprite(textureAtlases.GetTexture("CoalOreItem", out bounds), bounds, Drawable.DrawLayer.Item);
            return new Item("Coal Ore", itemIcon, null, 100);
        }

        private Item CreateIronOre()
        {
            IntRect bounds;
            StaticSprite itemIcon = new StaticSprite(textureAtlases.GetTexture("IronOreItem", out bounds), bounds, Drawable.DrawLayer.Item);
            return new Item("Iron Ore", itemIcon, null, 100);
        }

        private Item CreateCopperOre()
        {
            IntRect bounds;
            StaticSprite itemIcon = new StaticSprite(textureAtlases.GetTexture("CopperOreItem", out bounds), bounds, Drawable.DrawLayer.Item);
            return new Item("Copper Ore", itemIcon, null, 100);
        }

        private Item CreateFurnace()
        {
            IntRect bounds;
            StaticSprite itemIcon = new StaticSprite(textureAtlases.GetTexture("FurnaceItem", out bounds), bounds, Drawable.DrawLayer.Item);
            return new Item("Furnace", itemIcon, "Furnace", 50);
        }
    }
}
