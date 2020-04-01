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
        Renderer renderer;
        Font debugFont = new Font("SairaRegular.ttf");
        public MenuFactory(MenuContainer menuContainer, Renderer renderer)
        {
            this.menuContainer = menuContainer;
            this.renderer = renderer;
        }

        public void CreateMainMenu(Program program, Camera camera)
        {
            MenuPanel mainMenu = new MenuPanel(new Vector2f(0, 0), new Vector2f(300, 150));
            MenuButton startGameButton = new MenuButton(new Vector2f(25, 25), new Vector2f(100, 100), program.BeginGame);
            MenuText startGameText = new MenuText(new Vector2f(0, 0), debugFont, "Start Game", 24); 
            mainMenu.AttachComponent(startGameButton);
            startGameButton.AttachComponent(startGameText);
            MenuText endGameText = new MenuText(new Vector2f(0, 0), debugFont, "End Game", 24);
            MenuButton endGameButton = new MenuButton(new Vector2f(125, 25), new Vector2f(100, 100), program.ExitToMenu);
            mainMenu.AttachComponent(endGameButton);
            endGameButton.AttachComponent(endGameText);
            MenuText quitGameText = new MenuText(new Vector2f(0, 0), debugFont, "Quit Game", 24);
            MenuButton quitGameButton = new MenuButton(new Vector2f(225, 25), new Vector2f(100, 100), program.ExitGame);
            mainMenu.AttachComponent(quitGameButton);
            quitGameButton.AttachComponent(quitGameText);

            //Component adjustment
            mainMenu.closePanelKey = InputBindings.showPauseMenu;
            mainMenu.pivot1 = "center";
            mainMenu.pivot2 = "center";
            mainMenu.SetInitialPosition(camera.GetGUIView());
            menuContainer.AttachMenu(mainMenu);
        }

        public void CreatePauseMenu(Program program, Camera camera)
        {
            MenuPanel mainMenu = new MenuPanel(new Vector2f(0, 0), new Vector2f(300, 150));
            MenuButton startGameButton = new MenuButton(new Vector2f(25, 25), new Vector2f(100, 100), program.BeginGame);
            MenuText startGameText = new MenuText(new Vector2f(0, 0), debugFont, "Start Game", 24);
            mainMenu.AttachComponent(startGameButton);
            startGameButton.AttachComponent(startGameText);
            MenuText endGameText = new MenuText(new Vector2f(0, 0), debugFont, "End Game", 24);
            MenuButton endGameButton = new MenuButton(new Vector2f(125, 25), new Vector2f(100, 100), program.ExitToMenu);
            mainMenu.AttachComponent(endGameButton);
            endGameButton.AttachComponent(endGameText);
            MenuText quitGameText = new MenuText(new Vector2f(0, 0), debugFont, "Quit Game", 24);
            MenuButton quitGameButton = new MenuButton(new Vector2f(225, 25), new Vector2f(100, 100), program.ExitGame);
            mainMenu.AttachComponent(quitGameButton);
            quitGameButton.AttachComponent(quitGameText);
            mainMenu.closePanelKey = InputBindings.showPauseMenu;
            mainMenu.ClosePanelAction = program.BeginGame;
            program.PauseGame();
            mainMenu.pivot1 = "center";
            mainMenu.pivot2 = "center";
            mainMenu.SetInitialPosition(camera.GetGUIView());
            menuContainer.AttachMenu(mainMenu);
        }

        public void CreateDebugMenu()
        {
            MenuPanel debugMenu = new MenuPanel(new Vector2f(720, 0), new Vector2f(300, 150));
            MenuButton boundingBoxButton = new MenuButton(new Vector2f(25, 25), new Vector2f(100, 100), renderer.ToggleBoundingBoxRendering);
            MenuText boundingBoxButtonText = new MenuText(new Vector2f(0, 0), debugFont, "Show/Hide boundinggggggggggggggggg boxes", 24);
            debugMenu.AttachComponent(boundingBoxButton);
            boundingBoxButton.AttachComponent(boundingBoxButtonText);
            debugMenu.closePanelKey = InputBindings.showDebugMenu;
            menuContainer.AttachMenu(debugMenu);
        }

        public void CreateMinimap(Camera camera)
        {
            MenuPanel minimapPanel = new MenuPanel(new Vector2f(0, 0), new Vector2f(300, 300));
            MenuWorldMap minimap = new MenuWorldMap(camera, renderer, new Vector2f(25, 25), new Vector2f(250, 250));
            MenuButton minimapPollutionToggle = new MenuButton(new Vector2f(25, 275), new Vector2f(50, 50), minimap.TogglePollution);
            minimapPanel.AttachComponent(minimap);
            minimapPanel.AttachComponent(minimapPollutionToggle);
            minimapPollutionToggle.SetInitialPosition(camera.GetGUIView());
            minimapPanel.closePanelKey = InputBindings.showMinimap;
            minimapPanel.pivot1 = "top";
            minimapPanel.pivot2 = "right";
            minimapPanel.SetInitialPosition(camera.GetGUIView());
            minimapPanel.lockedPosition = true;
            menuContainer.AttachMenu(minimapPanel);
        }

        public void CreateWorldMap(Camera camera)
        {
            MenuPanel minimapPanel = new MenuPanel(new Vector2f(0, 0), camera.GetGUIView().Size);
            MenuWorldMap minimap = new MenuWorldMap(camera, renderer, new Vector2f(25, 25), camera.GetGUIView().Size - new Vector2f(50,50));
            MenuButton minimapPollutionToggle = new MenuButton(new Vector2f(25, 275), new Vector2f(50, 50), minimap.TogglePollution);
            minimapPanel.AttachComponent(minimap);
            minimapPanel.AttachComponent(minimapPollutionToggle);
            minimap.controllable = true;
            minimapPollutionToggle.pivot1 = "bottom";
            minimapPollutionToggle.SetInitialPosition(camera.GetGUIView());
            minimapPanel.closePanelKey = InputBindings.showWorldMap;
            minimapPanel.pivot1 = "top";
            minimapPanel.pivot2 = "left";
            minimapPanel.SetInitialPosition(camera.GetGUIView());
            minimapPanel.lockedPosition = true;
            menuContainer.AttachMenu(minimapPanel);
        }
    }
}
