using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFML.Graphics;

namespace EngineeringCorpsCS
{
    abstract class Entity : Base
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

        virtual public void InitializeEntity(Vector2 position, SurfaceContainer surface)
        {
            this.surface = surface;
            this.position = position;
            this.surface.InitiateEntityInChunks(this);
        }

        /// <summary>
        /// Used to enforce Updatability of entities
        /// </summary>
        /// <param name="entityCollection"></param>
        /// <param name="itemCollection"></param>
        virtual public void Update(EntityCollection entityCollection, ItemCollection itemCollection)
        {

        }
        /// <summary>
        /// Used to enforce mineability of entities
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

        abstract public Entity Clone();

        /// <summary>
        /// Tracks the chunks the entity is stored in for collision
        /// </summary>
        public int[] collisionChunks { get; set; }
        /// <summary>
        /// Tracks the chunk the entity is centered in
        /// </summary>
        public int centeredChunk { get; set; }
        /// <summary>
        /// Tracks the surface the entity is present on
        /// </summary>
        public SurfaceContainer surface { get; set; }

        /// <summary>
        /// Entity collision mask
        /// </summary>
        public CollisionLayer collisionMask { get; set; }

        /// <summary>
        /// The collision box representing the entity physically.
        /// </summary>
        public BoundingBox collisionBox { get; set; }

        /// <summary>
        /// The drawing collision box for the entity
        /// </summary>
        public BoundingBox drawingBox { get; set; }
        /// <summary>
        /// Array of drawables to be drawn representing the entity
        /// </summary>
        public Drawable[] drawArray { get; protected set; }
        /// <summary>
        /// The selectionbox of the entity
        /// </summary>
        public BoundingBox selectionBox { get; set; }
        public int tileWidth { get; set; } = 1;
        public int tileHeight { get; set; } = 1;
        public bool tileAligned { get; set; } = false;
        public float emissionPerSecond = 0.0f;
        public bool minable = false;
        public MiningProps miningProps;
        public string[] miningSounds = new string[] { "Pick1", "Pick2", "Pick3" };
        /// <summary>
        /// Color of entity on the map
        /// </summary>
        public Color mapColor { get; set; }

        public Vector2 position { get; set; }

    }
}
