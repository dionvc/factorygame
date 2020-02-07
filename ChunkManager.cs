using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EngineeringCorpsCS
{
    //stores collection of chunks, manages saving, loading, and generating chunks
    //stores collection of active chunks (to run update on)
    
    class ChunkManager
    {
        Chunk[] chunks;
        List<Chunk> activeChunks;
        FastNoise elevationNoise;
        FastNoise moistureNoise;
        FastNoise temperatureNoise;

        public ChunkManager()
        {
            chunks = new Chunk[Props.worldSize * Props.worldSize];
            elevationNoise = new FastNoise();
            elevationNoise.SetNoiseType(FastNoise.NoiseType.Perlin);
            elevationNoise.SetSeed(new Random().Next(0, 10000));

            moistureNoise = new FastNoise();
            moistureNoise.SetNoiseType(FastNoise.NoiseType.Perlin);
            moistureNoise.SetSeed(new Random().Next(0, 10000));

            temperatureNoise = new FastNoise();
            temperatureNoise.SetNoiseType(FastNoise.NoiseType.Perlin);
            temperatureNoise.SetSeed(new Random().Next(0, 10000));
        }

        public void Update()
        {
            for(int i = 0; i < activeChunks.Count; i++)
            {
                activeChunks[i].Update();
            }
        }
        public void GenerateTerrain(int x, int y)
        {
            Chunk chunk = new Chunk();
            chunk.GenerateTerrain((x) * Props.chunkSize, (y) * Props.chunkSize, elevationNoise, moistureNoise, temperatureNoise);
            SetChunk(x, y, chunk);
        }
        public Chunk GetChunk(int x, int y)
        {
            if(x < 0 || x > Props.worldSize || y < 0 || y > Props.worldSize)
            {
                return null;
            }
            if (chunks[x * Props.worldSize + y] != null)
            {
                return chunks[x * Props.worldSize + y];
            }
            else
            {
                GenerateTerrain(x, y);
                return chunks[x * Props.worldSize + y];
            }
        }
        public void SetChunk(int x, int y, Chunk chunk)
        {
            chunks[x * Props.worldSize + y] = chunk;
        }







        /// <summary>
        /// Part of chunk streaming mechanism, saves inactive out of range chunks
        /// </summary>
        public void StreamChunkFromDisk()
        {

        }

        /// <summary>
        /// Part of chunk streaming mechanism, loads newly activated chunks
        /// </summary>
        public void StreamChunkToDisk()
        {

        }                       

        /// <summary>
        /// Converts world coordinates to chunk coordinates
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns>x,y chunk coords</returns>
        public static int[] WorldToChunkCoords(float x, float y)
        {
            return new int[] {(int) Math.Floor(x/(Props.tileSize * Props.chunkSize)), (int) Math.Floor(y/(Props.tileSize * Props.chunkSize))};
        }

        /// <summary>
        /// Converts chunk coordinates to world coordinates
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns>x,y world coords</returns>
        public static int[] ChunkToWorldCoords(int x, int y)
        {
            return new int[] {x * Props.tileSize * Props.chunkSize, y * Props.chunkSize};
        }

        /// <summary>
        /// Converts world coordinates to chunk coordinates
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns>tile x, tile y, chunk x, chunk y</returns>
        public static int[] WorldToTileCoords(float x, float y)
        {
            int xR = (int)Math.Floor(x/Props.tileSize);
            int yR = (int)Math.Floor(y/Props.tileSize);
            return new int[] { xR % Props.chunkSize, yR % Props.chunkSize, xR/Props.chunkSize, yR/Props.chunkSize };
        }

        public static int[] TileToWorldCoords(int x, int y, int cx, int cy)
        {
            return new int[] { (cx * Props.chunkSize + x) * Props.tileSize, (cy * Props.chunkSize + x) * Props.tileSize };
        }
    }
}
