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
            //Window initialization (all of this stays here I think)
            RenderWindow window = new RenderWindow(new VideoMode(1280, 720), "Engineering Corps");
            window.SetFramerateLimit(60);
            window.Closed += (s, a) => window.Close();
            window.SetActive();
            Image icon = new Image("Graphics/EngineeringCorpsIcon.png");
            window.SetIcon(icon.Size.X, icon.Size.Y, icon.Pixels);

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

            //TODO: contain following within renderer?
            bool drawDebugText = true;
            RenderTexture GUI = new RenderTexture(window.Size.X, window.Size.Y);
            //TODO: contain guiview in camera?
            View GUIView = new View(new Vector2f(640, 360), new Vector2f(1280, 720));

            #region test entities
            List<Player> players = new List<Player>();
            Random random = new Random();
            for (int i = 0; i < 16; i++)
            {
                players.Add(new Player(new Vector2(1024 + 48 * i, 1024 + 48 * i), surfaceContainer, textureManager));
            }
            foreach(Player p in players)
            {
                //p.SubscribeToInput(input);
            }
            players[15].SubscribeToInput(input);
            #endregion
            //Menu testing
            MenuPanel test = new MenuPanel(new Vector2f(0,0), new Vector2f(150, 150), new bool[] { true, true });
            MenuText textTest = new MenuText(new Vector2f(0, 0), new Vector2f(140, 150), new bool[] { true, true }, debugFont, "This is test text intentionally long to test line splitting functionalityaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa", 32);
            MenuPanel test2 = new MenuPanel(new Vector2f(300, 300), new Vector2f(200, 200), new bool[] { true, true });
            MenuButton menuButton = new MenuButton(new Vector2f(300,300), gameRenderer.ToggleBoundingBoxRendering, new Vector2f(150, 150));
            MenuText buttonTest = new MenuText(new Vector2f(0, 0), new Vector2f(140, 150), new bool[] { false, false }, debugFont, "Toggle Bounding boxes", 24);
            menuButton.SubscribeToInput(input);
            menuButton.AttachComponent(buttonTest);
            test2.AttachComponent(textTest);
            test.AttachComponent(test2);
            test.AttachComponent(textTest);
            //End menu testing

            //Attaching the camera to something!
            camera.focusedEntity = players[15];
            
            
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
                //TODO: compartmentalize into renderer
                
                
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
                    menuButton.Draw(GUI, new Vector2f(0, 0));
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
