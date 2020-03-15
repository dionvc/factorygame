using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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


        public string type { get; }
        public BoundingBox collisionBox { get; set; }

        public Drawable[] drawArray;
        
        public Vector2 position { get; protected set; }
        //Consider what should be passed to entity by chunk (perhaps the chunk coordinates?)
        /// <summary>
        /// Defines behavior of entity upon being clicked.  Example: Open a menu.
        /// </summary>
        abstract public void OnClick();
    }
}
