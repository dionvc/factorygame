﻿using System;
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
        TextureContainer textureContainer;
        public MenuFactory(MenuContainer menuContainer, Renderer renderer, Program program, TextureContainer textureContainer, FontContainer fontContainer)
        {
            this.menuContainer = menuContainer;
            this.renderer = renderer;
            this.program = program;
            this.fontContainer = fontContainer;
            this.textureContainer = textureContainer;
        }

        public void CreateMainMenu(Camera camera)
        {
            Font titleFont = fontContainer.GetFont("SairaRegular");
            MenuPanel mainMenu = new MenuPanel(new Vector2f(0, 0), new Vector2f(200, 200), new FloatRect(0,0,96,96), 4);
            MenuButton newGameButton = new MenuButton(new Vector2f(150, 50), program.CreateMapGenMenu);
            MenuText startGameText = new MenuText(new Vector2f(150,50), titleFont, "New Game", 24, 0.6f); 
            mainMenu.AttachComponent(newGameButton);
            newGameButton.AttachComponent(startGameText);
            MenuText quitGameText = new MenuText(new Vector2f(150, 50), titleFont, "Quit Game", 24, 0.6f);
            MenuButton quitGameButton = new MenuButton(new Vector2f(150, 50), program.ExitGame);
            mainMenu.AttachComponent(quitGameButton);
            quitGameButton.AttachComponent(quitGameText);

            //Component adjustment
            mainMenu.closePanelKey = InputBindings.showPauseMenu;
            mainMenu.SetInitialPosition(camera.GetGUIView());
            newGameButton.SetPivots("top", "center", "inside", 25.0f);
            newGameButton.SetInitialPosition(camera.GetGUIView());
            quitGameButton.SetPivots("bottom", "center", "outside", 0.0f);
            quitGameButton.SetInitialPosition(newGameButton);

            menuContainer.AttachMenu(mainMenu);
        }

        public void CreatePauseMenu(Camera camera)
        {
            Font pauseMenuFont = fontContainer.GetFont("SairaRegular");
            MenuPanel pausemenu = new MenuPanel(new Vector2f(0, 0), new Vector2f(200, 200), new FloatRect(0, 0, 96, 96), 4);
            MenuButton startGameButton = new MenuButton(new Vector2f(150, 50), pausemenu.ClosePanel);
            MenuText startGameText = new MenuText(new Vector2f(150, 50), pauseMenuFont, "Resume Game", 24, 0.6f);
            pausemenu.AttachComponent(startGameButton);
            startGameButton.AttachComponent(startGameText);
            MenuText endGameText = new MenuText(new Vector2f(150, 50), pauseMenuFont, "Return to Title", 24, 0.6f);
            MenuButton endGameButton = new MenuButton(new Vector2f(150, 50), program.SwitchToMainMenu);
            pausemenu.AttachComponent(endGameButton);
            endGameButton.AttachComponent(endGameText);
            MenuText quitGameText = new MenuText(new Vector2f(150, 50), pauseMenuFont, "Quit Game", 24, 0.6f);
            MenuButton quitGameButton = new MenuButton(new Vector2f(150, 50), program.ExitGame);
            pausemenu.AttachComponent(quitGameButton);
            quitGameButton.AttachComponent(quitGameText);

            pausemenu.closePanelKey = InputBindings.showPauseMenu;
            pausemenu.ClosePanelAction = program.SwitchToIngame;
            pausemenu.SetPivots("center", "center", "inside", 0.0f);
            pausemenu.SetInitialPosition(camera.GetGUIView());
            startGameButton.SetPivots("top", "center", "inside", 25.0f);
            startGameButton.SetInitialPosition(camera.GetGUIView());
            endGameButton.SetPivots("bottom", "center", "outside", 0.0f);
            endGameButton.SetInitialPosition(startGameButton);
            quitGameButton.SetPivots("bottom", "center", "outside", 0.0f);
            quitGameButton.SetInitialPosition(endGameButton);
            menuContainer.AttachMenu(pausemenu);

            //Pause the game
            program.SwitchToPauseGame();
        }

        public void CreateDebugMenu()
        {
            Font debugFont = fontContainer.GetFont("SairaRegular");
            MenuPanel debugMenu = new MenuPanel(new Vector2f(720, 0), new Vector2f(300, 150), new FloatRect(0, 0, 96, 96), 4);
            MenuButton boundingBoxButton = new MenuButton(new Vector2f(100, 100), renderer.ToggleBoundingBoxRendering);
            MenuText boundingBoxButtonText = new MenuText(new Vector2f(100,100), debugFont, "Bounding Boxes", 24, 0.6f);
            MenuDynamicText fps = new MenuDynamicText(new Vector2f(100, 50), debugFont, "Fps: {0}", 24, new MenuDynamicText.DynamicString[] { program.GetFPS });
            debugMenu.AttachComponent(boundingBoxButton);
            debugMenu.AttachComponent(fps);
            boundingBoxButton.AttachComponent(boundingBoxButtonText);
            debugMenu.closePanelKey = InputBindings.showDebugMenu;
            menuContainer.AttachMenu(debugMenu);
        }

        public void CreateMinimap(Camera camera)
        {
            MenuPanel minimapPanel = new MenuPanel(new Vector2f(0, 0), new Vector2f(300, 350), new FloatRect(0, 0, 96, 96), 4);
            MenuWorldMap minimap = new MenuWorldMap(new Vector2f(250, 250), camera, renderer);
            MenuButton minimapPollutionToggle = new MenuButton(new Vector2f(50, 50), minimap.TogglePollution);
            minimapPanel.AttachComponent(minimap);
            minimapPanel.AttachComponent(minimapPollutionToggle);
            minimapPollutionToggle.SetInitialPosition(camera.GetGUIView());
            minimapPanel.closePanelKey = InputBindings.showMinimap;
            minimapPanel.SetPivots("top", "right", "inside", 0.0f);
            minimapPanel.SetInitialPosition(camera.GetGUIView());
            minimapPanel.lockedPosition = true;
            minimap.SetPivots("top", "center", "inside", 25.0f);
            minimap.SetInitialPosition(camera.GetGUIView());
            minimapPollutionToggle.SetPivots("bottom", "center", "inside", 25.0f);
            minimapPollutionToggle.SetInitialPosition(camera.GetGUIView());
            menuContainer.AttachMenu(minimapPanel);
        }

        public void CreateWorldMap(Camera camera)
        {
            MenuPanel minimapPanel = new MenuPanel(new Vector2f(0, 0), camera.GetGUIView().Size, new FloatRect(0, 0, 96, 96), 4);
            MenuWorldMap minimap = new MenuWorldMap(camera.GetGUIView().Size - new Vector2f(50, 50), camera, renderer);
            MenuButton minimapPollutionToggle = new MenuButton(new Vector2f(50, 50), minimap.TogglePollution);
            minimapPanel.AttachComponent(minimap);
            minimapPanel.AttachComponent(minimapPollutionToggle);
            minimap.controllable = true;
            minimapPollutionToggle.SetInitialPosition(camera.GetGUIView());
            minimapPanel.closePanelKey = InputBindings.showWorldMap;
            minimapPanel.SetInitialPosition(camera.GetGUIView());
            minimapPanel.lockedPosition = true;
            renderer.ToggleCullingMinimap();
            minimapPanel.ClosePanelAction = renderer.ToggleCullingMinimap;
            minimap.SetInitialPosition(camera.GetGUIView());
            menuContainer.AttachMenu(minimapPanel);
        }


        public void CreateMapGenMenu(SurfaceGenerator surfaceGenerator, Camera camera)
        {
            //Declaring elements
            Font mapGenFont = fontContainer.GetFont("SairaRegular");
            MenuPanel worldMenu = new MenuPanel(new Vector2f(0, 0), new Vector2f(500,500), new FloatRect(0, 0, 96, 96), 4);
            //Noise dropdowns
            string[] noiseNames = Enum.GetNames(typeof(FastNoise.NoiseType));
            int[] noiseValues = new int[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };
            MenuListBox moistureNoiseType = new MenuListBox(new Vector2f(150,25), noiseNames, noiseValues, surfaceGenerator.SetNoiseType, mapGenFont, 24, 24, 0);
            moistureNoiseType.tag = "moisture";
            MenuListBox elevationNoiseType = new MenuListBox(new Vector2f(150, 25), noiseNames, noiseValues, surfaceGenerator.SetNoiseType, mapGenFont, 24, 24, 0);
            elevationNoiseType.tag = "elevation";
            MenuListBox temperatureNoiseType = new MenuListBox(new Vector2f(150, 25), noiseNames, noiseValues, surfaceGenerator.SetNoiseType, mapGenFont, 24, 24, 0);
            temperatureNoiseType.tag = "temperature";
            //Surface size field
            MenuText surfaceSizeFieldTitle = new MenuText(new Vector2f(100, 25), mapGenFont, "Map size:", 24, 24);
            MenuField surfaceSizeField = new MenuField(new Vector2f(150,25), mapGenFont, surfaceGenerator.ParseString);
            surfaceSizeField.tag = "surfacesize";
            surfaceSizeField.AttachComponent(surfaceSizeFieldTitle);
            surfaceSizeFieldTitle.SetPivots("left", "center", "outside", 25.0f);
            surfaceSizeFieldTitle.SetInitialPosition();
            surfaceSizeFieldTitle.SetInitialPosition(surfaceSizeField);
            //Seed field
            MenuText seedFieldTitle = new MenuText(new Vector2f(100, 25), mapGenFont, "Map Seed:", 24, 24);
            MenuField seedField = new MenuField(new Vector2f(150, 25), mapGenFont, surfaceGenerator.ParseString);
            seedField.tag = "seed";
            seedField.AttachComponent(seedFieldTitle);
            seedFieldTitle.SetPivots("left", "center", "outside", 25.0f);
            seedFieldTitle.SetInitialPosition();
            seedFieldTitle.SetInitialPosition(seedField);
            //Start game button
            MenuButton startGame = new MenuButton(new Vector2f(100, 25), program.SwitchToIngame);
            //Slider
            MenuSlider elevationFactor = new MenuSlider(new Vector2f(100, 15), surfaceGenerator.SetNoiseFactor, 0.1f, 2.0f, 1.0f);
            elevationFactor.tag = "elevation";
            MenuSlider moistureFactor = new MenuSlider(new Vector2f(100, 15), surfaceGenerator.SetNoiseFactor, 0.1f, 2.0f, 1.0f);
            moistureFactor.tag = "moisture";
            MenuSlider temperatureFactor = new MenuSlider(new Vector2f(100, 15), surfaceGenerator.SetNoiseFactor, 0.1f, 2.0f, 1.0f);
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
            seedField.SetPivots("bottom", "center", "outside", 25.0f);
            seedField.SetInitialPosition(moistureNoiseType);
            surfaceSizeField.SetPivots("bottom", "center", "outside", 25.0f);
            surfaceSizeField.SetInitialPosition(seedField);
            startGame.SetInitialPosition(camera.GetGUIView());
            worldMenu.SetInitialPosition(camera.GetGUIView());
            elevationFactor.SetInitialPosition(camera.GetGUIView());
            moistureFactor.SetPivots("bottom", "center", "outside", 25.0f);
            moistureFactor.SetInitialPosition(elevationFactor);
            temperatureFactor.SetPivots("bottom", "center", "outside", 25.0f);
            temperatureFactor.SetInitialPosition(moistureFactor);
            moistureNoiseType.SetPivots("top", "center", "inside", 25.0f);
            moistureNoiseType.SetInitialPosition(camera.GetGUIView());
            elevationNoiseType.SetPivots("bottom", "center", "outside", 25.0f);
            elevationNoiseType.SetInitialPosition(moistureNoiseType);
            temperatureNoiseType.SetPivots("bottom", "center", "outside", 25.0f);
            temperatureNoiseType.SetInitialPosition(elevationNoiseType);
            //Finalize into menu container
            menuContainer.AttachMenu(worldMenu);
        }
    }
}
