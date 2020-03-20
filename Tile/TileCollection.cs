using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFML.Graphics;
using SFML.System;

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

        /// <summary>
        /// Generates the minimap vertexarray for a given chunk
        /// </summary>
        /// <param name="chunk"></param>
        /// <param name="chunkIndex"></param>
        /// <returns></returns>
        public VertexArray GenerateTerrainMinimap(Chunk chunk, int chunkIndex)
        {
            VertexArray vertexArray = new VertexArray(PrimitiveType.Triangles);
            if (chunk == null)
            {
                return vertexArray;
            }
            int oX = (chunkIndex / Props.worldSize) * Props.chunkSize;
            int oY = (chunkIndex % Props.worldSize) * Props.chunkSize;
            for (int i = 0; i < Props.chunkSize; i++)
            {
                for (int j = 0; j < Props.chunkSize; j++)
                {
                    byte tile = chunk.GetTile(i, j);
                    
                    if(tile >= terrainTiles.Count || tile < 0)
                    {
                        continue;
                    }
                    Color mapColor = terrainTiles[tile].mapColor;
                    vertexArray.Append(new Vertex(new Vector2f(oX + i, oY + j), mapColor));
                    vertexArray.Append(new Vertex(new Vector2f(oX + i + 1, oY + j), mapColor));
                    vertexArray.Append(new Vertex(new Vector2f(oX + i, oY + j + 1), mapColor));

                    vertexArray.Append(new Vertex(new Vector2f(oX + i + 1, oY + j), mapColor));
                    vertexArray.Append(new Vertex(new Vector2f(oX + i + 1, oY + j + 1), mapColor));
                    vertexArray.Append(new Vertex(new Vector2f(oX + i, oY + j + 1), mapColor));
                }
            }

            return vertexArray;
        }

        /// <summary>
        /// Generates the bounding box vertex array for a terrain using the map colors of the tile.
        /// </summary>
        /// <param name="chunk"></param>
        /// <param name="chunkIndex"></param>
        /// <param name="pointsTile"></param>
        /// <returns></returns>
        public VertexArray GenerateTerrainBoundingBoxArray(Chunk chunk, int chunkIndex, float[] pointsTile)
        {
            VertexArray vertexArray = new VertexArray(PrimitiveType.Lines);
            for (int k = 0; k < Props.chunkSize; k++)
            {
                for (int l = 0; l < Props.chunkSize; l++)
                {
                    Tile tile = GetTerrainTile(chunk.GetTile(k, l));
                    if ((tile.collisionMask & Base.CollisionLayer.TerrainSolid) != 0)
                    {
                        Vector2 position = SurfaceContainer.WorldToTileVector(chunkIndex, k * Props.chunkSize + l);
                        for (int x = 0; x < pointsTile.Length; x += 2)
                        {
                            vertexArray.Append(new Vertex(new Vector2f(pointsTile[x] + position.x, pointsTile[x + 1] + position.y), tile.mapColor));
                            vertexArray.Append(new Vertex(new Vector2f(pointsTile[(x + 2) % 8] + position.x, pointsTile[(x + 3) % 8] + position.y), tile.mapColor));
                        }
                    }
                }
            }
            return vertexArray;
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
