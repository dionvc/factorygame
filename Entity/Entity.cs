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
        virtual public void InitializeEntity(Vector2 position, SurfaceContainer surface)
        {
            this.surface = surface;
            this.position = position;
            this.surface.InitiateEntityInChunks(this);
        }

        virtual public void Update(EntityCollection entityCollection, ItemCollection itemCollection)
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

        public Vector2 position { get; set; }

    }
}
