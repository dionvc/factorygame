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
        public void GenerateTerrain(int x, int y, SurfaceGenerator surfaceGenerator)
        {
            for (int i = 0; i < Props.chunkSize; i++) 
            {
                for(int j = 0; j < Props.chunkSize; j++)
                {
                    int nx = i + x;
                    int ny = j + y;
                    SetTile(i, j, surfaceGenerator.GetTile(nx, ny));
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
