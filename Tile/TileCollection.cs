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
        Texture cliffTilesheet;
        Color cliffShade = new Color(255, 255, 255);

        public TileCollection(TextureContainer textureManager)
        {
            TileFactory tileFactory = new TileFactory(textureManager);
            cliffTilesheet = textureManager.GetTexture("cliffTilesheet");
            terrainTiles = tileFactory.GetTerrainTiles();
            impassableTileTypes = new List<byte>();
            for (byte i = 0; i < terrainTiles.Count; i++)
            {
                if ((terrainTiles[i].collisionMask & Base.CollisionLayer.TerrainSolid) == Base.CollisionLayer.TerrainSolid)
                {
                    impassableTileTypes.Add(i);
                }
            }
            terrainPathTiles = tileFactory.GetTerrainPathTiles();
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

                    if (tile >= terrainTiles.Count || tile < 0)
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

        public VertexArray[] GenerateTerrainVertices(SurfaceContainer surface, int[] pos)
        {
            VertexArray[] terrainVertexArray = new VertexArray[terrainTiles.Count];
            terrainVertexArray[0] = GenerateCliffVertices(surface, pos);
            for (int curType = 1; curType < terrainTiles.Count; curType++)
            {
                Tile tile = terrainTiles[curType];
                terrainVertexArray[curType] = new VertexArray(PrimitiveType.Triangles);
                int[] cXY = new int[] { pos[0] * Props.chunkSize, pos[1] * Props.chunkSize };
                for (int i = 0; i < Props.chunkSize; i++)
                {
                    for (int j = 0; j < Props.chunkSize; j++)
                    {

                        //the current tile in the loop
                        byte currentTile = surface.GetTileFromWorldInt(pos, i, j);
                        if (currentTile > curType)
                        {
                            continue;
                        }
                        //hash algo
                        int variantX = i * 266489917 + 374761393;
                        variantX = (variantX << 17) | (variantX >> 15);
                        variantX += j * 266489917;
                        variantX *= 668265263;
                        variantX ^= variantX >> 15;
                        variantX *= 246822519;
                        variantX = Math.Abs(variantX);
                        //origin that vertices will be defined relative to
                        int oX = (cXY[0] + i) * Props.tileSize;
                        int oY = (cXY[1] + j) * Props.tileSize;

                        #region Regular tile variant determination
                        if (currentTile == curType)
                        {
                            int variantRegular = variantX % 8;
                            //Append regular tile variation
                            AppendQuadVertices(terrainVertexArray[curType], oX, oY, 0, 0, variantRegular, tile.shade);
                            continue;
                        }
                        #endregion
                        int value = 0;
                        int variant = variantX % 6; //variant is determine by whether the transition is into an impassable tile
                        if (currentTile != 0 && impassableTileTypes.Contains(currentTile))
                        {
                            variant += 6;
                        }

                        value += (surface.GetTileFromWorldInt(pos, i - 1, j - 1) == curType) ? 1 : 0; //top left
                        value += (surface.GetTileFromWorldInt(pos, i, j - 1) == curType) ? 2 : 0; //top center
                        value += (surface.GetTileFromWorldInt(pos, i + 1, j - 1) == curType) ? 4 : 0; //top right
                        value += (surface.GetTileFromWorldInt(pos, i - 1, j) == curType) ? 8 : 0; //left
                        value += (surface.GetTileFromWorldInt(pos, i + 1, j) == curType) ? 16 : 0; //right
                        value += (surface.GetTileFromWorldInt(pos, i - 1, j + 1) == curType) ? 32 : 0; //bottom left
                        value += (surface.GetTileFromWorldInt(pos, i, j + 1) == curType) ? 64 : 0; //bottom center
                        value += (surface.GetTileFromWorldInt(pos, i + 1, j + 1) == curType) ? 128 : 0; //bottom right
                        if (value == 0)
                        {
                            continue; //no surrounding tiles of tiletype
                        }
                        if (((value & 24) + (value & 2)) == 2)
                        {
                            //top fade
                            AppendQuadVertices(terrainVertexArray[curType], oX, oY, 0, 2, variant, tile.shade);
                        }
                        if (((value & 24) + (value & 64)) == 64)
                        {
                            //bottom fade
                            AppendQuadVertices(terrainVertexArray[curType], oX, oY, 12, 2, variant, tile.shade);
                        }
                        if (((value & 66) + (value & 8)) == 8)
                        {
                            //left fade
                            AppendQuadVertices(terrainVertexArray[curType], oX, oY, 0, 3, variant, tile.shade);
                        }
                        if (((value & 66) + (value & 16)) == 16)
                        {
                            //right fade
                            AppendQuadVertices(terrainVertexArray[curType], oX, oY, 12, 3, variant, tile.shade);
                        }
                        if ((value & 10) == 10)
                        {
                            //inside corner topleft
                            AppendQuadVertices(terrainVertexArray[curType], oX, oY, 0, 4, variant, tile.shade);
                        }
                        else if (((value & 1) + (value & 10)) == 1)
                        {
                            //outside corner topleft
                            AppendQuadVertices(terrainVertexArray[curType], oX, oY, 0, 6, variant, tile.shade);
                        }
                        if ((value & 18) == 18)
                        {
                            //inside corner topright
                            AppendQuadVertices(terrainVertexArray[curType], oX, oY, 12, 4, variant, tile.shade);
                        }
                        else if (((value & 4) + (value & 18)) == 4)
                        {
                            //outside corner topright
                            AppendQuadVertices(terrainVertexArray[curType], oX, oY, 12, 6, variant, tile.shade);
                        }
                        if ((value & 72) == 72)
                        {
                            //inside corner bottomleft
                            AppendQuadVertices(terrainVertexArray[curType], oX, oY, 0, 5, variant, tile.shade);
                        }
                        else if (((value & 32) + (value & 72)) == 32)
                        {
                            //outside corner bottomleft
                            AppendQuadVertices(terrainVertexArray[curType], oX, oY, 0, 7, variant, tile.shade);
                        }
                        if ((value & 80) == 80)
                        {
                            //inside corner bottomright
                            AppendQuadVertices(terrainVertexArray[curType], oX, oY, 12, 5, variant, tile.shade);
                        }
                        else if (((value & 128) + (value & 80)) == 128)
                        {
                            //outside corner bottomright
                            AppendQuadVertices(terrainVertexArray[curType], oX, oY, 12, 7, variant, tile.shade);
                        }
                    }
                }
            }
            return terrainVertexArray;
        }

        private VertexArray GenerateCliffVertices(SurfaceContainer surface, int[] pos)
        {
            VertexArray vA = new VertexArray(PrimitiveType.Triangles);
            int[] cXY = new int[] { pos[0] * Props.chunkSize, pos[1] * Props.chunkSize };
            for (int i = 0; i < Props.chunkSize; i++)
            {
                for (int j = 0; j < Props.chunkSize; j++)
                {
                    //the current tile in the loop
                    byte currentTile = surface.GetTileFromWorldInt(pos, i, j);
                    #region Skip tile determination
                    if (currentTile != 0)
                    {
                        continue;
                    }
                    #endregion
                    //hash algo
                    int variantX = i * 266489917 + 374761393;
                    variantX = (variantX << 17) | (variantX >> 15);
                    variantX += j * 266489917;
                    variantX *= 668265263;
                    variantX ^= variantX >> 15;
                    variantX *= 246822519;
                    variantX = Math.Abs(variantX);
                    //origin that vertices will be defined relative to
                    int oX = (cXY[0] + i) * Props.tileSize;
                    int oY = (cXY[1] + j) * Props.tileSize;

                    int value = 0;
                    int variant = variantX % 4; //variant is determine by whether the transition is into an impassable tile

                    value += (surface.GetTileFromWorldInt(pos, i - 1, j - 1) != 0) ? 1 : 0; //top left
                    value += (surface.GetTileFromWorldInt(pos, i, j - 1) != 0) ? 2 : 0; //top center
                    value += (surface.GetTileFromWorldInt(pos, i + 1, j - 1) != 0) ? 4 : 0; //top right
                    value += (surface.GetTileFromWorldInt(pos, i - 1, j) != 0) ? 8 : 0; //left
                    value += (surface.GetTileFromWorldInt(pos, i + 1, j) != 0) ? 16 : 0; //right
                    value += (surface.GetTileFromWorldInt(pos, i - 1, j + 1) != 0) ? 32 : 0; //bottom left
                    value += (surface.GetTileFromWorldInt(pos, i, j + 1) != 0) ? 64 : 0; //bottom center
                    value += (surface.GetTileFromWorldInt(pos, i + 1, j + 1) != 0) ? 128 : 0; //bottom right

                    //Virtual mapping of surrounding tiles
                    // 1 2 3
                    // 4 0 5
                    // 6 7 8
                    //bitflag values of surrounding tiles
                    // 1  2  4
                    // 8  0  16
                    // 32 64 128

                    //need specific checks for if left and right exist simultaneously
                    //if top and bottom exist simultaneously
                    //if two corners exist (u shape) in any config
                    //if four corners exist in any config

                    if (value == 0)
                    {
                        continue; //no surrounding tiles of tiletype
                    }
                    if (((value & 24) + (value & 2)) == 2)
                    {
                        //top fade
                        AppendQuadVerticesCliff(vA, oX, oY, 0, 0, variant, cliffShade);
                    }
                    if (((value & 24) + (value & 64)) == 64)
                    {
                        //bottom fade
                        AppendQuadVerticesCliff(vA, oX, oY, 4, 0, variant, cliffShade);
                    }
                    if (((value & 66) + (value & 8)) == 8)
                    {
                        //left fade
                        AppendQuadVerticesCliff(vA, oX, oY, 8, 0, variant, cliffShade);
                    }
                    if (((value & 66) + (value & 16)) == 16)
                    {
                        //right fade
                        AppendQuadVerticesCliff(vA, oX, oY, 12, 0, variant, cliffShade);
                    }
                    if ((value & 10) == 10)
                    {
                        //inside corner topleft
                        AppendQuadVerticesCliff(vA, oX, oY, 0, 1, variant, cliffShade);
                    }
                    else if (((value & 1) + (value & 10)) == 1)
                    {
                        //outside corner topleft
                        AppendQuadVerticesCliff(vA, oX, oY, 0, 2, variant, cliffShade);
                    }
                    if ((value & 18) == 18)
                    {
                        //inside corner topright
                        AppendQuadVerticesCliff(vA, oX, oY, 4, 1, variant, cliffShade);
                    }
                    else if (((value & 4) + (value & 18)) == 4)
                    {
                        //outside corner topright
                        AppendQuadVerticesCliff(vA, oX, oY, 4, 2, variant, cliffShade);
                    }
                    if ((value & 72) == 72)
                    {
                        //inside corner bottomleft
                        AppendQuadVerticesCliff(vA, oX, oY, 8, 1, variant, cliffShade);
                    }
                    else if (((value & 32) + (value & 72)) == 32)
                    {
                        //outside corner bottomleft
                        AppendQuadVerticesCliff(vA, oX, oY, 8, 2, variant, cliffShade);
                    }
                    if ((value & 80) == 80)
                    {
                        //inside corner bottomright
                        AppendQuadVerticesCliff(vA, oX, oY, 12, 1, variant, cliffShade);
                    }
                    else if (((value & 128) + (value & 80)) == 128)
                    {
                        //outside corner bottomright
                        AppendQuadVerticesCliff(vA, oX, oY, 12, 2, variant, cliffShade);
                    }
                }
            }
            return vA;
        }

        /// <summary>
        /// Helper function for appending terrain vertices.  May actually increase code execution time, hopefully C# inlines it :(
        /// </summary>
        /// <param name="vA"></param>
        /// <param name="oX"></param>
        /// <param name="oY"></param>
        /// <param name="texOX"></param>
        /// <param name="texOY"></param>
        /// <param name="variant"></param>
        /// <param name="shade"></param>
        private void AppendQuadVertices(VertexArray vA, int oX, int oY, int texOX, int texOY, int variant, Color shade)
        {
            //Pixel correction offset, fixes issue #2 on github
            float pR = 0.5f;
            //first triangle
            vA.Append(new Vertex(new Vector2f(oX - pR, oY - pR), shade,
                new Vector2f((texOX + variant) * Props.tileSize * Props.resolutionScaleFactor, texOY * Props.tileSize * Props.resolutionScaleFactor))); //top left

            vA.Append(new Vertex(new Vector2f(oX + Props.tileSize + pR, oY - pR), shade,
                new Vector2f((texOX + variant + 1) * Props.tileSize * Props.resolutionScaleFactor, texOY * Props.tileSize * Props.resolutionScaleFactor))); //top right

            vA.Append(new Vertex(new Vector2f(oX - pR, oY + Props.tileSize + pR), shade,
                new Vector2f((texOX + variant) * Props.tileSize * Props.resolutionScaleFactor, (texOY + 1) * Props.tileSize * Props.resolutionScaleFactor))); //bottom left

            //second triangle
            vA.Append(new Vertex(new Vector2f(oX + Props.tileSize + pR, oY - pR), shade,
                new Vector2f((texOX + variant + 1) * Props.tileSize * Props.resolutionScaleFactor, texOY * Props.tileSize * Props.resolutionScaleFactor))); //top right

            vA.Append(new Vertex(new Vector2f(oX + Props.tileSize + pR, oY + Props.tileSize + pR), shade,
                new Vector2f((texOX + variant + 1) * Props.tileSize * Props.resolutionScaleFactor, (texOY + 1) * Props.tileSize * Props.resolutionScaleFactor))); //bottom right

            vA.Append(new Vertex(new Vector2f(oX - pR, oY + Props.tileSize + pR), shade,
                new Vector2f((texOX + variant) * Props.tileSize * Props.resolutionScaleFactor, (texOY + 1) * Props.tileSize * Props.resolutionScaleFactor))); //bottom left
        }

        /// <summary>
        /// Helper function for appending terrain vertices.  May actually increase code execution time, hopefully C# inlines it :(
        /// </summary>
        /// <param name="vA"></param>
        /// <param name="oX"></param>
        /// <param name="oY"></param>
        /// <param name="texOX"></param>
        /// <param name="texOY"></param>
        /// <param name="variant"></param>
        /// <param name="shade"></param>
        private void AppendQuadVerticesCliff(VertexArray vA, int oX, int oY, int texOX, int texOY, int variant, Color shade)
        {
            //Pixel correction offset, fixes issue #2 on github
            float pR = 0.5f;
            //first triangle
            vA.Append(new Vertex(new Vector2f(oX - pR, oY - pR), shade,
                new Vector2f((texOX + variant) * Props.tileSize * Props.resolutionScaleFactor, texOY * 2 * Props.tileSize * Props.resolutionScaleFactor))); //top left

            vA.Append(new Vertex(new Vector2f(oX + Props.tileSize + pR, oY - pR), shade,
                new Vector2f((texOX + variant + 1) * Props.tileSize * Props.resolutionScaleFactor, texOY * 2 * Props.tileSize * Props.resolutionScaleFactor))); //top right

            vA.Append(new Vertex(new Vector2f(oX - pR, oY + 2 * Props.tileSize + pR), shade,
                new Vector2f((texOX + variant) * Props.tileSize * Props.resolutionScaleFactor, (texOY + 1) * 2 * Props.tileSize * Props.resolutionScaleFactor))); //bottom left

            //second triangle
            vA.Append(new Vertex(new Vector2f(oX + Props.tileSize + pR, oY - pR), shade,
                new Vector2f((texOX + variant + 1) * Props.tileSize * Props.resolutionScaleFactor, texOY * 2 * Props.tileSize * Props.resolutionScaleFactor))); //top right

            vA.Append(new Vertex(new Vector2f(oX + Props.tileSize + pR, oY + 2 * Props.tileSize + pR), shade,
                new Vector2f((texOX + variant + 1) * Props.tileSize * Props.resolutionScaleFactor, (texOY + 1) * 2 * Props.tileSize * Props.resolutionScaleFactor))); //bottom right

            vA.Append(new Vertex(new Vector2f(oX - pR, oY + 2 * Props.tileSize + pR), shade,
                new Vector2f((texOX + variant) * Props.tileSize * Props.resolutionScaleFactor, (texOY + 1) * 2 * Props.tileSize * Props.resolutionScaleFactor))); //bottom left
        }

        /// <summary>
        /// Gets the terrain texture tilesheets.  Required for drawing the constructed vertex arrays.
        /// </summary>
        /// <returns></returns>
        public Texture[] GetTerrainTilesheets()
        {
            Texture[] terrainTilesheet = new Texture[terrainTiles.Count];
            for (byte i = 1; i < terrainTiles.Count; i++)
            {
                terrainTilesheet[i] = terrainTiles[i].tileSheet;
            }
            terrainTilesheet[0] = cliffTilesheet;
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
            if (tileType >= terrainTiles.Count)
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

        public List<Tile> GetTerrainTiles()
        {
            return terrainTiles;
        }
    }
}
