using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFML.Graphics;

namespace EngineeringCorpsCS
{
    class RecipeFactory
    {
        TextureAtlases textureAtlases;
        public Dictionary<string, Recipe> GetRecipes(TextureAtlases textureAtlases)
        {
            Dictionary<string, Recipe> recipeCollection = new Dictionary<string, Recipe>();
            this.textureAtlases = textureAtlases;
            Recipe recipe = GrowWoodRecipe();
            recipeCollection.Add(recipe.name, recipe);
            return recipeCollection;
        }

        public Recipe GrowWoodRecipe()
        {
            IntRect bounds;
            StaticSprite recipeIcon = new StaticSprite(textureAtlases.GetTexture("woodItem", out bounds), bounds, Drawable.DrawLayer.None);
            Recipe recipe = new Recipe("Grow Wood", new int[0], new string[0], new int[] { 1 }, new string[] { "Wood" }, 180, recipeIcon);
            recipe.canBeMadeIn = new string[] { "Greenhouse" };
            return recipe;
        }
    }
}
