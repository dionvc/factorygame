using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFML.Graphics;

namespace EngineeringCorpsCS
{
    class Recipe
    {
        public int[] counts; //counts of the inputs required
        public string[] itemsRequired; //Names of required items
        public int[] countsResult; //counts of the results
        public string[] itemsResults; //Names of results
        public int recipeTime;
        public StaticSprite recipeSprite;
        public string name;
        public string[] canBeMadeIn { get; set; } = new string[] { "" };

        public Recipe(string name, int[] counts, string[] itemsRequired, int[] countsResult, string[] itemsResults, int recipeTime, StaticSprite recipeSprite)
        {
            this.name = name;
            this.counts = counts;
            this.itemsRequired = itemsRequired;
            this.countsResult = countsResult;
            this.itemsResults = itemsResults;
            this.recipeTime = recipeTime;
            this.recipeSprite = recipeSprite;
        }

        public Recipe(string name, int counts, string itemRequired, int countResult, string itemResult, int recipeTime, StaticSprite recipeSprite)
        {
            this.name = name;
            this.counts = new int[] { counts };
            this.itemsRequired = new string[] { itemRequired };
            this.countsResult = new int[] { countResult };
            this.itemsResults = new string[] { itemResult };
            this.recipeTime = recipeTime;
            this.recipeSprite = recipeSprite;
        }
    }
}
