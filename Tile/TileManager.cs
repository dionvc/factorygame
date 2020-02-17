using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFML.Graphics;

namespace EngineeringCorpsCS
{
    class TileManager
    {
        Tile[] terrainTiles;
        byte[] impassableTileTypes;
        Tile[] terrainPathTiles;
        public TileManager(TextureManager textureManager)
        {
            TileFactory tileFactory = new TileFactory(textureManager);
            terrainTiles = tileFactory.GetTerrainTiles();
            for(byte i = 0; i < terrainTiles.Length; i++)
            {
                if(terrainTiles[i].collisionMask == Base.CollisionLayer.TerrainSolid)
                {
                    impassableTileTypes.Append(i);
                }
            }
            terrainPathTiles = tileFactory.GetTerrainPathTiles();
        }

        /// <summary>
        /// Generates all the necessary vertex arrays for drawing a chunk
        /// </summary>
        /// <param name="chunk"></param>
        /// <param name="cXY"></param>
        /// <returns></returns>
        public VertexArray[] GenerateTerrainVertexArray(Chunk chunk, int[] cXY)
        {
            byte[] terrain = chunk.GetTerrain();
            VertexArray[] terrainVertexArray = new VertexArray[terrainTiles.Length];
            //Iterate over every tile type starting with the lowest layer (0)
            for(int i = 0; i < terrainTiles.Length; i++)
            {
                terrainVertexArray[i] = new VertexArray(PrimitiveType.Triangles, 4);
                terrainTiles[i].AppendTerrainVertices(terrainVertexArray[i], terrain, cXY, impassableTileTypes);
            }
            return terrainVertexArray;
        }

        /// <summary>
        /// Gets the terrain texture tilesheets.  Required for drawing the constructed vertex arrays.
        /// </summary>
        /// <returns></returns>
        public Texture[] GetTerrainTilesheets()
        {
            Texture[] terrainTilesheet = new Texture[terrainTiles.Length];
            for(byte i = 0; i < terrainTiles.Length; i++)
            {
                
            }
            return terrainTilesheet;
        }
    }
}
