using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EngineeringCorpsCS
{
    abstract class Entity
    {
        /// <summary>
        /// Describes the layer(s) on which the instance in question is collidable
        /// </summary>
        [Flags]
        enum CollisionLayer
        {
            None = 0,
            Terrain = 1,

            All = Terrain
        }
        /// <summary>
        /// Describes the layer on which the instance in question will be drawn
        /// </summary>
        enum DrawLayer
        {
            None = 0,
            Terrain = 1,
            Resource = 2,
            Item = 4,
            Shadow = 8,
            LayeredEntity = 16,
            Overlay = 32,
            Air = 64,
            IconOverlay = 128,
            GUI = 256
        }
        public string name { get; }
        public string type { get; }
        //Consider what should be passed to entity by chunk (perhaps the chunk coordinates?)
        public abstract void Update();
    }
}
