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
        public EntityCollection(TextureAtlases textureAtlases)
        {
            entityPrototypes = new Dictionary<string, Entity>();
            this.textureAtlases = textureAtlases;
        }

        public void LoadPrototypes()
        {
            Entity playerPrototype = CreatePlayer();
            entityPrototypes.Add(playerPrototype.name, playerPrototype);
            Entity pineTree1Prototype = CreatePineTree1();
            entityPrototypes.Add(pineTree1Prototype.name, pineTree1Prototype);
        }

        public Entity InstantiatePrototype(string name, Vector2 position, SurfaceContainer surface)
        {
            Entity prototype;
            if(entityPrototypes.TryGetValue(name, out prototype))
            {
                Entity newEntity = prototype.Clone();
                newEntity.InitializeEntity(position, surface);
                return newEntity;
            }
            return null;
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

        private Entity CreatePlayer()
        {
            Player playerPrototype = new Player(textureAtlases, "player");
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
            pineTree1.miningProps = new Entity.MiningProps("Pine Sapling", 1, 320, 0, "");
            pineTree1.minable = true;
            pineTree1.collisionBox = new BoundingBox(16, 16);
            pineTree1.drawingBox = new BoundingBox(128, 192);
            return pineTree1;
        }
    }
}
