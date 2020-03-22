using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFML.Graphics;

namespace EngineeringCorpsCS
{
    //store terrain data, list of entities(collision, rendering), and list of active entities (updating)
    //store pollution data
    class Chunk
    {

        private byte[] terrain;
        public List<Entity> entityList { get; protected set; } //Collection of entities for drawing (entities will be dynamically sorted)
        public List<Entity> entityCollisionList { get; protected set; } //Collection of entities for collision (2+ different chunks may contain the same entity)

        public Chunk()
        {
            entityList = new List<Entity>();
            entityCollisionList = new List<Entity>();
            terrain = new byte[Props.chunkSize * Props.chunkSize];
        }

        public void Update()
        {

        }
        public void GenerateTerrain(int x, int y, FastNoise elevationNoise, FastNoise moistureNoise, FastNoise temperatureNoise)
        {
            for (int i = 0; i < Props.chunkSize; i++) 
            {
                for(int j = 0; j < Props.chunkSize; j++)
                {
                    //generate terrain type in layers from bitflag (level select)
                    //elevation - defines where water could appear and preference for "higher" terrains
                    //moisture - defines moisture level for more wet biomes and trees versus dead trees
                    //rivers - could be generated from fractal noise?
                    int nx = i + x;
                    int ny = j + y;
                    double elevation =  0.5 * elevationNoise.GetPerlin(1 * nx, 1 * ny)
                    +0.5 * elevationNoise.GetPerlin(2 * nx, 2 * ny)
                    +0.5 * elevationNoise.GetPerlin(4 * nx, 4 * ny);
                    double moisture = 0.5 * moistureNoise.GetPerlin(1 * nx, 1 * ny)
                    + 0.5 * moistureNoise.GetPerlin(2 * nx, 2 * ny)
                    + 0.5 * moistureNoise.GetPerlin(4 * nx, 4 * ny);
                    double temperature = 0.5 * temperatureNoise.GetPerlin(1 * nx, 1 * ny)
                    + 0.5 * temperatureNoise.GetPerlin(2 * nx, 2 * ny)
                    + 0.5 * temperatureNoise.GetPerlin(4 * nx, 4 * ny);
                    elevation += 0.5;
                    moisture += 0.5;
                    temperature += 1;
                    temperature *= 3;
                    if (moisture > 0.5 && elevation < 1)
                    {
                        SetTile(i, j, 1);
                    }
                    else
                    {
                        SetTile(i, j, Convert.ToByte(Math.Abs(temperature)));
                    }
                }
            }
        }
        public void SetTile(int x, int y, byte tile)
        {
            terrain[x * Props.chunkSize + y] = tile;
        }

        public void SetTile(int tileIndex, byte tile)
        {
            terrain[tileIndex] = tile;
        }
        public byte GetTile(int x, int y)
        {
            return terrain[x * Props.chunkSize + y];
        }
        public byte GetTile(int tileIndex)
        {
            return terrain[tileIndex];
        }
        public byte[] GetTerrain()
        {
            return terrain;
        }

        public void AddEntityToChunk(Entity entity)
        {
            entityList.Add(entity);
        }
        public void AddEntityCollisionCheck(Entity entity)
        {
            entityCollisionList.Add(entity);
        }
        public void RemoveEntityFromChunk(Entity entity)
        {
            entityList.Remove(entity);
        }

        public void RemoveEntityCollisionCheck(Entity entity)
        {
            entityCollisionList.Remove(entity);
        }
    }
}
