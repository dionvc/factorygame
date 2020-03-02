using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EngineeringCorpsCS
{
    abstract class Entity : Base
    {
        public string name { get; }
        public string type { get; }
        public BoundingBox collisionBox { get; protected set; }
        public int[] collisionChunks { get; set; }
        public int centeredChunk { get; set; }
        SurfaceContainer surface;
        //Consider what should be passed to entity by chunk (perhaps the chunk coordinates?)
    }
}
