using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EngineeringCorpsCS
{
    class TileManager
    {
        Tile[] terrainTiles;
        byte[] impassableTileTypes;
        Tile[] terrainPathTiles;
        public TileManager()
        {
            TileFactory tileFactory = new TileFactory();
            terrainTiles = tileFactory.GetTerrainTiles();
            terrainPathTiles = tileFactory.GetTerrainPathTiles();
        }
    }
}
