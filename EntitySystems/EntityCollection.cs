using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFML.Graphics;
using SFML.System;

namespace EngineeringCorpsCS
{
    class EntityCollection
    {
        //construct kvp of names and entities which are then cloned and returned, it is up to the asker to initialize the cloned object
        Dictionary<string, Entity> entityPrototypes;
        TextureAtlases textureAtlases;
        EntityUpdateSystem entityUpdateSystem;
        public EntityCollection(TextureAtlases textureAtlases, EntityUpdateSystem entityUpdateSystem)
        {
            entityPrototypes = new Dictionary<string, Entity>();
            this.textureAtlases = textureAtlases;
            this.entityUpdateSystem = entityUpdateSystem;
        }

        public void LoadPrototypes(ItemCollection itemCollection)
        {
            Entity playerPrototype = CreatePlayer();
            entityPrototypes.Add(playerPrototype.name, playerPrototype);
            Entity pineTree1Prototype = CreatePineTree1();
            entityPrototypes.Add(pineTree1Prototype.name, pineTree1Prototype);
            Entity greenhousePrototype = CreateGreenhouse();
            entityPrototypes.Add(greenhousePrototype.name, greenhousePrototype);

            LoadItemEntityPrototypes(itemCollection);
        }

        /// <summary>
        /// Instantiates an entity and subscribes it to updating
        /// </summary>
        /// <param name="name"></param>
        /// <param name="position"></param>
        /// <param name="surface"></param>
        /// <returns></returns>
        public Entity InstantiatePrototype(string name, Vector2 position, SurfaceContainer surface)
        {
            Entity prototype;
            if(entityPrototypes.TryGetValue(name, out prototype))
            {
                Entity newEntity = prototype.Clone();
                //Initialize the Entity
                newEntity.InitializeEntity(position, surface);
                //Add to update system
                entityUpdateSystem.AddEntity(newEntity);
                return newEntity;
            }
            return null;
        }

        public void DestroyInstance(Entity entity)
        {
            //Remove the entity from the surface
            if(entity.surface != null)
            {
                entity.surface.RemoveEntity(entity);
                entity.surface = null;
            }
            //Remove the entity from updating
            entityUpdateSystem.RemoveEntity(entity);
        }

        /// <summary>
        /// Returns the actual prototype.  Recommended not to modify the returned entity (it will affect all future instances, which may be desirable).
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public Entity GetPrototype(string name)
        {
            Entity prototype;
            if(entityPrototypes.TryGetValue(name, out prototype))
            {
                return prototype;
            }
            return null;
        }

        #region Entity Definitions
        private void LoadItemEntityPrototypes(ItemCollection itemCollection)
        {
            //Iterate over itemcollection and create an itementity prototype for each in the collection
            Dictionary<string, Item> items = itemCollection.GetItemCollection();
            foreach(string key in items.Keys)
            {
                StaticSprite itemSprite = items[key].itemSprite.Clone();
                itemSprite.drawLayer = Drawable.DrawLayer.Item;
                Entity itemEntity = new EntityItem(key, itemSprite);
                itemEntity.selectionBox = new BoundingBox(32, 32);
                itemEntity.drawingBox = new BoundingBox(32, 32);
                itemEntity.collisionBox = new BoundingBox(32, 32);
                itemEntity.collisionMask = Base.CollisionLayer.Item | Base.CollisionLayer.EntityPhysical | Base.CollisionLayer.TerrainSolid;
                entityPrototypes.Add(key, itemEntity);
            }
        }
        private Entity CreatePlayer()
        {
            Player playerPrototype = new Player(textureAtlases, "player");
            playerPrototype.selectionBox = new BoundingBox(32, 32);
            return playerPrototype;
        }

        private Entity CreatePineTree1()
        {
            IntRect bounds;
            StaticSprite trunk = new StaticSprite(textureAtlases.GetTexture("tree", out bounds), new IntRect(bounds.Left, bounds.Top, bounds.Width/4, bounds.Height), new Vector2f(0, -64));
            trunk.drawLayer = Drawable.DrawLayer.EntitySorted;
            Animation leaves = new Animation(textureAtlases.GetTexture("tree", out bounds), 128, bounds.Height, 1, bounds, new Vector2f(0, -64));
            leaves.drawLayer = Drawable.DrawLayer.EntitySorted;
            Animation shadow = new Animation(textureAtlases.GetTexture("treeshadow", out bounds), 192, bounds.Height, 1, bounds, new Vector2f(32, 0));
            shadow.drawLayer = Drawable.DrawLayer.Shadow;
            Tree pineTree1 = new Tree("pineTree1", trunk, leaves, shadow);
            pineTree1.collisionMask = Base.CollisionLayer.EntityPhysical | Base.CollisionLayer.TerrainSolid;
            pineTree1.mapColor = new Color(32, 160, 0);
            pineTree1.miningProps = new EntityPhysical.MiningProps("Wood", 1, 90, 0, "");
            pineTree1.minable = true;
            pineTree1.collisionBox = new BoundingBox(16, 16);
            pineTree1.drawingBox = new BoundingBox(128, 192);
            pineTree1.selectionBox = new BoundingBox(32, 32);
            return pineTree1;
        }

        private Entity CreateGreenhouse()
        {
            IntRect bounds;
            Animation working = new Animation(textureAtlases.GetTexture("greenhouseworking", out bounds), bounds.Width, bounds.Height, 1, bounds, new Vector2f(0, -48));
            working.drawLayer = Drawable.DrawLayer.EntitySorted;
            StaticSprite idle = new StaticSprite(textureAtlases.GetTexture("greenhouse", out bounds), bounds, new Vector2f(0, -48));
            idle.drawLayer = Drawable.DrawLayer.EntitySorted;
            Animation shadow = new Animation(textureAtlases.GetTexture("greenhouseshadow", out bounds), bounds.Width, bounds.Height, 1, bounds, new Vector2f(96, -16));
            shadow.drawLayer = Drawable.DrawLayer.Shadow;
            Machine greenhouse = new Machine("greenhouse", working, idle, shadow);
            greenhouse.miningProps = new EntityPhysical.MiningProps("Greenhouse", 1, 60, 0, "");
            greenhouse.collisionMask = Base.CollisionLayer.EntityPhysical | Base.CollisionLayer.TerrainSolid;
            greenhouse.mapColor = new Color(96, 64, 0);
            greenhouse.minable = true;
            greenhouse.collisionBox = new BoundingBox(-85, -80, 85, 48);
            greenhouse.drawingBox = new BoundingBox(192, 192);
            greenhouse.selectionBox = new BoundingBox(-96, -112, 96, 48);
            greenhouse.tileAligned = true;
            return greenhouse;
        }

        #endregion Entity Definitions
    }
}
