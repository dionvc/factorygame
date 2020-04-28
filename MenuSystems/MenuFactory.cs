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
        Program program;
        FontContainer fontContainer;
        TextureAtlases textureAtlases;
        Camera camera;
        public MenuFactory(Camera camera, MenuContainer menuContainer, Renderer renderer, Program program, TextureAtlases textureAtlases, FontContainer fontContainer)
        {
            this.menuContainer = menuContainer;
            this.renderer = renderer;
            this.program = program;
            this.fontContainer = fontContainer;
            this.textureAtlases = textureAtlases;
            this.camera = camera;
        }

        public void CreateMainMenu()
        {
            Font titleFont = fontContainer.GetFont("SairaRegular");
            MenuPanel mainMenu = new MenuPanel(new Vector2i(0, 0), new Vector2i(200, 200), new FloatRect(0,0,96,96), 4, null, new Color(255,255,255,224));
            MenuButton newGameButton = new MenuButton(new Vector2i(150, 50), program.CreateMapGenMenu);
            MenuText startGameText = new MenuText(new Vector2i(150,50), titleFont, "New Game", 24, 0.6f); 
            mainMenu.AttachComponent(newGameButton);
            newGameButton.AttachComponent(startGameText);
            MenuText quitGameText = new MenuText(new Vector2i(150, 50), titleFont, "Quit Game", 24, 0.6f);
            MenuButton quitGameButton = new MenuButton(new Vector2i(150, 50), program.SwitchGameState);
            quitGameButton.tag = "exit";
            mainMenu.AttachComponent(quitGameButton);
            quitGameButton.AttachComponent(quitGameText);

            //Component adjustment
            mainMenu.SetInitialPosition(camera.GetGUIView());
            newGameButton.SetPivots("top", "center", "inside", 25);
            newGameButton.SetInitialPosition(camera.GetGUIView());
            quitGameButton.SetPivots("bottom", "center", "outside", 0);
            quitGameButton.SetInitialPosition(newGameButton);

            menuContainer.AttachMenu(mainMenu);
        }

        public void CreatePauseMenu()
        {
            Font pauseMenuFont = fontContainer.GetFont("SairaRegular");

            MenuPanel pausemenu = new MenuPanel(new Vector2i(0, 0), new Vector2i(200, 200), new FloatRect(0, 0, 96, 96), 4, program.SwitchGameState, new Color(255, 255, 255, 224));
            pausemenu.tag = "inGame";

            MenuText resumeGameText = new MenuText(new Vector2i(150, 50), pauseMenuFont, "Resume Game", 24, 0.6f);
            MenuButton resumeGameButton = new MenuButton(new Vector2i(150, 50), pausemenu.ClosePanel);
            resumeGameButton.tag = "inGame";

            MenuText endGameText = new MenuText(new Vector2i(150, 50), pauseMenuFont, "Return to Title", 24, 0.6f);
            MenuButton endGameButton = new MenuButton(new Vector2i(150, 50), program.SwitchGameState);
            endGameButton.tag = "mainMenu";
            
            MenuText quitGameText = new MenuText(new Vector2i(150, 50), pauseMenuFont, "Quit Game", 24, 0.6f);
            MenuButton quitGameButton = new MenuButton(new Vector2i(150, 50), program.SwitchGameState);
            quitGameButton.tag = "exit";

            pausemenu.AttachComponent(resumeGameButton);
            resumeGameButton.AttachComponent(resumeGameText);
            pausemenu.AttachComponent(endGameButton);
            endGameButton.AttachComponent(endGameText);
            pausemenu.AttachComponent(quitGameButton);
            quitGameButton.AttachComponent(quitGameText);

            pausemenu.closePanelKey = InputBindings.showPauseMenu;
            pausemenu.SetPivots("center", "center", "inside", 0);
            pausemenu.SetInitialPosition(camera.GetGUIView());
            resumeGameButton.SetPivots("top", "center", "inside", 25);
            resumeGameButton.SetInitialPosition(camera.GetGUIView());
            endGameButton.SetPivots("bottom", "center", "outside", 0);
            endGameButton.SetInitialPosition(resumeGameButton);
            quitGameButton.SetPivots("bottom", "center", "outside", 0);
            quitGameButton.SetInitialPosition(endGameButton);

            menuContainer.AttachMenu(pausemenu);

            //Pause the game
            program.SwitchGameState("paused");
        }

        public void CreateDebugMenu()
        {
            Font debugFont = fontContainer.GetFont("SairaRegular");
            MenuPanel debugMenu = new MenuPanel(new Vector2i(720, 0), new Vector2i(300, 150), new FloatRect(0, 0, 96, 96), 4, null, new Color(255, 255, 255, 224));
            MenuButton boundingBoxButton = new MenuButton(new Vector2i(100, 100), renderer.ToggleBoundingBoxRendering);
            MenuText boundingBoxButtonText = new MenuText(new Vector2i(100,100), debugFont, "Bounding Boxes", 24, 0.6f);
            MenuDynamicText fps = new MenuDynamicText(new Vector2i(100, 50), debugFont, "FPS: {0}\nRendered Entity Count: {1}\n,Rendered Sprite Count: {2}", 24, new MenuDynamicText.DynamicString[] { program.GetFPS, renderer.GetRenderedEntityCount, renderer.GetRenderedSpriteCount });
            debugMenu.AttachComponent(boundingBoxButton);
            debugMenu.AttachComponent(fps);
            boundingBoxButton.AttachComponent(boundingBoxButtonText);
            debugMenu.closePanelKey = InputBindings.showDebugMenu;
            menuContainer.AttachMenu(debugMenu);
        }

        public void CreateMinimap()
        {
            MenuPanel minimapPanel = new MenuPanel(new Vector2i(0, 0), new Vector2i(300, 350), new FloatRect(0, 0, 96, 96), 4, null, new Color(255, 255, 255, 224));
            MenuWorldMap minimap = new MenuWorldMap(new Vector2i(250, 250), camera, renderer);
            minimap.mapScale = 1.25f;
            MenuButton minimapPollutionToggle = new MenuButton(new Vector2i(50, 50), minimap.TogglePollution);
            minimapPanel.AttachComponent(minimap);
            minimapPanel.AttachComponent(minimapPollutionToggle);
            minimapPollutionToggle.SetInitialPosition(camera.GetGUIView());
            minimapPanel.closePanelKey = InputBindings.showMinimap;
            minimapPanel.SetPivots("top", "right", "inside", 0);
            minimapPanel.SetInitialPosition(camera.GetGUIView());
            minimapPanel.lockedPosition = true;
            minimap.SetPivots("top", "center", "inside", 25);
            minimap.SetInitialPosition(camera.GetGUIView());
            minimapPollutionToggle.SetPivots("bottom", "center", "inside", 25);
            minimapPollutionToggle.SetInitialPosition(camera.GetGUIView());
            menuContainer.AttachMenu(minimapPanel);
        }

        public void CreateWorldMap()
        {
            Vector2i screenSize = new Vector2i((int)camera.GetGUIView().Size.X, (int)camera.GetGUIView().Size.Y);
            MenuPanel minimapPanel = new MenuPanel(new Vector2i(0, 0), screenSize, new FloatRect(0, 0, 96, 96), 4, null, new Color(255, 255, 255, 224));
            MenuWorldMap minimap = new MenuWorldMap(screenSize - new Vector2i(50, 50), camera, renderer);
            MenuButton minimapPollutionToggle = new MenuButton(new Vector2i(50, 50), minimap.TogglePollution);
            minimapPanel.AttachComponent(minimap);
            minimapPanel.AttachComponent(minimapPollutionToggle);
            minimap.controllable = true;
            minimapPollutionToggle.SetInitialPosition(camera.GetGUIView());
            minimapPanel.closePanelKey = InputBindings.showWorldMap;
            minimapPanel.SetInitialPosition(camera.GetGUIView());
            minimapPanel.lockedPosition = true;
            renderer.ToggleCullingMinimap("");
            minimapPanel.ClosePanelAction = renderer.ToggleCullingMinimap;
            minimap.SetInitialPosition(camera.GetGUIView());
            menuContainer.AttachMenu(minimapPanel);
        }


        public void CreateMapGenMenu(SurfaceGenerator surfaceGenerator)
        {
            //Declaring elements
            Font mapGenFont = fontContainer.GetFont("SairaRegular");
            MenuPanel worldMenu = new MenuPanel(new Vector2i(0, 0), new Vector2i(500,500), new FloatRect(0, 0, 96, 96), 4, null, new Color(255, 255, 255, 224));
            //Noise dropdowns
            string[] noiseNames = Enum.GetNames(typeof(FastNoise.NoiseType));
            int[] noiseValues = new int[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };
            MenuListBox moistureNoiseType = new MenuListBox(new Vector2i(150,25), noiseNames, noiseValues, surfaceGenerator.SetNoiseType, mapGenFont, 24, 24, 0);
            moistureNoiseType.tag = "moisture";
            MenuListBox elevationNoiseType = new MenuListBox(new Vector2i(150, 25), noiseNames, noiseValues, surfaceGenerator.SetNoiseType, mapGenFont, 24, 24, 0);
            elevationNoiseType.tag = "elevation";
            MenuListBox temperatureNoiseType = new MenuListBox(new Vector2i(150, 25), noiseNames, noiseValues, surfaceGenerator.SetNoiseType, mapGenFont, 24, 24, 0);
            temperatureNoiseType.tag = "temperature";
            //Surface size field
            MenuText surfaceSizeFieldTitle = new MenuText(new Vector2i(100, 25), mapGenFont, "Map size:", 24, 24);
            MenuField surfaceSizeField = new MenuField(new Vector2i(150,25), mapGenFont, surfaceGenerator.ParseString);
            surfaceSizeField.tag = "surfacesize";
            surfaceSizeField.AttachComponent(surfaceSizeFieldTitle);
            surfaceSizeFieldTitle.SetTextPosition("right", "center");
            surfaceSizeFieldTitle.SetPivots("left", "center", "outside", 0);
            surfaceSizeFieldTitle.SetInitialPosition(surfaceSizeField);
            //Seed field
            MenuText seedFieldTitle = new MenuText(new Vector2i(100, 25), mapGenFont, "Map Seed:", 24, 24);
            MenuField seedField = new MenuField(new Vector2i(150, 25), mapGenFont, surfaceGenerator.ParseString);
            seedField.tag = "seed";
            seedField.AttachComponent(seedFieldTitle);
            seedFieldTitle.SetTextPosition("right", "center");
            seedFieldTitle.SetPivots("left", "center", "outside", 0);
            seedFieldTitle.SetInitialPosition(seedField);
            //Start game button
            MenuButton startGame = new MenuButton(new Vector2i(100, 25), program.SwitchGameState);
            startGame.tag = "inGame";
            //Slider
            MenuSlider elevationFactor = new MenuSlider(5, surfaceGenerator.SetNoiseFactor, 0.5f, 1.5f, 1.0f);
            elevationFactor.tag = "elevation";
            MenuSlider moistureFactor = new MenuSlider(5, surfaceGenerator.SetNoiseFactor, 0.5f, 1.5f, 1.0f);
            moistureFactor.tag = "moisture";
            MenuSlider temperatureFactor = new MenuSlider(5, surfaceGenerator.SetNoiseFactor, 0.5f, 1.5f, 1.0f);
            temperatureFactor.tag = "temperature";
            //Attaching Elements
            worldMenu.AttachComponent(startGame);
            worldMenu.AttachComponent(seedField);
            worldMenu.AttachComponent(surfaceSizeField);
            worldMenu.AttachComponent(temperatureNoiseType);
            worldMenu.AttachComponent(elevationNoiseType);
            worldMenu.AttachComponent(moistureNoiseType);


            worldMenu.AttachComponent(elevationFactor);
            worldMenu.AttachComponent(moistureFactor);
            worldMenu.AttachComponent(temperatureFactor);
            //Positioning elements
            seedField.SetPivots("bottom", "center", "outside", 25);
            seedField.SetInitialPosition(moistureNoiseType);
            surfaceSizeField.SetPivots("bottom", "center", "outside", 25);
            surfaceSizeField.SetInitialPosition(seedField);
            startGame.SetPivots("bottom", "right", "inside", 25);
            startGame.SetInitialPosition();
            worldMenu.SetInitialPosition(camera.GetGUIView());
            elevationFactor.SetInitialPosition(camera.GetGUIView());
            moistureFactor.SetPivots("bottom", "center", "outside", 25);
            moistureFactor.SetInitialPosition(elevationFactor);
            temperatureFactor.SetPivots("bottom", "center", "outside", 25);
            temperatureFactor.SetInitialPosition(moistureFactor);
            moistureNoiseType.SetPivots("top", "center", "inside", 25);
            moistureNoiseType.SetInitialPosition(camera.GetGUIView());
            elevationNoiseType.SetPivots("bottom", "center", "outside", 25);
            elevationNoiseType.SetInitialPosition(moistureNoiseType);
            temperatureNoiseType.SetPivots("bottom", "center", "outside", 25);
            temperatureNoiseType.SetInitialPosition(elevationNoiseType);
            //Finalize into menu container
            menuContainer.AttachMenu(worldMenu);
        }

        public void CreatePlayerInventory(Player accessingPlayer, ItemStack[] inventory)
        {
            Font itemFont = fontContainer.GetFont("SairaRegular");
            MenuPanel inventoryPanel = new MenuPanel(new Vector2i(0, 0), new Vector2i(300, 300), new FloatRect(0, 0, 96, 96), 8, null, new Color(255, 255, 255, 224));
            inventoryPanel.closePanelKey = InputBindings.showInventory;
            MenuInventory inventoryMenu = new MenuInventory(new Vector2i(256, 256), inventory, accessingPlayer, accessingPlayer, itemFont);
            inventoryPanel.AttachComponent(inventoryMenu);
            menuContainer.AttachMenu(inventoryPanel);

            menuContainer.ClosePanelsWithTag("EntityGUI");
            inventoryPanel.panelTag = "EntityGUI";
        }

        public MenuComponent CreatePlayerProgressBar(Camera camera, Player player, string tag, Color color, MenuComponent relativeComponent)
        {
            MenuProgressBar progressBar = new MenuProgressBar(new Vector2i(512, 16), new FloatRect(0,0,32,32), new FloatRect(0,0,32,32), player.GetProgress, color);
            progressBar.tag = tag;
            if (relativeComponent == null)
            {
                progressBar.SetPivots("center", "bottom", "inside", 8);
                progressBar.SetInitialPosition(camera.GetGUIView());
            }
            progressBar.hideWhenEmpty = true;
            progressBar.hideWhenFull = true;
            menuContainer.AttachMenu(progressBar);
            return progressBar;
        }

        public MenuComponent CreateMachineInterface(Machine machine, Player player)
        {
            Font menuFont = fontContainer.GetFont("SairaRegular");
            MenuPanel machinePanel = new MenuPanel(new Vector2i(0, 0), new Vector2i(500, 200), new FloatRect(0, 0, 96, 96), 8, null, new Color(255, 255, 255, 224));
            MenuInventory inputInventory = new MenuInventory(new Vector2i(96, 64), machine.input, player, machine, menuFont);
            MenuInventory outputInventory = new MenuInventory(new Vector2i(96, 64), machine.result, player, machine, menuFont);
            MenuProgressBar recipeProgress = new MenuProgressBar(new Vector2i(128, 16), new FloatRect(0, 0, 32, 32), new FloatRect(0, 0, 32, 32), machine.GetProgress, new Color(196, 92, 0));
            MenuDrawable machineGraphic = new MenuDrawable(new Vector2i(128,128), machine, 0);
            machinePanel.AttachComponent(inputInventory);
            machinePanel.AttachComponent(recipeProgress);
            machinePanel.AttachComponent(outputInventory);
            machinePanel.AttachComponent(machineGraphic);
            inputInventory.SetInitialPosition();
            recipeProgress.SetPivots("right", "center", "outside", 0);
            recipeProgress.SetInitialPosition(inputInventory);
            outputInventory.SetPivots("right", "center", "outside", 0);
            outputInventory.SetInitialPosition(inputInventory);
            menuContainer.AttachMenu(machinePanel);
            machinePanel.closePanelKey = InputBindings.showInventory;
            menuContainer.ClosePanelsWithTag("EntityGUI");
            machinePanel.panelTag = "EntityGUI";

            return machinePanel;
        }
    }
}
