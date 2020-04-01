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
        /// TODO: What is this??
        /// </summary>
        public string type { get; protected set; }
        /// <summary>
        /// The collision mask representing the entity physically.
        /// </summary>
        public BoundingBox collisionBox { get; protected set; }
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

        public Vector2 position { get; protected set; }
        /// <summary>
        /// Defines behavior of entity upon being clicked.  Example: Open a menu.
        /// </summary>
        abstract public void OnClick();
    }
}
