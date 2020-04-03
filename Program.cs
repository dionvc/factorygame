using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFML.Graphics;
using SFML.System;
using SFML.Window;

namespace EngineeringCorpsCS
{
    class Program: IInputSubscriber
    {
        //Entry point for the program
        static void Main(string[] args)
        {
            Program program = new Program();
            program.InitializeWindow();
            //would be nice to implement a loading screen at this point
            program.InitializeResources();
            //end loading screen
            //launch sound thread before entering main program
            program.StartMenu();

            #region debug code DELETION
            /*
            //Debug flags and stuff
            Font debugFont = new Font("SairaRegular.ttf");
            Text biomeText = new Text("default", debugFont, 32);
            Text fpsText = new Text("default", debugFont, 32);
            Text coordinates = new Text("default", debugFont, 32);
            coordinates.LineSpacing = 0.5f;
            


            while (window.IsOpen)
            {

                //The following is debugging code to be removed
                //==============================START menu debugging code
                /*
                if (drawDebugText == true)
                {
                    float fps = 1.0f / clock.ElapsedTime.AsSeconds();
                    clock.Restart();
                    float worldX = camera.GetGameView().Center.X;
                    float worldY = camera.GetGameView().Center.Y;
                    int[] cXY = SurfaceContainer.WorldToChunkCoords(worldX, worldY);
                    biomeText.DisplayedString = "Biome: " + tileCollection.GetTerrainTileName(surfaceContainer.GetTileFromWorld(worldX, worldY));
                    biomeText.Position = new Vector2f(0, 0);
                    fpsText.DisplayedString = "FPS/TPS: " + fps.ToString();
                    fpsText.Position = new Vector2f(0, 32);
                    coordinates.DisplayedString = "World Coordinates: " + worldX + ", " + worldY + 
                        "\nChunk Coordinates:" + cXY[0] + ", " + cXY[1] + 
                        "\nTile Coordinates:" + (int)(worldX / Props.tileSize) % Props.chunkSize + ", " + (int)(worldY / Props.tileSize) % Props.chunkSize +
                        "\nChunk Index:" + SurfaceContainer.WorldToChunkIndex(worldX, worldY);
                    coordinates.Position = new Vector2f(0, 64);
                    */
            #endregion debug code DELETIOn
        }


        private enum GameState
        {
            mainMenu,
            inGame,
            paused
        }
        //Essentials
        private RenderWindow window;
        private TextureContainer textureContainer;
        private FontContainer fontContainer;
        private InputManager input; //the client's input system
        private Camera camera;
        private MenuContainer menuContainer;
        private MenuFactory menuFactory; //the client;s menufactory
        private Renderer renderer;
        private GameState gameState;
        //ingame variables
        SurfaceContainer surfaceContainer;
        SurfaceGenerator surfaceGenerator;
        TileCollection tileCollection;
        //Debug variable
        Clock clock;
        float fps;

        private List<Player> players;
        private LightSource testLightSource1;
        private List<Tree> tree;

        public void InitializeWindow()
        {
            //Window initialization (all of this stays here I think)
            window = new RenderWindow(new VideoMode(1280, 720), "Engineering Corps");
            window.SetFramerateLimit(60);
            window.Closed += (s, a) => window.Close();
            window.SetActive();
            Image icon = new Image("Graphics/GUI/EngineeringCorpsIcon.png");
            window.SetIcon(icon.Size.X, icon.Size.Y, icon.Pixels);
            gameState = GameState.mainMenu;
        }
        public void InitializeResources()
        {
            //TODO: loading screen
            //Repository, Managers, Resource collections creation
            textureContainer = new TextureContainer();
            textureContainer.LoadTextures();
            fontContainer = new FontContainer();
            fontContainer.LoadFonts();
            //TODO: End loading screen

            //Input, menu, display systems
            //TODO: Investigate this cyclic couple of the menu system and input.  Input definitely needs access to menufactory.  Menucontainer may not need access to input.
            input = new InputManager(window);
            menuContainer = new MenuContainer(input);
            
            camera = new Camera();
            camera.SubscribeToInput(input);
            renderer = new Renderer(window, menuContainer, textureContainer.GetTexture("guiTilesheet"));
            menuFactory = new MenuFactory(menuContainer, renderer, this);
            window.Resized += camera.HandleResize;
            window.Resized += renderer.ResizeGUI;
            window.Resized += menuContainer.RepositionMenus;
            input.menuFactory = menuFactory;

            //Game prototypes
            tileCollection = new TileCollection(textureContainer);
        }
        public void StartMenu()
        {
            //TODO: double check this menu creation
            renderer.SubscribeToInput(input);
            menuFactory.CreateMainMenu(this, camera);

            surfaceGenerator = new SurfaceGenerator(tileCollection);
            menuFactory.CreateTestField(surfaceGenerator);
            menuFactory.CreateWorldMenu(camera, surfaceGenerator);
            while (window.IsOpen && gameState == GameState.mainMenu)
            {
                window.Clear();
                window.DispatchEvents();
                input.Update();
                renderer.RenderGUI(window, camera);
                window.Display();
            }
            if(gameState == GameState.inGame)
            {
                InitializeGame();
                RunGame();
            }
        }

        public void InitializeGame()
        {
            menuContainer.RemoveAllMenus();
            this.SubscribeToInput(input);
            //Game systems intialization
            surfaceContainer = new SurfaceContainer(tileCollection, surfaceGenerator, 3000, 1000, 196) ;
            renderer.InitializeForGame(tileCollection);
            #region test entities
            players = new List<Player>();
            tree = new List<Tree>();
            for (int i = 0; i < 16; i++)
            {
                players.Add(new Player(new Vector2(1024 + 48 * i, 1024 + 48 * i), surfaceContainer, textureContainer));
            }
            foreach (Player p in players)
            {
                //p.SubscribeToInput(input);
            }
            for(int i = 0; i < 16; i++) {
                tree.Add(new Tree(new Vector2(2064 + (256) * i, 2064), surfaceContainer, textureContainer));
            }
            testLightSource1 = new LightSource(new Vector2(1024, 1024), surfaceContainer, 2000.0f, textureContainer.GetTexture("lightsource"), players[15]);
            LightSource testLightSource2 = new LightSource(new Vector2(2048, 2048), surfaceContainer, 2000.0f, textureContainer.GetTexture("lightsource"), players[14]);
            players[15].SubscribeToInput(input);
            #endregion
            //Attaching the camera to something!
            camera.focusedEntity = players[15];
            menuFactory.CreateMinimap(camera);
            TestTilePlacer tilePlacer = new TestTilePlacer(surfaceContainer, renderer, textureContainer);
            tilePlacer.SubscribeToInput(input);
        }

        public void FinalizeGame()
        {
            //TODO: change to entitysystem clear
            menuContainer.RemoveAllMenus();
            players.Clear();
            this.UnsubscribeToInput(input);
            camera.focusedEntity = null;
            input.ClearGameSubscribers();
            renderer.DetachGameWorld();
        }

        public void RunGame()
        {
            clock = new Clock();
            while (window.IsOpen && (gameState == GameState.inGame || gameState == GameState.paused))
            {
                //Check fps
                fps = 1.0f / clock.ElapsedTime.AsSeconds();
                clock.Restart();
                //prepare for drawing and dispatch window events
                window.Clear();
                window.DispatchEvents();
                //update input
                input.Update();

                //Do not update game world if game is paused TODO: add to input pause on handling game subscriber input
                if (gameState != GameState.paused)
                {
                    //update entities
                    foreach (Player p in players)
                    {
                        p.Update();
                    }
                    List<Player> test = BoundingBox.GetCollisionListOfType<Player>(players[15]);
                    testLightSource1.Update();
                    surfaceContainer.Update();
                }
                //update camera
                camera.Update();
                //drawing game world (terrain, entities)
                renderer.RenderWorld(window, camera, surfaceContainer);
                //drawing menus (main menu, pause, ingame, etc)
                renderer.RenderGUI(window, camera);
                window.Display();
            }
            if(gameState == GameState.mainMenu)
            {
                FinalizeGame();
                StartMenu();
            }
        }
        public void SwitchToIngame()
        {
            gameState = GameState.inGame;
        }
        public void SwitchToMainMenu()
        {
            gameState = GameState.mainMenu;
        }
        public void SwitchToPauseGame()
        {
            gameState = GameState.paused;
        }
        public void ExitGame()
        {
            window.Close();
        }

        public void SubscribeToInput(InputManager input)
        {
            input.AddInputSubscriber(this, false);
        }

        public void UnsubscribeToInput(InputManager input)
        {
            input.RemoveInputSubscriber(this, false);
        }

        public void HandleInput(InputManager input)
        {
            if (input.GetKeyPressed(InputBindings.showPauseMenu, true))
            {
                menuFactory.CreatePauseMenu(this, camera);
            }
        }

        public string GetFPS()
        {
            return fps.ToString();
        }
    }
}
