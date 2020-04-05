using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EngineeringCorpsCS
{
    static class Props
    {
        //These constants are pretty vital to the functionality of the game and are justifiable as globals.
        //Graphics stuff
        public const int chunkSize = 32; //size of chunk in tiles
        public const int tileSize = 32; //tilesize in pixels
        public const int resolutionScaleFactor = 2; //The scale factor (resolution factor) of tiles and entities versus their actual size
        //Example: With tilesize of 32 and resolutionscalefactor of 2, tiles will have 64x64 pixel sizes but 32x32 world units size
        public const int vertexArrayCullingDistance = 32; //The distance in chunks where vertexarrays will be culled
        public const int overdrawDistance = 128;

        //Game and collision stuff
        public const int maxPollution = 255; //The max pollution count allowed for a chunk.
        public static float maxVelocity = 8.0f; //the max velocity an object can have before it is split into multiple checks
        public static int tileCollisionFactor = 1; //extra range of tiles around entities that are check for collisions
        public const int maxSurfaceSize = 8192;
    }
}
