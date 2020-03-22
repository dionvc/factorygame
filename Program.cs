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
            Clock clock = new Clock();


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
                    VertexArray boundingBoxArray = new VertexArray(PrimitiveType.Lines);
                    float[] pointsE = test.collisionBox.GetPoints();
                    Vector2f positionE = test.position;
                    for (int i = 0; i < pointsE.Length; i += 2)
                    {
                        boundingBoxArray.Append(new Vertex(new Vector2f(pointsE[i], pointsE[i + 1]) + positionE, Color.Magenta));
                        boundingBoxArray.Append(new Vertex(new Vector2f(pointsE[(i + 2) % 8], pointsE[(i + 3) % 8]) + positionE, Color.Magenta));
                    }

                    GUI.Draw(boundingBoxArray);
                    GUI.Draw(biomeText);
                    GUI.Draw(fpsText);
                    GUI.Draw(coordinates);
                    GUI.Display();
                }
                Sprite GUISprite = new Sprite(GUI.Texture);
                window.Draw(GUISprite);
                //==============================END menu debugging code
                //End of debugging code
                */

            //window.Display();

            //}
            #endregion debug code DELETIOn
        }


        private enum GameState
        {
            mainMenu,
            inGame
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
        TileCollection tileCollection;
        //Debug variable
        private List<Player> players;
        private List<Tree> tree;

        public void InitializeWindow()
        {
            //Window initialization (all of this stays here I think)
            window = new RenderWindow(new VideoMode(1280, 720), "Engineering Corps");
            window.SetFramerateLimit(60);
            window.Closed += (s, a) => window.Close();
            window.SetActive();
            Image icon = new Image("Graphics/EngineeringCorpsIcon.png");
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
            menuFactory = new MenuFactory(menuContainer);
            input.menuFactory = menuFactory;
            camera = new Camera();
            camera.SubscribeToInput(input);
            renderer = new Renderer(window, menuContainer);
            window.Resized += camera.HandleResize;
            window.Resized += renderer.ResizeGUI;
            window.Resized += menuContainer.RepositionMenus;
        }
        public void StartMenu()
        {
            //TODO: double check this menu creation
            renderer.SubscribeToInput(input);
            menuFactory.CreateMainMenu(this, camera);
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
            tileCollection = new TileCollection(textureContainer);
            surfaceContainer = new SurfaceContainer(tileCollection);
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
            players[15].SubscribeToInput(input);
            #endregion
            //Attaching the camera to something!
            camera.focusedEntity = players[15];
            menuFactory.CreateMinimap(renderer, camera);
            TestTilePlacer tilePlacer = new TestTilePlacer(surfaceContainer, renderer);
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
            while (window.IsOpen && gameState == GameState.inGame)
            {
                //prepare for drawing and dispatch window events
                window.Clear();
                window.DispatchEvents();
                //update input
                input.Update();
                
                //update entities
                foreach (Player p in players)
                {
                    p.Update();
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
        public void BeginGame()
        {
            gameState = GameState.inGame;
        }
        public void ExitToMenu()
        {
            gameState = GameState.mainMenu;
        }
        public void ExitGame()
        {
            window.Close();
        }

        public void SubscribeToInput(InputManager input)
        {
            input.AddInputSubscriber(this, true);
        }

        public void UnsubscribeToInput(InputManager input)
        {
            input.RemoveInputSubscriber(this, true);
        }

        public void HandleInput(InputManager input)
        {
            if (input.ConsumeKeyPress(InputBindings.showPauseMenu))
            {
                menuFactory.CreateMainMenu(this, camera);
            }
        }
    }
}
