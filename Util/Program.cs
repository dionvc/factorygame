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
            while (program.window.IsOpen)
            {
                if (program.gameState == GameState.mainMenu)
                {
                    program.StartMenu();
                }
                else if(program.gameState == GameState.inGame)
                {
                    program.RunGame();
                }
            }

            #region debug code DELETION
            /*
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


        public enum GameState
        {
            mainMenu,
            inGame,
            paused
        }
        //Essentials
        private RenderWindow window;
        private TextureAtlases textureAtlases;
        private FontContainer fontContainer;
        private InputManager input; //the client's input system
        private Camera camera;
        private MenuContainer menuContainer;
        private MenuFactory menuFactory; //the client;s menufactory
        private Renderer renderer;
        private GameState gameState;
        EntityUpdateSystem entityUpdateSystem;
        //ingame variables
        SurfaceContainer surfaceContainer;
        SurfaceGenerator surfaceGenerator;
        //Prototype/Flyweight collections
        TileCollection tileCollection;
        EntityCollection entityCollection;
        ItemCollection itemCollection;
        //Debug variable
        Clock clock;
        float fps;
        Queue<float> fpsQueue;
        public int frame;

        private Entity player;

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
            //Loading text/splashscreen loading
            Image icon = new Image("Graphics/GUI/EngineeringCorpsIcon.png");
            Texture loadingTexture = new Texture(icon);
            Sprite loadingTitle = new Sprite(loadingTexture);
            Font loadingFont = new Font("Fonts/SairaRegular.ttf");
            Text loadingText = new Text("",loadingFont);
            loadingText.DisplayedString = "Constructing Texture Atlases...";
            loadingText.Origin = new Vector2f(loadingText.GetGlobalBounds().Width / 2, loadingText.GetGlobalBounds().Height/2);
            loadingText.Position = new Vector2f(window.Size.X / 2, window.Size.Y / 2);
            loadingTitle.Origin = new Vector2f(loadingTexture.Size.X/2, loadingTexture.Size.Y / 2);
            loadingTitle.Position = new Vector2f(window.Size.X / 2, window.Size.Y / 4);

            //Loading textures
            window.Clear(Color.Black);
            window.Draw(loadingTitle);
            window.Draw(loadingText);
            window.Display();
            textureAtlases = new TextureAtlases();
            textureAtlases.LoadTextures(Props.packTogether);

            //Loading fonts
            loadingText.DisplayedString = "Loading Fonts...";
            loadingText.Origin = new Vector2f(loadingText.GetGlobalBounds().Width / 2, loadingText.GetGlobalBounds().Height / 2);
            window.Clear(Color.Black);
            window.Draw(loadingTitle);
            window.Draw(loadingText);
            window.Display();
            fontContainer = new FontContainer();
            fontContainer.LoadFonts();

            //Initializing Input
            //TODO: Investigate this cyclic couple of the menu system and input.  Input definitely needs access to menufactory.  Menucontainer may not need access to input.
            loadingText.DisplayedString = "Initializing Input...";
            loadingText.Origin = new Vector2f(loadingText.GetGlobalBounds().Width / 2, loadingText.GetGlobalBounds().Height / 2);
            window.Clear(Color.Black);
            window.Draw(loadingTitle);
            window.Draw(loadingText);
            window.Display();
            input = new InputManager(window);

            //Initializing Rendering systems
            loadingText.DisplayedString = "Initializing Rendering...";
            loadingText.Origin = new Vector2f(loadingText.GetGlobalBounds().Width / 2, loadingText.GetGlobalBounds().Height / 2);
            window.Clear(Color.Black);
            window.Draw(loadingTitle);
            window.Draw(loadingText);
            window.Display();
            menuContainer = new MenuContainer(input);
            camera = new Camera();
            camera.SubscribeToInput(input);
            renderer = new Renderer(window, menuContainer, textureAtlases.GetTexture("guiTilesheet", out _));
            menuFactory = new MenuFactory(camera, menuContainer, renderer, this, textureAtlases, fontContainer);
            window.Resized += camera.HandleResize;
            window.Resized += renderer.HandleResize;
            window.Resized += menuContainer.RepositionMenus;
            input.menuFactory = menuFactory;

            //Loading prototypes
            loadingText.DisplayedString = "Initializing Collections...";
            loadingText.Origin = new Vector2f(loadingText.GetGlobalBounds().Width / 2, loadingText.GetGlobalBounds().Height / 2);
            window.Clear(Color.Black);
            window.Draw(loadingTitle);
            window.Draw(loadingText);
            window.Display();
            EntityUpdateSystem.UpdateProperties[] updateOrder = new EntityUpdateSystem.UpdateProperties[]
            {
                new EntityUpdateSystem.UpdateProperties(typeof(Player), 1),
                new EntityUpdateSystem.UpdateProperties(typeof(Machine), 1),
                new EntityUpdateSystem.UpdateProperties(typeof(Tree), 600)

            };
            entityUpdateSystem = new EntityUpdateSystem(updateOrder);
            tileCollection = new TileCollection(textureAtlases);
            entityCollection = new EntityCollection(textureAtlases, entityUpdateSystem);
            entityCollection.LoadPrototypes();
            input.entityCollection = entityCollection;
            itemCollection = new ItemCollection(textureAtlases);
            input.itemCollection = itemCollection;

            
        }
        public void StartMenu()
        {
            StaticSoundManager.PlayMusic();
            input.menuFactory.CreateMainMenu();
            while (window.IsOpen && gameState == GameState.mainMenu)
            {
                window.Clear();
                window.DispatchEvents();
                input.Update(gameState);
                renderer.RenderGUI(window, camera);
                window.Display();
            }
            StaticSoundManager.StopMusic();
        }

        public void InitializeGame()
        {
            //Pop all menus
            menuContainer.RemoveAllMenus();
            //Game systems intialization
            surfaceContainer = new SurfaceContainer(tileCollection, surfaceGenerator, 6000, 3000, 255) ;
            renderer.InitializeForGame(tileCollection);
            #region test entities

            player = entityCollection.InstantiatePrototype("player", new Vector2(1024, 1024), surfaceContainer);
            IInputSubscriber playerSubscriber = player as IInputSubscriber;
            if (playerSubscriber != null)
            {
                playerSubscriber.SubscribeToInput(input);
            }
            #endregion
            //Attaching the camera to something!
            camera.SetFocusedEntity(player, menuFactory);
            this.SubscribeToInput(input);
            renderer.SubscribeToInput(input);
        }

        public void FinalizeGame()
        {
            //Pop all menus
            menuContainer.RemoveAllMenus();
            player = null;
            camera.DetachEntity();
            camera.viewedSurface = null;
            surfaceContainer = null;
            input.ClearGameSubscribers();
            renderer.DetachGameWorld();
        }

        public void RunGame()
        {
            InitializeGame();
            clock = new Clock();
            fpsQueue = new Queue<float>(10);
            frame = 0;
            Pathing pathTest = new Pathing(tileCollection);
            Vector2f target = new Vector2f(2048 * 3, 2048 * 3);
            RectangleShape targetBox = new RectangleShape(new Vector2f(32, 32));
            targetBox.Position = target;
            targetBox.FillColor = Color.Green;
            PathNode path = pathTest.GetPath(surfaceContainer, new Vector2(2048, 2048), new Vector2(2048 * 3, 2048 * 3), 500, Base.CollisionLayer.TerrainSolid, Base.CollisionLayer.TerrainSolid | Base.CollisionLayer.Terrain, 2.0f);
            for(int i = 0; i < 30; i ++)
            {
                fpsQueue.Enqueue(60.0f);
            }
            while (window.IsOpen && (gameState == GameState.inGame || gameState == GameState.paused))
            {
                //Check fps
                fps = 1.0f / clock.ElapsedTime.AsSeconds();
                fpsQueue.Enqueue(fps);
                fpsQueue.Dequeue();
                clock.Restart();
                //prepare for drawing and dispatch window events
                window.Clear();
                window.DispatchEvents();
                //update input
                input.Update(gameState);

                //Do not update game world if game is paused
                if (gameState != GameState.paused)
                {
                    //update surface
                    surfaceContainer.Update();
                    //update entities
                    entityUpdateSystem.UpdateEntities(entityCollection, itemCollection);
                    entityUpdateSystem.AddNewEntities();
                    entityUpdateSystem.DestroyEntities();
                }
                //update camera
                camera.Update();
                //drawing game world (terrain, entities)
                if (camera.viewedSurface != null)
                {
                    renderer.RenderWorld(window, camera, camera.viewedSurface);
                    if (camera.focusedEntity is Player) {
                        renderer.RenderSelectionBox(window, input, camera, (Player)camera.focusedEntity, textureAtlases);
                    }
                    window.SetView(camera.GetGameView());
                    //debug pathtesting
                    if (path != null)
                    {
                        pathTest.DrawPath(window, path);
                    }
                    window.Draw(targetBox);
                    //end debug pathtesting
                }
                //drawing menus (main menu, pause, ingame, etc)
                renderer.RenderGUI(window, camera);
                //Draw the player's held item (very ugly to have to put it here but couldnt think of anything better)
                if (camera.focusedEntity is Player) {
                    renderer.RenderHeldItem(window, camera, ((Player)camera.focusedEntity).heldItem, input);
                }
                //Cull far away vertexarrays from renderer cache
                renderer.CheckCullVertexCache(camera, surfaceContainer);
                window.Display();
                frame++;
            }
            FinalizeGame();
        }
        public void SwitchGameState(string tag)
        {
            if (tag == "inGame")
            {
                gameState = GameState.inGame;
            }
            else if(tag == "mainMenu")
            {
                gameState = GameState.mainMenu;
            }
            else if(tag == "paused")
            {
                gameState = GameState.paused;
            }
            else if(tag == "exit")
            {
                window.Close();
            }
        }

        public void CreateMapGenMenu(string tag)
        {
            GeneratorEntityAffinity treeCollection = new GeneratorEntityAffinity(new string[] { "pineTree1" }, new float[] { 0.5f }, new float[] { 0.5f }, new float[] { 0.5f }, new float[] { 0.6f }, new float[] { 0.6f }, new float[] { 0.6f });
            treeCollection.SetDensity("", 96);
            List<GeneratorEntityAffinity> list = new List<GeneratorEntityAffinity>();
            list.Add(treeCollection);
            surfaceGenerator = new SurfaceGenerator(tileCollection, entityCollection, list);
            menuFactory.CreateMapGenMenu(surfaceGenerator);
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
            if (gameState == GameState.inGame && input.GetKeyPressed(InputBindings.showPauseMenu, true))
            {
                menuFactory.CreatePauseMenu();
            }
        }

        public string GetFPS()
        {
            float fps = fpsQueue.Average();
            return fps.ToString("0.0");
        }
    }
}
