using System.Collections.Generic;

namespace EngineeringCorpsCS.Util
{
    class EntityUpdateSystem
    {
        HashSet<Entity> entities = new HashSet<Entity>();
        HashSet<Entity> activeEntities = new HashSet<Entity>();
        HashSet<Entity> destroyIndices = new HashSet<Entity>();
        HashSet<Entity> newEntities = new HashSet<Entity>();

        /// <summary>
        /// Updates all active entities.  All active entities get a chance to update, even if they will be destroyed.
        /// </summary>
        public void UpdateEntities()
        {
            for(int i = 0; i < activeEntities.Count; i++)
            {
                //activeEntities[i].Update();
                //Have some logic to remove the active entity if its for destruction or doesn't need to be updated next frame
            }
        }

        /// <summary>
        /// Finalizes the destruction of entites that were marked for destruction within update.
        /// </summary>
        public void DestroyEntities()
        {
            foreach(Entity e in destroyIndices)
            {
                entities.Remove(e);
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
                entities.Add(e);
                activeEntities.Add(e);
            }
            newEntities.Clear();
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
