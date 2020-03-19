using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFML.System;
using SFML.Graphics;

namespace EngineeringCorpsCS
{
    class MenuFactory
    {
        MenuContainer menuContainer;
        Font debugFont = new Font("SairaRegular.ttf");
        public MenuFactory(MenuContainer menuContainer)
        {
            this.menuContainer = menuContainer;
        }

        public void CreateMainMenu(Program program)
        {
            MenuPanel mainMenu = new MenuPanel(new Vector2f(0, 0), new Vector2f(300, 150), new bool[] { true, true });
            MenuButton startGameButton = new MenuButton(new Vector2f(25, 25), program.BeginGame, new Vector2f(100, 100));
            MenuText startGameText = new MenuText(new Vector2f(0, 0), new Vector2f(100, 100), new bool[] { true, true }, debugFont, "Start Game", 24); 
            mainMenu.AttachComponent(startGameButton);
            startGameButton.AttachComponent(startGameText);
            MenuText endGameText = new MenuText(new Vector2f(0, 0), new Vector2f(100, 100), new bool[] { true, true }, debugFont, "End Game", 24);
            MenuButton endGameButton = new MenuButton(new Vector2f(125, 25), program.ExitToMenu, new Vector2f(100, 100));
            mainMenu.AttachComponent(endGameButton);
            endGameButton.AttachComponent(endGameText);
            MenuText quitGameText = new MenuText(new Vector2f(0, 0), new Vector2f(100, 100), new bool[] { true, true }, debugFont, "Quit Game", 24);
            MenuButton quitGameButton = new MenuButton(new Vector2f(225, 25), program.ExitGame, new Vector2f(100, 100));
            mainMenu.AttachComponent(quitGameButton);
            quitGameButton.AttachComponent(quitGameText);
            mainMenu.closePanelKey = InputBindings.showPauseMenu;
            menuContainer.AttachMenu(mainMenu);
        }

        public void CreateDebugMenu(Renderer renderer)
        {
            MenuPanel debugMenu = new MenuPanel(new Vector2f(720, 0), new Vector2f(300, 150), new bool[] { true, true });
            MenuButton boundingBoxButton = new MenuButton(new Vector2f(25, 25), renderer.ToggleBoundingBoxRendering, new Vector2f(100, 100));
            MenuText boundingBoxButtonText = new MenuText(new Vector2f(0, 0), new Vector2f(100, 100), new bool[] { true, true }, debugFont, "Show/Hide bounding boxes", 24);
            debugMenu.AttachComponent(boundingBoxButton);
            boundingBoxButton.AttachComponent(boundingBoxButtonText);
            debugMenu.closePanelKey = InputBindings.showDebugMenu;
            menuContainer.AttachMenu(debugMenu);
        }
    }
}
