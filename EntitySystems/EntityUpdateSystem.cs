using System.Collections.Generic;

namespace EngineeringCorpsCS
{
    class EntityUpdateSystem
    {
        public struct UpdateProperties
        {
            public System.Type type;
            public int updateFrequency;
            public UpdateProperties(System.Type type, int updateFrequency)
            {
                this.type = type;
                this.updateFrequency = updateFrequency;
            }
        }
        Dictionary<System.Type, List<Entity>> entities = new Dictionary<System.Type, List<Entity>>();
        Dictionary<System.Type, List<Entity>> activeEntities = new Dictionary<System.Type, List<Entity>>();
        List<Entity> destroyIndices = new List<Entity>();
        List<Entity> newEntities = new List<Entity>();
        UpdateProperties[] updateProperties;
        int[] updateCounters;

        public EntityUpdateSystem(UpdateProperties[] updateProperties)
        {
            this.updateProperties = updateProperties;
            updateCounters = new int[updateProperties.Length];
            for(int i = 0; i < updateProperties.Length; i++)
            {
                entities.Add(updateProperties[i].type, new List<Entity>());
                activeEntities.Add(updateProperties[i].type, new List<Entity>());
                updateCounters[i] = 1;
            }
        }

        /// <summary>
        /// Updates all active entities.  All active entities get a chance to update, even if they will be destroyed.
        /// </summary>
        public void UpdateEntities()
        {
            for(int i = 0; i < updateProperties.Length; i++)
            {
                if (updateCounters[i] > updateProperties[i].updateFrequency) //If the update counter is greater than update frequency, then update
                {
                    List<Entity> list = activeEntities[updateProperties[i].type];
                    for (int j = 0; j < list.Count; j++)
                    {
                        list[j].Update();
                    }
                    updateCounters[i] = 0; //reset update counter after updating
                }
                updateCounters[i]++;
            }
        }

        /// <summary>
        /// Finalizes the destruction of entites that were marked for destruction within update.
        /// </summary>
        public void DestroyEntities()
        {
            foreach(Entity e in destroyIndices)
            {
                //Assumption is that the entity was properly instantiated so we can use direct indexing
                //If it wasn't, then this code should crash the program anyway because unintended behavior is occurring
                activeEntities[e.GetType()].Remove(e);
                entities[e.GetType()].Remove(e);
            }
            destroyIndices.Clear();
        }

        /// <summary>
        /// Adds newly created entities to the list of entities and activeentities.  All new entities update at least once.
        /// </summary>
        public void AddNewEntities()
        {
            foreach(Entity e in newEntities)
            {
                List<Entity> list;
                //Only add the entity to active if it is in the system
                if(activeEntities.TryGetValue(e.GetType(), out list))
                {
                    list.Add(e);
                }
                //Add to full collection of entities even if not already present
                if (entities.TryGetValue(e.GetType(), out list))
                {
                    list.Add(e);
                }
                else
                {
                    list = new List<Entity>();
                    list.Add(e);
                    entities.Add(e.GetType(), list);
                }
            }
            newEntities.Clear();
        }

        public void AddEntity(Entity e)
        {
            newEntities.Add(e);
        }

        public void RemoveEntity(Entity e)
        {
            destroyIndices.Add(e);
        }
        
        /// <summary>
        /// Adds an already present entity to the entity list
        /// </summary>
        /// <param name="entity"></param>
        public void SetEntityToUpdate(Entity entity)
        {

        }
    }
}
