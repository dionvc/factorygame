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
        Color defaultShade;
        public TileFactory(TextureManager textureManager)
        {
            this.textureManager = textureManager;
            defaultShade = new Color(255, 255, 255, 255);
        }

        public List<Tile> GetTerrainTiles()
        {
            List<Tile> tiles = new List<Tile>();
            tiles.Add(CreateVoid((byte)tiles.Count));
            tiles.Add(CreateDeepWater((byte)tiles.Count));
            tiles.Add(CreateBeachSand((byte)tiles.Count));
            tiles.Add(CreateDesert((byte)tiles.Count));
            return tiles;
        }
        public List<Tile> GetTerrainPathTiles()
        {
            return null;
        }

        public Tile CreateDeepWater(byte index)
        {
            Base.CollisionLayer collisionMask = Base.CollisionLayer.TerrainSolid;
            return new Tile(textureManager.GetTexture("watersquare"), index, "Deep Water", 0, defaultShade, new Color(), collisionMask);
        }

        public Tile CreateBeachSand(byte index)
        {
            Base.CollisionLayer collisionMask = Base.CollisionLayer.Terrain;
            return new Tile(textureManager.GetTexture("tileTransitionTest"), index, "Beach Sand", 1, defaultShade, new Color(), collisionMask);
        }

        public Tile CreateDesert(byte index)
        {
            Base.CollisionLayer collisionMask = Base.CollisionLayer.Terrain;
            return new Tile(textureManager.GetTexture("desertTilesheet"), index, "Desert", 1, defaultShade, new Color(), collisionMask);
        }

        public Tile CreateVoid(byte index)
        {
            Base.CollisionLayer collisionMask = Base.CollisionLayer.Void & Base.CollisionLayer.TerrainSolid;
            return new Tile(textureManager.GetTexture(""), index, "Void", 0, defaultShade, new Color(), collisionMask);
        }
    }
}
