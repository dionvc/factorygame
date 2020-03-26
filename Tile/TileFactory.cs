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
        TextureContainer textureManager;
        Color defaultShade;
        public TileFactory(TextureContainer textureManager)
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
            tiles.Add(CreateSnow((byte)tiles.Count));
            return tiles;
        }
        public List<Tile> GetTerrainPathTiles()
        {
            return null;
        }

        public Tile CreateDeepWater(byte index)
        {
            Base.CollisionLayer collisionMask = Base.CollisionLayer.TerrainSolid;
            return new Tile(textureManager.GetTexture("watersquare"), index, "Deep Water", 0, defaultShade, new Color(0,45,200), collisionMask, 1.0f, -0.25f, 0.0f);
        }

        public Tile CreateBeachSand(byte index)
        {
            Base.CollisionLayer collisionMask = Base.CollisionLayer.Terrain;
            return new Tile(textureManager.GetTexture("lavaTilesheet"), index, "Beach Sand", 0.85f, defaultShade, new Color(160,64,0), collisionMask, 0.6f, 0.0f, 0.7f);
        }

        public Tile CreateDesert(byte index)
        {
            Base.CollisionLayer collisionMask = Base.CollisionLayer.Terrain;
            return new Tile(textureManager.GetTexture("desertTilesheet"), index, "Desert", 0.85f, defaultShade, new Color(160,64,0), collisionMask, -0.75f, 0.25f, 0.75f);
        }

        public Tile CreateVoid(byte index)
        {
            Base.CollisionLayer collisionMask = Base.CollisionLayer.Void | Base.CollisionLayer.TerrainSolid;
            return new Tile(textureManager.GetTexture(""), index, "Void", 0, defaultShade, new Color(0,0,0), collisionMask, 0.0f, -1.0f, 0.2f);
        }

        public Tile CreateGrass(byte index)
        {
            Base.CollisionLayer collisionMask = Base.CollisionLayer.Terrain;
            return new Tile(textureManager.GetTexture("grassTilesheet"), index, "Grass", 1.1f, defaultShade, new Color(45,128,64), collisionMask, 0.5f, 0.2f, 0.25f);
        }

        public Tile CreateSnow(byte index)
        {
            Base.CollisionLayer collisionMask = Base.CollisionLayer.Terrain;
            return new Tile(textureManager.GetTexture("snowTilesheet"), index, "Snow", 0.9f, defaultShade, new Color(12, 12, 12), collisionMask, 0.6f, 0.2f, -0.5f);
        }
    }
}
