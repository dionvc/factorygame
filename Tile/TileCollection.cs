using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFML.Graphics;

namespace EngineeringCorpsCS
{
    class TileCollection
    {
        List<Tile> terrainTiles;
        public List<byte> impassableTileTypes { get; protected set; }
        List<Tile> terrainPathTiles;
        public TileCollection(TextureManager textureManager)
        {
            TileFactory tileFactory = new TileFactory(textureManager);
            terrainTiles = tileFactory.GetTerrainTiles();
            impassableTileTypes = new List<byte>();
            for(byte i = 0; i < terrainTiles.Count; i++)
            {
                if((terrainTiles[i].collisionMask & Base.CollisionLayer.TerrainSolid) == Base.CollisionLayer.TerrainSolid)
                {
                    impassableTileTypes.Add(i);
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
        public VertexArray[] GenerateTerrainVertexArray(ChunkManager chunkManager, int[] cXY)
        {
            VertexArray[] terrainVertexArray = new VertexArray[terrainTiles.Count];
            //Iterate over every tile type starting with the lowest layer (0)
            for(int i = 1; i < terrainTiles.Count; i++)
            {
                terrainVertexArray[i] = new VertexArray(PrimitiveType.Triangles);
                terrainTiles[i].AppendTerrainVertices(terrainVertexArray[i], chunkManager, cXY, impassableTileTypes);
            }
            return terrainVertexArray;
        }

        /// <summary>
        /// Gets the terrain texture tilesheets.  Required for drawing the constructed vertex arrays.
        /// </summary>
        /// <returns></returns>
        public Texture[] GetTerrainTilesheets()
        {
            Texture[] terrainTilesheet = new Texture[terrainTiles.Count];
            for(byte i = 0; i < terrainTiles.Count; i++)
            {
                terrainTilesheet[i] = terrainTiles[i].tileSheet;
            }
            return terrainTilesheet;
        }

        public string GetTileName(byte tileType)
        {
            if(tileType >= terrainTiles.Count)
            {
                return "Undefined";
            }
            return terrainTiles[tileType].name;
        }
    }
}
