using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EngineeringCorpsCS
{
    class MapGenerator
    {
        //Probabilities assigned here
        //each tile type will have a probability rating for each noise type
        //The sum of the differences between the sampled noise and these probabilities will determine the tile used
        //The smallest sum is the most likely tile
        //Expensive, but only needs to run once
        //Will store the numbers (random) needed to generate the map once again.
        //Domain of fastnoise perlin is [-1, 1]
    }
}
