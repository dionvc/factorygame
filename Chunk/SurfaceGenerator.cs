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
        float moistureFactor;
        float temperatureFactor;
        float elevationFactor;
        public int seed { get; protected set; }
        public SurfaceGenerator(TileCollection tiles)
        {
            surfaceSize = 2048;
            moistureFactor = 1.0f;
            temperatureFactor = 1.0f;
            elevationFactor = 1.0f;
            elevationType = FastNoise.NoiseType.PerlinFractal;
            moistureType = FastNoise.NoiseType.PerlinFractal;
            temperatureType = FastNoise.NoiseType.PerlinFractal;

            elevationNoise = new FastNoise();
            elevationNoise.SetNoiseType(elevationType);
            moistureNoise = new FastNoise();
            moistureNoise.SetNoiseType(moistureType);
            temperatureNoise = new FastNoise();
            temperatureNoise.SetNoiseType(temperatureType);

            Random r = new Random();
            seed = r.Next(0, int.MaxValue - 1);
            SetSeed(seed);

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

            elevation = Math.Pow(elevation, elevationFactor); 
            moisture = Math.Pow(moisture, moistureFactor);
            temperature = Math.Pow(temperature, temperatureFactor);
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

        public void SetSeed(int seed)
        {
            this.seed = seed;
            Random r = new Random(seed);
            elevationNoise.SetSeed(r.Next(0, int.MaxValue - 1));
            moistureNoise.SetSeed(r.Next(0, int.MaxValue - 1));
            temperatureNoise.SetSeed(r.Next(0, int.MaxValue -1));
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

        public void SetNoiseFactor(string tag, float value)
        {
            if(tag == "moisture")
            {
                moistureFactor = value;
            }
            if(tag == "elevation")
            {
                elevationFactor = value;
            }
            if(tag == "temperature")
            {
                temperatureFactor = value;
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
                    surfaceSize = surfaceSize > Props.maxSurfaceSize * Props.chunkSize ? Props.maxSurfaceSize * Props.chunkSize : surfaceSize;
                    updated = surfaceSize.ToString();
                    return true;
                }
            }
            if (tag == "seed")
            {
                int val;
                if (int.TryParse(value, out val))
                {
                    SetSeed(val);
                    updated = val.ToString();
                    return true;
                }
                else
                {
                    //case of nil input need to update back to some random value
                    if (value == "")
                    {
                        SetSeed(new Random().Next(0, int.MaxValue-1));
                        updated = "";
                        return true;
                    }
                    //alternatively if the input couldn't be parsed just return the currently set seed
                    updated = seed.ToString();
                    return false;
                }
            }
            return false;
        }
    }
}
