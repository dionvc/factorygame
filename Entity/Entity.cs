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

        virtual public void Update()
        {

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
        public BoundingBox collisionBox { get; protected set; }

        /// <summary>
        /// The drawing collision box for the entity
        /// </summary>
        public BoundingBox drawingBox { get; protected set; }
        /// <summary>
        /// Array of drawables to be drawn representing the entity
        /// </summary>
        public Drawable[] drawArray { get; protected set; }
        /// <summary>
        /// Color of entity on the map
        /// </summary>
        public Color mapColor { get; protected set; }

        public int tileWidth { get; protected set; } = 2;
        public int tileHeight { get; protected set; } = 2;

        public float emissionPerSecond = 0.0f;

        public bool minable = false;
        public MiningProps miningProps;

        public Vector2 position { get; set; }
        /// <summary>
        /// Defines behavior of entity upon being clicked.  Example: Open a menu.
        /// </summary>
        abstract public void OnClick();

        /// <summary>
        /// Used to enforce mineability of objects
        /// </summary>
        virtual public void OnMined()
        {
            surface.RemoveEntity(this);
        }
    }
}
