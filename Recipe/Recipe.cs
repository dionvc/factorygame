using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EngineeringCorpsCS
{
    class Recipe
    {
        public int[] counts; //counts of the inputs required
        public string[] itemsRequired; //Names of required items
        public int[] countsResult; //counts of the results
        public string[] itemsResults; //Names of results
        public int recipeTime;

        public Recipe(int[] counts, string[] itemsRequired, int[] countsResult, string[] itemsResults, int recipeTime)
        {
            this.counts = counts;
            this.itemsRequired = itemsRequired;
            this.countsResult = countsResult;
            this.itemsResults = itemsResults;
            this.recipeTime = recipeTime;
        }
    }
}
