using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EngineeringCorpsCS
{
    class GeneratorEntityAffinity
    {
        public string[] prototypeVars { get; protected set; }
        public float[] moistureAffinities { get; protected set; }
        public float[] elevationAffinities { get; protected set; }
        public float[] temperatureAffinities { get; protected set; }
        public float[] moistureRange { get; protected set; }
        public float[] temperatureRange { get; protected set; }
        public float[] elevationRange { get; protected set; }
        public float density { get; protected set; }

        public Base.CollisionLayer placementMask = Base.CollisionLayer.TerrainSolid | Base.CollisionLayer.EntityPhysical;

        public int prototypeCount;
        public GeneratorEntityAffinity(string[] prototypes, float[] moistureAffinities, float[] temperatureAffinities, float[] elevationAffinities, float[] moistureRange, float[] temperatureRange, float[] elevationRange)
        {
            this.prototypeVars = prototypes;
            this.moistureAffinities = moistureAffinities;
            this.temperatureAffinities = temperatureAffinities;
            this.elevationAffinities = elevationAffinities;
            this.moistureRange = moistureRange;
            this.elevationRange = elevationRange;
            this.temperatureRange = temperatureRange;
            prototypeCount = prototypeVars.Length;
        }

        public void SetDensity(string tag, float value)
        {
            density = value;
        }
    }
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
        EntityCollection entityCollection;
        List<GeneratorEntityAffinity> entitiesGenerated;
        public int surfaceSize { get; set; }
        public byte levelOfDarkness { get; set; } = 196;
        public int lengthOfDay { get; set; } = 12000;
        public int lengthOfNight { get; set; } = 3000;
        float moistureFactor;
        float temperatureFactor;
        float elevationFactor;
        public int seed { get; protected set; }
        public SurfaceGenerator(TileCollection tiles, EntityCollection entityCollection, List<GeneratorEntityAffinity> entities)
        {
            surfaceSize = Props.maxSurfaceSize * Props.chunkSize;
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
            this.entityCollection = entityCollection;
            this.entitiesGenerated = entities;
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

            elevation = 3 * Math.Pow(elevation, elevationFactor); 
            moisture = 3 * Math.Pow(moisture, moistureFactor);
            temperature = 3 * Math.Pow(temperature, temperatureFactor);
            byte chosenTile = 0;
            float currentMin = 2;
            foreach(Tile t in tileList)
            {
                float tileProb = (float) (Math.Abs(t.moistureAffinity - moisture) + Math.Abs(t.temperatureAffinity - temperature) + Math.Abs(t.elevationAffinity - elevation));
                if (tileProb < currentMin) {
                    chosenTile = t.tileType;
                    currentMin = tileProb;
                }
            }
            return chosenTile;
        }

        /// <summary>
        /// Generates the entities to be placed into a chunk
        /// </summary>
        /// <param name="chunkX"></param>
        /// <param name="chunkY"></param>
        /// <param name="surface"></param>
        public void GenerateEntities(int chunkX, int chunkY, SurfaceContainer surface)
        {
            Vector2 chunkPos = new Vector2(chunkX * Props.chunkSize * Props.tileSize, chunkY * Props.chunkSize * Props.tileSize);
            for (int i = 0; i < entitiesGenerated.Count; i++)
            {
                Vector2[] poissonDiscDistribution = PoissonDiscDistribution.GetDistribution(entitiesGenerated[i].density, (chunkX * (surfaceSize / Props.chunkSize) + chunkY) + seed, 10);
                for (int j = 0; j < poissonDiscDistribution.Length; j++)
                {
                    Vector2 curPos = poissonDiscDistribution[j].VAdd(chunkPos);
                    int nx = (int)(curPos.x) / Props.tileSize;
                    int ny = (int)(curPos.y) / Props.tileSize;
                    double elevation = 0.5 * elevationNoise.GetNoise(1 * nx, 1 * ny)
                    + 0.25 * elevationNoise.GetNoise(2 * nx, 2 * ny)
                    + 0.25 * elevationNoise.GetNoise(4 * nx, 4 * ny);
                    double moisture = 0.5 * moistureNoise.GetNoise(1 * nx, 1 * ny)
                    + 0.25 * moistureNoise.GetNoise(2 * nx, 2 * ny)
                    + 0.25 * moistureNoise.GetNoise(4 * nx, 4 * ny);
                    double temperature = 0.5 * temperatureNoise.GetNoise(1 * nx, 1 * ny)
                    + 0.25 * temperatureNoise.GetNoise(2 * nx, 2 * ny)
                    + 0.25 * temperatureNoise.GetNoise(4 * nx, 4 * ny);
                    elevation = 3 * Math.Pow(elevation, elevationFactor);
                    moisture = 3 * Math.Pow(moisture, moistureFactor);
                    temperature = 3 * Math.Pow(temperature, temperatureFactor);

                    float lowestAffinity = 2;
                    int lowestPrototype = 0;

                    for (int k = 0; k < entitiesGenerated[i].prototypeVars.Length; k++)
                    {
                        float entityProb = (float)(Math.Abs(entitiesGenerated[i].elevationAffinities[k] - elevation) +
                           Math.Abs(entitiesGenerated[i].moistureAffinities[k] - moisture) +
                           Math.Abs(entitiesGenerated[i].temperatureAffinities[k] - temperature));
                        if (entityProb < lowestAffinity)
                        {
                            lowestAffinity = entityProb;
                            lowestPrototype = k;
                        }
                    }
                    float moistureDiff = (float)(Math.Abs(entitiesGenerated[i].moistureAffinities[lowestPrototype] - moisture));
                    float elevationDiff = (float)(Math.Abs(entitiesGenerated[i].elevationAffinities[lowestPrototype] - elevation));
                    float temperatureDiff = (float)(Math.Abs(entitiesGenerated[i].temperatureAffinities[lowestPrototype] - temperature));
                    if (entitiesGenerated[i].moistureRange[lowestPrototype % entitiesGenerated[i].prototypeCount] > elevationDiff
                        && entitiesGenerated[i].temperatureRange[lowestPrototype % entitiesGenerated[i].prototypeCount] > temperatureDiff
                        && entitiesGenerated[i].elevationRange[lowestPrototype % entitiesGenerated[i].prototypeCount] > moistureDiff)
                    {
                        Entity prototype = entityCollection.GetPrototype(entitiesGenerated[i].prototypeVars[lowestPrototype]);
                        if(prototype.tileAligned)
                        {
                            curPos = new Vector2(curPos.x - curPos.x % 32 + 16, curPos.y - curPos.y % 32 + 16);
                        }
                        if (prototype != null && !BoundingBox.CheckForPlacementCollision(prototype.collisionBox, curPos, surface, entitiesGenerated[i].placementMask))
                        {
                            entityCollection.InstantiatePrototype(prototype.name, curPos, surface);
                        }
                    }
                }
            }
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

        public void SetSurfaceTime(string tag, float value)
        {
            if(tag == "darkness")
            {
                levelOfDarkness = Convert.ToByte(value);
            }
            if(tag == "lengthOfDay")
            {
                lengthOfDay = (int)(value);
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
