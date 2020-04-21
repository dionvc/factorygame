using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            Tree pineTree1 = new Tree(textureAtlases, "pineTree1");
            pineTree1.collisionMask = Base.CollisionLayer.EntityPhysical | Base.CollisionLayer.TerrainSolid;
            return pineTree1;
        }
    }
}
