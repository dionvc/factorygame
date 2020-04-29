using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EngineeringCorpsCS
{
    class RecipeCollection
    {
        TextureAtlases textureAtlases;
        Dictionary<string, Recipe> recipeCollection;
        public RecipeCollection(TextureAtlases textureAtlases)
        {
            this.textureAtlases = textureAtlases;
        }

        public void LoadRecipes()
        {
            RecipeFactory recipeFactory = new RecipeFactory();
            recipeCollection = recipeFactory.GetRecipes(textureAtlases);
        }

        public Recipe[] GetRecipes(string[] tags)
        {
            List<Recipe> recipeList = new List<Recipe>();
            foreach(string key in recipeCollection.Keys)
            {
                for (int i = 0; i < tags.Length; i++)
                {
                    if (recipeCollection[key].canBeMadeIn.Contains(tags[i]))
                    {
                        recipeList.Add(recipeCollection[key]);
                    }
                }
            }
            return recipeList.ToArray();
        }
    }
}
