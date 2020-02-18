using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFML.Graphics;
using SFML.System;

namespace EngineeringCorpsCS
{
    class Tile: Base
    {
        public Texture tileSheet { get; protected set; } //texture with terrain tiles
        public byte tileType { get; protected set; } //number specifying the type of the tile (corresponding to the type in the terrain array and tilemanager collection)
        public CollisionLayer collisionMask { get; protected set; } //Whatever layers are set here is what will determine its collision type

        float speedModifier; //modifier for speed of entities on this tile
        Color shade; //Apply a color to the tile
        Color mapColor; //Color that will be represented in minimap
        

        public Tile(Texture tileSheet, byte tileType, string name, float speedModifier, Color shade, Color mapColor, CollisionLayer collisionMask)
        {
            this.tileSheet = tileSheet;
            this.tileType = tileType;
            this.speedModifier = speedModifier;
            this.shade = shade;
            this.mapColor = mapColor;
            this.collisionMask = collisionMask;
            this.name = name;
        }

        public void AppendTerrainVertices(VertexArray vertexArray, ChunkManager chunkManager, int[]pos, List<byte> impassableTileTypes)
        {
            int[] cXY = new int[] { pos[0] * Props.chunkSize, pos[1] * Props.chunkSize };
            for (int i = 0; i < Props.chunkSize; i++)
            {
                for (int j = 0; j < Props.chunkSize; j++)
                {
                    //Modified Fast Hash algorithm 
                    int variantX = i * 266489917 + 374761393;
                    variantX = (variantX << 17) | (variantX >> 15);
                    variantX += j * 266489917;
                    variantX *= 668265263;
                    variantX ^= variantX >> 15;
                    variantX *= 246822519;
                    variantX = Math.Abs(variantX);
                    float oX = (cXY[0] + i) * Props.tileSize;
                    float oY = (cXY[1] + j) * Props.tileSize;
                    //First check if tile is present at location, if it is then simply append vertices of the regular tile
                    byte currentTile = chunkManager.GetTileFromWorldInt(pos, i, j);
                    if (currentTile == this.tileType)
                    {
                        int variantRegular = variantX % 8;
                        //Append regular tile variation
                        //first triangle
                        
                        vertexArray.Append(new Vertex(new Vector2f(oX, oY),
                            new Vector2f(Props.tileSize * variantRegular, 0.0f))); //top left

                        vertexArray.Append(new Vertex(new Vector2f(oX + Props.tileSize, oY),
                            new Vector2f(Props.tileSize * (variantRegular + 1), 0.0f))); //top right

                        vertexArray.Append(new Vertex(new Vector2f(oX, oY + Props.tileSize),
                            new Vector2f(Props.tileSize * variantRegular, Props.tileSize))); //bottom left

                        //second triangle
                        vertexArray.Append(new Vertex(new Vector2f(oX + Props.tileSize, oY),
                            new Vector2f((Props.tileSize) * (variantRegular + 1), 0.0f))); //top right

                        vertexArray.Append(new Vertex(new Vector2f(oX + Props.tileSize, oY + Props.tileSize),
                            new Vector2f(Props.tileSize * (variantRegular + 1) , Props.tileSize))); //bottom right

                        vertexArray.Append(new Vertex(new Vector2f(oX, oY + Props.tileSize),
                            new Vector2f(Props.tileSize * (variantRegular), Props.tileSize))); //bottom left

                        continue;
                    }
                    else if(currentTile > this.tileType)
                    {
                        continue;
                    }
                    //Next check for tile transitions
                    int value = 0;
                    int variant = variantX % 4; //variant is determine by whether the transition is into an impassable tile
                    
                    if (impassableTileTypes.Contains(currentTile))
                    {
                        variant += 0;
                    }
                    value += (chunkManager.GetTileFromWorldInt(pos, i-1, j-1) == tileType) ? 1 : 0; //top left
                    value += (chunkManager.GetTileFromWorldInt(pos, i, j - 1) == tileType) ? 2 : 0; //top center
                    value += (chunkManager.GetTileFromWorldInt(pos, i + 1, j - 1) == tileType) ? 4 : 0; //top right
                    value += (chunkManager.GetTileFromWorldInt(pos, i - 1, j) == tileType) ? 8 : 0; //left
                    value += (chunkManager.GetTileFromWorldInt(pos, i + 1, j) == tileType) ? 16 : 0; //right
                    value += (chunkManager.GetTileFromWorldInt(pos, i - 1, j + 1) == tileType) ? 32 : 0; //bottom left
                    value += (chunkManager.GetTileFromWorldInt(pos, i , j + 1) == tileType) ? 64 : 0; //bottom center
                    value += (chunkManager.GetTileFromWorldInt(pos, i + 1, j + 1) == tileType) ? 128 : 0; //bottom right

                    //Virtual mapping of surrounding tiles
                    // 1 2 3
                    // 4 0 5
                    // 6 7 8
                    //bitflag values of surrounding tiles
                    // 1  2  4
                    // 8  0  16
                    // 32 64 128


                    if (value == 0)
                    {
                        continue; //no surrounding tiles of tiletype
                    }
                    if (((value & 24) + (value & 2)) == 2)
                    {
                        //top fade
                        vertexArray.Append(new Vertex(new Vector2f(oX, oY),
                            new Vector2f(Props.tileSize * variant, Props.tileSize))); //top left

                        vertexArray.Append(new Vertex(new Vector2f(oX + Props.tileSize, oY),
                            new Vector2f(Props.tileSize * (variant + 1), Props.tileSize))); //top right

                        vertexArray.Append(new Vertex(new Vector2f(oX, oY + Props.tileSize),
                            new Vector2f(Props.tileSize * variant, 2 * Props.tileSize))); //bottom left

                        //second triangle
                        vertexArray.Append(new Vertex(new Vector2f(oX + Props.tileSize, oY),
                            new Vector2f((Props.tileSize) * (variant + 1), Props.tileSize))); //top right

                        vertexArray.Append(new Vertex(new Vector2f(oX + Props.tileSize, oY + Props.tileSize),
                            new Vector2f(Props.tileSize * (variant + 1), 2 * Props.tileSize))); //bottom right

                        vertexArray.Append(new Vertex(new Vector2f(oX, oY + Props.tileSize),
                            new Vector2f(Props.tileSize * (variant), 2 * Props.tileSize))); //bottom left
                    }
                    if (((value & 24) + (value & 64)) == 64)
                    {
                        //bottom fade
                        vertexArray.Append(new Vertex(new Vector2f(oX, oY),
                            new Vector2f(Props.tileSize * (variant + 8), Props.tileSize))); //top left

                        vertexArray.Append(new Vertex(new Vector2f(oX + Props.tileSize, oY),
                            new Vector2f(Props.tileSize * (variant + 1 + 8), Props.tileSize))); //top right

                        vertexArray.Append(new Vertex(new Vector2f(oX, oY + Props.tileSize),
                            new Vector2f(Props.tileSize * (variant + 8), 2 * Props.tileSize))); //bottom left

                        //second triangle
                        vertexArray.Append(new Vertex(new Vector2f(oX + Props.tileSize, oY),
                            new Vector2f((Props.tileSize) * (variant + 1 + 8), Props.tileSize))); //top right

                        vertexArray.Append(new Vertex(new Vector2f(oX + Props.tileSize, oY + Props.tileSize),
                            new Vector2f(Props.tileSize * (variant + 1 + 8), 2 * Props.tileSize))); //bottom right

                        vertexArray.Append(new Vertex(new Vector2f(oX, oY + Props.tileSize),
                            new Vector2f(Props.tileSize * (variant + 8), 2 * Props.tileSize))); //bottom left
                    }
                    if (((value & 66) + (value & 8)) == 8)
                    {
                        //left fade
                        vertexArray.Append(new Vertex(new Vector2f(oX, oY),
                            new Vector2f(Props.tileSize * variant, 2 * Props.tileSize))); //top left

                        vertexArray.Append(new Vertex(new Vector2f(oX + Props.tileSize, oY),
                            new Vector2f(Props.tileSize * (variant + 1), 2 * Props.tileSize))); //top right

                        vertexArray.Append(new Vertex(new Vector2f(oX, oY + Props.tileSize),
                            new Vector2f(Props.tileSize * variant, 3 * Props.tileSize))); //bottom left

                        //second triangle
                        vertexArray.Append(new Vertex(new Vector2f(oX + Props.tileSize, oY),
                            new Vector2f((Props.tileSize) * (variant + 1), 2 * Props.tileSize))); //top right

                        vertexArray.Append(new Vertex(new Vector2f(oX + Props.tileSize, oY + Props.tileSize),
                            new Vector2f(Props.tileSize * (variant + 1), 3 * Props.tileSize))); //bottom right

                        vertexArray.Append(new Vertex(new Vector2f(oX, oY + Props.tileSize),
                            new Vector2f(Props.tileSize * (variant), 3 * Props.tileSize))); //bottom left
                    }
                    if (((value & 66) + (value & 16)) == 16)
                    {
                        //right fade
                        vertexArray.Append(new Vertex(new Vector2f(oX, oY),
                            new Vector2f(Props.tileSize * (variant + 8), 2 * Props.tileSize))); //top left

                        vertexArray.Append(new Vertex(new Vector2f(oX + Props.tileSize, oY),
                            new Vector2f(Props.tileSize * (variant + 1 + 8), 2 * Props.tileSize))); //top right

                        vertexArray.Append(new Vertex(new Vector2f(oX, oY + Props.tileSize),
                            new Vector2f(Props.tileSize * (variant + 8), 3 * Props.tileSize))); //bottom left

                        //second triangle
                        vertexArray.Append(new Vertex(new Vector2f(oX + Props.tileSize, oY),
                            new Vector2f((Props.tileSize) * (variant + 1 + 8), 2 * Props.tileSize))); //top right

                        vertexArray.Append(new Vertex(new Vector2f(oX + Props.tileSize, oY + Props.tileSize),
                            new Vector2f(Props.tileSize * (variant + 1 + 8), 3 * Props.tileSize))); //bottom right

                        vertexArray.Append(new Vertex(new Vector2f(oX, oY + Props.tileSize),
                            new Vector2f(Props.tileSize * (variant + 8), 3 * Props.tileSize))); //bottom left
                    }
                    if ((value & 10) == 10)
                    {
                        //inside corner topleft
                    }
                    else if (((value & 1) + (value & 10)) == 1)
                    {
                        //outside corner topleft
                    }
                    if ((value & 18) == 18)
                    {
                        //inside corner topright
                    }
                    else if (((value & 4) + (value & 18)) == 4)
                    {
                        //outside corner topright
                    }
                    if ((value & 72) == 72)
                    {
                        //inside corner bottomleft
                    }
                    else if (((value & 32) + (value & 72)) == 32)
                    {
                        //outside corner bottomleft
                    }
                    if ((value & 80) == 80)
                    {
                        //inside corner bottomright
                    }
                    else if (((value & 128) + (value & 80)) == 128)
                    {
                        //outside corner bottomright
                    }
                }
            }
        }
    }
}
