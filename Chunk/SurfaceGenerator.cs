using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EngineeringCorpsCS
{
    class SurfaceGenerator
    {
        //Probabilities assigned here
        //each tile type will have a probability rating for each noise type
        //The sum of the differences between the sampled noise and these probabilities will determine the tile used
        //The smallest sum is the most likely tile
        //Expensive, but only needs to run once
        //Will store the numbers (random) needed to generate the map once again.
        //Domain of fastnoise perlin is [-1, 1]


        //elevation - defines where water could appear and preference for "higher" terrains
        //moisture - defines moisture level for more wet biomes and trees versus dead trees
        //rivers - could be generated from fractal noise?
        FastNoise elevationNoise;
        FastNoise moistureNoise;
        FastNoise temperatureNoise;
        List<Tile> tileList;
        public SurfaceGenerator(int seed, TileCollection tiles)
        {
            Random r = new Random();
            elevationNoise = new FastNoise();
            elevationNoise.SetNoiseType(FastNoise.NoiseType.Perlin);
            elevationNoise.SetSeed(r.Next(0, int.MaxValue - 1));

            moistureNoise = new FastNoise();
            moistureNoise.SetNoiseType(FastNoise.NoiseType.Perlin);
            moistureNoise.SetSeed(r.Next(0, int.MaxValue - 1));

            temperatureNoise = new FastNoise();
            temperatureNoise.SetNoiseType(FastNoise.NoiseType.Perlin);
            temperatureNoise.SetSeed(r.Next(0, int.MaxValue - 1));

            tileList = tiles.GetTerrainTiles();
        }

        public byte GetTile(int nx, int ny)
        {
            double elevation = 0.5 * elevationNoise.GetPerlin(1 * nx, 1 * ny)
            + 0.25 * elevationNoise.GetPerlin(2 * nx, 2 * ny)
            + 0.25 * elevationNoise.GetPerlin(4 * nx, 4 * ny);
            double moisture = 0.5 * moistureNoise.GetPerlin(1 * nx, 1 * ny)
            + 0.25 * moistureNoise.GetPerlin(2 * nx, 2 * ny)
            + 0.25 * moistureNoise.GetPerlin(4 * nx, 4 * ny);
            double temperature = 0.5 * temperatureNoise.GetPerlin(1 * nx, 1 * ny)
            + 0.25 * temperatureNoise.GetPerlin(2 * nx, 2 * ny)
            + 0.25 * temperatureNoise.GetPerlin(4 * nx, 4 * ny);
            byte chosenTile = 0;
            float currentMin = 2;
            foreach(Tile t in tileList)
            {
                float tileProb = (float) (Math.Abs(t.moistureAffinity - 2 * moisture) + Math.Abs(t.temperatureAffinity - 2 * temperature) + Math.Abs(t.elevationAffinity - 2 * elevation));
                if (tileProb < currentMin) {
                    chosenTile = t.tileType;
                    currentMin = tileProb;
                }
            }
            return chosenTile;
        }
    }
}
