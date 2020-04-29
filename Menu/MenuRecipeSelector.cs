using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFML.System;
using SFML.Graphics;

namespace EngineeringCorpsCS
{
    class MenuRecipeSelector:MenuComponent
    {
        public delegate void ApplyRecipe(Recipe recipe, Player applyingPlayer);

        ApplyRecipe applyRecipe;
        Recipe[] allowedRecipes;
        BoundingBox frameBox;
        int hoveredIndex = -1;
        Vector2f hoveredPosition;
        MenuText recipeName;
        Player applyingPlayer;
        /// <summary>
        /// Pass in an applyRecipeMethod
        /// </summary>
        /// <param name="componentSize"></param>
        /// <param name="entityRecipe"></param>
        public MenuRecipeSelector(Vector2i componentSize, ApplyRecipe applyRecipe, Player applyPlayer, Recipe[] allowedRecipes, Font font)
        {
            Initialize(componentSize);
            this.applyRecipe = applyRecipe;
            this.applyingPlayer = applyPlayer;
            this.allowedRecipes = allowedRecipes;
            frameBox = new BoundingBox(0, 0, 32, 32);
            recipeName = new MenuText(new Vector2i(128, 32), font, "", 16, 0);
        }

        public override void Draw(RenderTexture gui, Vector2i origin, RenderStates guiState)
        {
            RectangleShape frame = new RectangleShape(new Vector2f(32, 32));
            frame.FillColor = Color.Red;
            Vector2f pos = new Vector2f((position + origin).X, (origin + position).Y);
            for (int i = 0; i < allowedRecipes.Length; i++)
            {
                frame.Position = pos + new Vector2f((i * 32) % (int)size.X, ((i * 32) / (int)size.X) * 32);
                gui.Draw(frame);
                Sprite recipeSprite = allowedRecipes[i].recipeSprite.GetSprite();
                recipeSprite.Position = pos + new Vector2f((i * 32) % (int)size.X, (i * 32) / (int)size.X * 32);
                gui.Draw(recipeSprite);
            }
            if(hoveredIndex != -1)
            {
                recipeName.SetText(allowedRecipes[hoveredIndex].name);
                recipeName.SetTextPosition("left", "center");
                recipeName.SetRelativePosition(new Vector2i((int)hoveredPosition.X, (int)hoveredPosition.Y));
                recipeName.Draw(gui, new Vector2i(0,0), guiState);
            }
            base.Draw(gui, origin, guiState);
        }

        public override void HandleInput(InputManager input)
        {
            base.HandleInput(input);
            Vector2f mousePos;
            bool mouse = input.GetMousePosition(out mousePos);
            Vector2i origin = new Vector2i(0, 0);
            MenuComponent bubble = parent;
            while (bubble != null)
            {
                origin += bubble.position;
                bubble = bubble.parent;
            }
            Vector2f pos = new Vector2f((position + origin).X, (origin + position).Y);
            if (mouse)
            {
                hoveredIndex = -1;
                for (int i = 0; i < allowedRecipes.Length; i++)
                {
                    bool collided = BoundingBox.CheckPointMenuCollision(mousePos.X, mousePos.Y, frameBox, pos + new Vector2f((i * 32) % (int)size.X, ((i * 32) / (int)size.X) * 32));
                    if (collided)
                    {
                        hoveredIndex = i;
                        hoveredPosition = mousePos;
                        if(input.GetMouseClicked(InputBindings.primary, true))
                        {
                            applyRecipe?.Invoke(allowedRecipes[i], applyingPlayer);
                        }
                    }
                }
            }
        }
    }
}
