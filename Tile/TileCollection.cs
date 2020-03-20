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
        List<Tile> terrainTiles; //list of terrainTiles
        List<Tile> terrainPathTiles; //list of placable paths,etc
        List<byte> impassableTileTypes; //specifically for rendering do not use for anything else
        
        public TileCollection(TextureContainer textureManager)
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
        public VertexArray[] GenerateTerrainVertexArray(SurfaceContainer surface, int[] cXY)
        {
            VertexArray[] terrainVertexArray = new VertexArray[terrainTiles.Count];
            //Iterate over every tile type starting with the lowest layer (0)
            for(int i = 1; i < terrainTiles.Count; i++)
            {
                terrainVertexArray[i] = new VertexArray(PrimitiveType.Triangles);
                terrainTiles[i].AppendTerrainVertices(terrainVertexArray[i], surface, cXY, impassableTileTypes);
            }
            return terrainVertexArray;
        }

        public Image GenerateTerrainMinimap(SurfaceContainer surface, int chunkIndex)
        {
            Image minimapImage = new Image(32, 32, Color.Black);
            for (int i = 1; i < terrainTiles.Count; i++)
            {
                terrainTiles[i].GenerateMinimapImage(minimapImage, surface, chunkIndex);
            }
            return minimapImage;
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

        /// <summary>
        /// Gets the string name of the terrain tile with specified tiletype
        /// </summary>
        /// <param name="tileType"></param>
        /// <returns></returns>
        public string GetTerrainTileName(byte tileType)
        {
            //TODO: should not be needed in properly generated world, remove when map generator is functional to cut down on processing
            if(tileType >= terrainTiles.Count)
            {
                return "Undefined";
            }
            return terrainTiles[tileType].name;
        }

        public Tile GetTerrainTile(byte tileType)
        {
            //TODO: should not be needed in properly generated world, remove when map generator is functional to cut down on processing
            if (tileType >= terrainTiles.Count)
            {
                return terrainTiles[0]; //return void in case of invalid terrain tile type
            }
            return terrainTiles[tileType];
        }
    }
}
