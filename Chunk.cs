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
        List<IEntity> chunkEntities;
        List<IEntity> chunkEntitiesActive;
        
        public Chunk()
        {
            terrain = new byte[32 * 32];
        }

        public void Update()
        {
            for(int i = 0; i < chunkEntitiesActive.Count; i++)
            {
                //Update entities.
            }
        }
        public void GenerateTerrain(int x, int y, FastNoise noise)
        {
            for (int i = 0; i < Props.chunkSize; i++) 
            {
                for(int j = 0; j < Props.chunkSize; j++)
                {
                    //generate terrain type in layers from bitflag (level select)
                    int nx = i + x;
                    int ny = j + y;
                    double elevation =  0.5 * noise.GetPerlin(1 * nx, 1 * ny)
                    +0.5 * noise.GetPerlin(2 * nx, 2 * ny)
                    +0.5 * noise.GetPerlin(4 * nx, 2 * ny);
                    elevation *= 4;
                    elevation += 0.5;
                    SetTile(i, j, Convert.ToByte(Math.Abs(elevation)));
                }
            }
        }
        public void SetTile(int x, int y, byte tile)
        {
            terrain[x * Props.chunkSize + y] = tile;
        }
        public byte GetTile(int x, int y)
        {
            return terrain[x * Props.chunkSize + y];
        }
    }
}
