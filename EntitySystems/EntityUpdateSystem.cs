using System.Collections.Generic;

namespace EngineeringCorpsCS
{
    class UpdateProperties
    {
        public System.Type type;
        public int updateFrequency;
        public bool dontUpdate;
        public int updateCounter;
        public UpdateProperties(System.Type type, int updateFrequency, bool dontUpdate)
        {
            this.type = type;
            this.updateFrequency = updateFrequency;
            this.dontUpdate = dontUpdate;
        }
    }
    class EntityUpdateSystem
    {
        
        Dictionary<System.Type, List<Entity>> entities = new Dictionary<System.Type, List<Entity>>();
        Dictionary<System.Type, List<Entity>> activeEntities = new Dictionary<System.Type, List<Entity>>();
        List<Entity> destroyIndices = new List<Entity>();
        List<Entity> newEntities = new List<Entity>();
        Dictionary<System.Type, UpdateProperties> updateProperties;

        public EntityUpdateSystem(Dictionary<System.Type, UpdateProperties> updateProperties)
        {
            this.updateProperties = updateProperties;
            foreach(System.Type key in updateProperties.Keys)
            {
                entities.Add(updateProperties[key].type, new List<Entity>());
                activeEntities.Add(updateProperties[key].type, new List<Entity>());
                updateProperties[key].updateCounter = 0;
            }
        }

        public void ResetSystem()
        {
            entities.Clear();
            activeEntities.Clear();
            foreach (System.Type key in updateProperties.Keys)
            {
                entities.Add(updateProperties[key].type, new List<Entity>());
                activeEntities.Add(updateProperties[key].type, new List<Entity>());
                updateProperties[key].updateCounter = 0;
            }
        }

        /// <summary>
        /// Updates all active entities.  All active entities get a chance to update, even if they will be destroyed.
        /// </summary>
        public void UpdateEntities(EntityCollection entityCollection, ItemCollection itemCollection)
        {
            foreach(System.Type key in updateProperties.Keys) 
            {
                UpdateProperties current = updateProperties[key];
                if(current.dontUpdate == true)
                {
                    continue;
                }
                if (current.updateCounter > current.updateFrequency) //If the update counter is greater than update frequency, then update
                {
                    List<Entity> list = activeEntities[current.type];
                    for (int j = 0; j < list.Count; j++)
                    {
                        list[j].Update(entityCollection, itemCollection);
                    }
                    current.updateCounter = 0; //reset update counter after updating
                }
                current.updateCounter++;
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
                UpdateProperties current;
                if (updateProperties.TryGetValue(e.GetType(), out current))
                {
                    activeEntities[e.GetType()].Remove(e);
                }
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
