using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EngineeringCorpsCS
{
    abstract class Entity : Base
    {
        public int[] collisionChunks { get; set; }
        public int centeredChunk { get; set; }
        public SurfaceContainer surface { get; set; }


        public string name { get; }
        public string type { get; }
        public BoundingBox collisionBox { get; set; }
        
        public Vector2 position { get; protected set; }
        //Consider what should be passed to entity by chunk (perhaps the chunk coordinates?)
        /// <summary>
        /// Defines behavior of entity upon being clicked.  Example: Open a menu.
        /// </summary>
        abstract public void OnClick();
    }
}
