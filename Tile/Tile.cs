using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFML.Graphics;

namespace EngineeringCorpsCS
{
    class Tile: Base
    {
        Texture tileSheet;
        //Tiletype also specifies draw order
        byte tileType; //number specifying the type of the tile (corresponding to the type in the terrain array and tilemanager collection)
        float speedModifier; //modifier for speed of entities on this tile
        Color shade; //Apply a color to the tile
        Color mapColor; //Color that will be represented in minimap
        CollisionLayer collisionMask; //Whatever layers are set here is what will determine its collision type

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

        public void AppendTerrainVertices(byte[] terrain, int[] xy, byte currentTileType, byte[] impassableTileTypes)
        {
            //First check if tile is present at location, if it is then simply append vertices of the regular tile
            if(terrain[xy[0] * Props.chunkSize + xy[1]] == tileType)
            {
                //Append regular tile variation
            }

            //Next check for tile transitions
            int value = 0;
            int variant = ((xy[0] * Props.chunkSize + xy[1]) * 7) % 4; //variant is determine by whether the transition is into an impassable tile
            if(impassableTileTypes.Contains(terrain[xy[0] * Props.chunkSize + xy[1]]))
            {
                variant += 4;
            }
            value += (terrain[(xy[0] - 1) * Props.chunkSize + (xy[1] - 1)] == currentTileType) ? 1 : 0; //top left
            value += (terrain[(xy[0]) * Props.chunkSize + (xy[1] - 1)] == currentTileType) ? 2 : 0; //top center
            value += (terrain[(xy[0]) * Props.chunkSize + (xy[1] - 1)] == currentTileType) ? 4 : 0; //top right
            value += (terrain[(xy[0] - 1) * Props.chunkSize + (xy[1])] == currentTileType) ? 8 : 0; //left
            value += (terrain[(xy[0] + 1) * Props.chunkSize + (xy[1])] == currentTileType) ? 16 : 0; //right
            value += (terrain[(xy[0] - 1) * Props.chunkSize + (xy[1] + 1)] == currentTileType) ? 32 : 0; //bottom left
            value += (terrain[(xy[0]) * Props.chunkSize + (xy[1] + 1)] == currentTileType) ? 64 : 0; //bottom center
            value += (terrain[(xy[0] + 1) * Props.chunkSize + (xy[1] + 1)] == currentTileType) ? 128 : 0; //bottom right
            
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
                return; //no surrounding tiles of tiletype
            }
            if((value & 24 + value & 2) == 2)
            {
                //top fade
            }
            if((value & 24 + value & 64) == 64)
            {
                //bottom fade
            }
            if((value & 66 + value & 8) == 8)
            {
                //left fade
            }
            if((value & 66 + value & 16) == 16)
            {
                //right fade
            }
            if((value & 10) == 10)
            {
                //inside corner topleft
            }
            else if ((value & 1 + value & 10) == 1)
            {
                //outside corner topleft
            }
            if ((value & 18) == 18)
            {
                //inside corner topright
            }
            else if ((value & 4 + value & 18) == 4)
            {
                //outside corner topright
            }
            if ((value & 72) == 72)
            {
                //inside corner bottomleft
            }
            else if ((value & 32 + value & 72) == 32)
            {
                //outside corner bottomleft
            }
            if ((value & 80) == 80)
            {
                //inside corner bottomright
            }
            else if ((value & 128 + value & 80) == 128)
            {
                //outside corner bottomright
            }
        }
    }
}
