using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFML.Graphics;

namespace EngineeringCorpsCS
{
    abstract class EntityPhysical: Entity
    {
        
        /// <summary>
        /// The health of the entity.
        /// </summary>
        public int health
        {
            get => _health;
            set
            {
                if (health > fullHealth)
                {
                    _health = fullHealth;
                }
                else
                {
                    _health = value;
                }
            }
        }
        private int _health = 100;
        public int fullHealth { get; set; } = 100;


        
        string remainsMined;
        string remainsDestroyed;

        /// <summary>
        /// Used to create particles, play sounds, and create remains when an entity with health is destroyed
        /// </summary>
        virtual public void OnDestroyed()
        {
            surface.RemoveEntity(this);
        }

        /// <summary>
        /// Defines behavior of entity upon being clicked.  Example: Open a menu.
        /// </summary>
        virtual public void OnClick(Entity accessingEntity, MenuFactory menuFactory, RecipeCollection recipeCollection)
        {

        }
    }
}
