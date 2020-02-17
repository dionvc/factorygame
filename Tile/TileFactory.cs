using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFML.Graphics;

namespace EngineeringCorpsCS
{
    class TileFactory
    {
        TextureManager textureManager;
        public TileFactory(TextureManager textureManager)
        {
            this.textureManager = textureManager;
        }

        public Tile[] GetTerrainTiles()
        {
            byte index = 1;
            Tile[] tiles = new Tile[1];
            tiles.Append(CreateDeepWater(index));
            index++;
            tiles.Append(CreateBeachSand(index));
            index++;
            return tiles;
        }
        public Tile[] GetTerrainPathTiles()
        {
            return null;
        }

        public Tile CreateDeepWater(byte index)
        {
            Base.CollisionLayer collisionMask = Base.CollisionLayer.TerrainSolid;
            return new Tile(textureManager.GetTexture(""), index, "Deep Water", 0, new Color(), new Color(), collisionMask);
        }

        public Tile CreateBeachSand(byte index)
        {
            Base.CollisionLayer collisionMask = Base.CollisionLayer.Terrain;
        }
    }
}
