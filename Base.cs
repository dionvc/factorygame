using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EngineeringCorpsCS
{
    class Base
    {

        /// <summary>
        /// Describes the layer on which the instance in question will be drawn
        /// </summary>
        public enum DrawLayer
        {
            None = 0,
            Terrain = 1,
            Resource = 2,
            Item = 3,
            LayeredEntity = 4,
            Overlay = 5,
            Air = 6,
            IconOverlay = 7,
            GUI = 8
        }

        /// <summary>
        /// Describes the layer(s) on which the instance in question is collidable
        /// </summary>
        [Flags]
        enum CollisionLayer
        {
            None = 0,
            Terrain = 1,
            Resource = 2,
            Item = 4,

            All = Terrain
        }
    }
}
