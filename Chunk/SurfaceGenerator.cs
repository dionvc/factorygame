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
        FastNoise.NoiseType elevationType;
        FastNoise.NoiseType moistureType;
        FastNoise.NoiseType temperatureType;
        List<Tile> tileList;
        public int surfaceSize { get; set; }
        public SurfaceGenerator(TileCollection tiles)
        {
            elevationType = FastNoise.NoiseType.PerlinFractal;
            moistureType = FastNoise.NoiseType.PerlinFractal;
            temperatureType = FastNoise.NoiseType.PerlinFractal;
            Random r = new Random();
            int seed = r.Next(0, int.MaxValue - 1);
            elevationNoise = new FastNoise();
            elevationNoise.SetNoiseType(elevationType);
            elevationNoise.SetSeed(seed);
            elevationNoise.SetFractalType(FastNoise.FractalType.RigidMulti);

            moistureNoise = new FastNoise();
            moistureNoise.SetNoiseType(moistureType);
            moistureNoise.SetSeed(seed/3);

            temperatureNoise = new FastNoise();
            temperatureNoise.SetNoiseType(temperatureType);
            temperatureNoise.SetSeed(seed/7);

            tileList = tiles.GetTerrainTiles();
        }

        public byte GetTile(int nx, int ny)
        {
            double elevation = 0.5 * elevationNoise.GetNoise(1 * nx, 1 * ny)
            + 0.25 * elevationNoise.GetNoise(2 * nx, 2 * ny)
            + 0.25 * elevationNoise.GetNoise(4 * nx, 4 * ny);
            double moisture = 0.5 * moistureNoise.GetNoise(1 * nx, 1 * ny)
            + 0.25 * moistureNoise.GetNoise(2 * nx, 2 * ny)
            + 0.25 * moistureNoise.GetNoise(4 * nx, 4 * ny);
            double temperature = 0.5 * temperatureNoise.GetNoise(1 * nx, 1 * ny)
            + 0.25 * temperatureNoise.GetNoise(2 * nx, 2 * ny)
            + 0.25 * temperatureNoise.GetNoise(4 * nx, 4 * ny);

            elevation = Math.Pow(elevation, 1); 
            moisture = Math.Pow(moisture, 0.5);
            temperature = Math.Pow(temperature, 1);
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

        public bool SetSeed(string seed, out string parsed)
        {
            int parsedInt;
            if(!int.TryParse(seed, out parsedInt))
            {
                parsed = "0";
                return false;
            }
            Random r = new Random(parsedInt);
            elevationNoise.SetSeed(r.Next(0, int.MaxValue - 1));
            moistureNoise.SetSeed(r.Next(0, int.MaxValue - 1));
            temperatureNoise.SetSeed(r.Next(0, int.MaxValue -1));
            parsed = parsedInt.ToString();
            return true;
        }

        public void SetNoiseType(string type, int value)
        {
            if(type == "moisture")
            {
                moistureNoise.SetNoiseType((FastNoise.NoiseType)value);
            }
            else if(type == "elevation")
            {
                elevationNoise.SetNoiseType((FastNoise.NoiseType)value);
            }
            else if(type == "temperature")
            {
                temperatureNoise.SetNoiseType((FastNoise.NoiseType)value);
            }
        }

        public bool ParseString(string tag, string value, out string updated)
        {
            updated = "";
            if (tag == "surfacesize")
            {
                int val;
                if (int.TryParse(value, out val))
                {
                    surfaceSize = val;
                    surfaceSize = surfaceSize > 2048 ? 2048 : surfaceSize;
                    updated = surfaceSize.ToString();
                    return true;
                }
            }
            return false;
        }
    }
}
