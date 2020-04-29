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
        public struct MiningProps
        {
            public string[] results;
            public int[] counts;
            public int miningTime;
            public int fluid;
            public string fluidRequired;

            public MiningProps(string[] results, int[] counts, int miningTime, int fluidCount, string fluidRequired)
            {
                this.results = results;
                this.counts = counts;
                this.miningTime = miningTime;
                this.fluid = fluidCount;
                this.fluidRequired = fluidRequired;
            }

            public MiningProps(string result, int count, int miningTime, int fluidCount, string fluidRequired)
            {
                this.results = new string[] { result };
                this.counts = new int[] { count };
                this.miningTime = miningTime;
                this.fluid = fluidCount;
                this.fluidRequired = fluidRequired;
            }
        }
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


        public bool minable = false;
        public MiningProps miningProps;
        string remainsMined;
        string remainsDestroyed;
        /// <summary>
        /// Color of entity on the map
        /// </summary>
        public Color mapColor { get; set; }
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

        /// <summary>
        /// Used to enforce mineability of objects
        /// </summary>
        virtual public void OnMined(Player player, ItemCollection itemCollection, EntityCollection entityCollection)
        {
            if (miningProps.results != null)
            {
                for (int i = 0; i < miningProps.results.Length; i++)
                {
                    player.InsertIntoInventory(new ItemStack(itemCollection.GetItem(miningProps.results[i]), miningProps.counts[i]), true);
                }
            }
            entityCollection.DestroyInstance(this);
        }
    }
}
