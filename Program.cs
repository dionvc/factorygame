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
    class Program
    {
        static void Main(string[] args)
        {
            //Window initialization
            RenderWindow window = new RenderWindow(new VideoMode(1280, 720), "Engineering Corps");
            window.SetFramerateLimit(60);
            window.Closed += (s, a) => window.Close();
            window.SetActive();
            Image icon = new Image("Graphics/EngineeringCorpsIcon.png");
            window.SetIcon(icon.Size.X, icon.Size.Y, icon.Pixels);

            //Camera is the entry point for the game, first it subscribes to input, then an entity is added which makes the entity subscribe to input



            //Repository, Managers, Resource collections creation
            TextureContainer textureManager = new TextureContainer();
            textureManager.LoadTextures();
            FontContainer fontContainer = new FontContainer();
            fontContainer.LoadFonts();
            
            TileCollection tileCollection = new TileCollection(textureManager);
            SurfaceContainer surfaceContainer = new SurfaceContainer(tileCollection);
            InputManager input = new InputManager(window);
            Camera camera = new Camera();
            camera.SubscribeToInput(input);
            Renderer gameRenderer = new Renderer(tileCollection, surfaceContainer);
            gameRenderer.SubscribeToInput(input);

            //Debug flags and stuff
            Font debugFont = new Font("SairaRegular.ttf");
            Text biomeText = new Text("default", debugFont, 32);
            Text fpsText = new Text("default", debugFont, 32);
            Text coordinates = new Text("default", debugFont, 32);
            coordinates.LineSpacing = 0.5f;
            Clock clock = new Clock();
            bool drawDebugText = true;
            //TODO: contain within renderer?
            RenderTexture GUI = new RenderTexture(window.Size.X, window.Size.Y);
            //TODO: contain guiview in camera?
            View GUIView = new View(new Vector2f(640, 360), new Vector2f(1280, 720));

            #region randomstuff
            //Texture[] waves = new Texture[1];
            //waves[0] = textureManager.GetTexture("WavesAlpha");
            //RotatedAnimation wavesTest = new RotatedAnimation(waves, new Vector2i(320, 320), new Vector2f(0, 0), new Vector2f(2.0f, 2.0f), 1, 10, "fb", 12.0f);
            //Texture[] multi = new Texture[] { textureManager.GetTexture("1"), textureManager.GetTexture("2"), textureManager.GetTexture("3"), textureManager.GetTexture("4") };
            //RotatedAnimation multiTest = new RotatedAnimation(multi, new Vector2i(256, 256), new Vector2f(0, 0), new Vector2f(1.0f, 1.0f), 1, 4, "fb", 30.0f);
            
            List<Player> players = new List<Player>();
            Random random = new Random();
            for (int i = 0; i < 1024; i++)
            {
                players.Add(new Player(new Vector2(48 * i, 48 * i), surfaceContainer, textureManager));
            }
            #endregion
            MenuPanel test = new MenuPanel(new Vector2f(0,0), new Vector2f(150, 150), new bool[] { true, true });
            MenuText textTest = new MenuText(new Vector2f(0, 0), new Vector2f(140, 150), new bool[] { true, true }, debugFont, "This is test text intentionally long to test line splitting functionalityaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa", 32);
            MenuPanel test2 = new MenuPanel(new Vector2f(300, 300), new Vector2f(200, 200), new bool[] { true, true });
            test2.AttachComponent(textTest);
            test.AttachComponent(test2);
            test.AttachComponent(textTest);

            camera.focusedEntity = players[15];
            players[15].SubscribeToInput(input);
            
            while (window.IsOpen)
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
                //set the view to current camera
                window.SetView(camera.GetView());
                //drawing game world (terrain, entities)
                gameRenderer.RenderWorld(window, camera);
                foreach (Player p in players)
                {
                    window.Draw(p.playerTest.GetAnimationFrame());
                }
                
                //set the view to GUI
                //window.SetView(GUIView);
                //drawing menus (main menu, pause, ingame, etc)
                //gameRenderer.RenderGUI();

                //The following is debugging code to be removed
                //==============================START menu debugging code
                window.SetView(GUIView);
                if (drawDebugText == true)
                {
                    GUI.Clear(Color.Transparent);
                    Vector2f mouse = window.MapPixelToCoords(new Vector2i(input.mouseX - input.mouseXdiff,input.mouseY - input.mouseYdiff), GUIView);
                    if(input.mouseButton[Mouse.Button.Left] && BoundingBox.CheckPointMenuCollision(mouse.X, mouse.Y, test.collisionBox, test.position))
                    {
                        test.Translate(new Vector2f(input.mouseXdiff, input.mouseYdiff));
                    }
                    test.Draw(GUI, new Vector2f(0,0));
                    float fps = 1.0f / clock.ElapsedTime.AsSeconds();
                    clock.Restart();
                    float worldX = camera.GetView().Center.X;
                    float worldY = camera.GetView().Center.Y;
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


                window.Display();
            }
        }
    }
}
