using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EngineeringCorpsCS
{
    static class Props
    {
        public const int chunkSize = 32; //size of chunk in tiles
        public const int worldSize = 2048; //size of world in chunks
        public const int tileSize = 32; //tilesize in pixels

        public static int startingSize = 8;
        public static float maxVelocity = 8.0f;
        public static int tileCollisionFactor = 1;
    }
}
