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

            //Camera is the entry point for the game, first it subscribes to input, then an entity is added which makes the entity subscribe to input
            
            Camera camera = new Camera();

            //Repository, Managers, Resource collections creation
            TextureContainer textureManager = new TextureContainer();
            textureManager.LoadTextures();
            SurfaceContainer chunkManager = new SurfaceContainer();
            TileCollection tileCollection = new TileCollection(textureManager);
            InputManager input = new InputManager(window);
            Renderer gameRenderer = new Renderer(tileCollection, chunkManager);

            //Debug flags and stuff
            Font debugFont = new Font("SairaRegular.ttf");
            Text biomeText = new Text("default", debugFont, 32);
            Text fpsText = new Text("default", debugFont, 32);
            Text coordinates = new Text("default", debugFont, 32);
            coordinates.LineSpacing = 0.5f;
            Clock clock = new Clock();
            bool drawBoundingBoxes = true;
            bool drawDebugText = true;
            RenderTexture GUI = new RenderTexture(window.Size.X, window.Size.Y);
            View GUIView = new View(new Vector2f(640, 360), new Vector2f(1280, 720));

            #region randomstuff
            //Texture[] waves = new Texture[1];
            //waves[0] = textureManager.GetTexture("WavesAlpha");
            //RotatedAnimation wavesTest = new RotatedAnimation(waves, new Vector2i(320, 320), new Vector2f(0, 0), new Vector2f(2.0f, 2.0f), 1, 10, "fb", 12.0f);
            Texture[] multi = new Texture[] { textureManager.GetTexture("1"), textureManager.GetTexture("2"), textureManager.GetTexture("3"), textureManager.GetTexture("4") };
            RotatedAnimation multiTest = new RotatedAnimation(multi, new Vector2i(256, 256), new Vector2f(0, 0), new Vector2f(1.0f, 1.0f), 1, 4, "fb", 30.0f);
            List<Player> players = new List<Player>();
            Random random = new Random();
            for (int i = 0; i < 10; i++)
            {
                for (int j = 0; j < 10; j++)
                {
                    players.Add(new Player());
                    players[i * 10 + j].position = new Vector2(1024 + 128 * i, 1024 + 128 * j);
                    players[i * 10 + j].collisionBox.SetRotation(0);//random.Next(0, 360));
                }
            }
            #endregion
            MenuPanel test = new MenuPanel(new Vector2f(0,0), new Vector2f(150, 150), new bool[] { true, true });
            MenuText textTest = new MenuText(new Vector2f(0, 0), new Vector2f(140, 150), new bool[] { true, true }, debugFont, "This is test text intentionally long to test line splitting functionalityaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa", 32);
            MenuPanel test2 = new MenuPanel(new Vector2f(300, 300), new Vector2f(200, 200), new bool[] { true, true });
            test2.AttachComponent(textTest);
            test.AttachComponent(test2);
            test.AttachComponent(textTest);
            //TODO: compartmentalize
            players.Add(camera.focusedEntity);
            
            
            while (window.IsOpen)
            {
                window.Clear();
                window.DispatchEvents();
                camera.Update(); //TODO: compartmentalize into input manager
                window.SetView(camera.GetView());
                input.Update();


                //drawing game world (terrain, entities)
                gameRenderer.RenderWorld(window, camera);
                //draw GUI
                //gameRenderer.RenderGUI();

                //The following is debugging code to be removed

                //==============================START animation debugging code
                multiTest.Update();
                window.Draw(multiTest.GetAnimationFrame());

                //==============================END Animation Draw Test

                //==============================START entity debugging code
                Vector2f origin = window.MapPixelToCoords(new Vector2i(0, 0), camera.GetView());
                Vector2f extent = window.MapPixelToCoords(new Vector2i((int)window.Size.X, (int)window.Size.Y), camera.GetView());
                if (drawBoundingBoxes == true)
                {
                    VertexArray boundingBoxArray = new VertexArray(PrimitiveType.Lines);
                    for (int j = 0; j < players.Count; j++)
                    {
                        float[] points = players[j].collisionBox.GetPoints();
                        Vector2 position = players[j].position;
                        Vector2 pushBack;
                        if (!players[j].Equals(camera.focusedEntity) && BoundingBox.CheckCollision(camera.focusedEntity.collisionBox, players[j].collisionBox, camera.focusedEntity.position, new Vector2(camera.moveVector.X, camera.moveVector.Y), position, out pushBack))
                        {
                            camera.moveVector.X += pushBack.x;
                            camera.moveVector.Y += pushBack.y;
                        }
                        if (players[j].position.x > origin.X && players[j].position.x < extent.X
                            && players[j].position.y > origin.Y && players[j].position.y < extent.Y)
                        {
                            for (int i = 0; i < points.Length; i += 2)
                            {
                                boundingBoxArray.Append(new Vertex(new Vector2f(points[i] + position.x, points[i + 1] + position.y), Color.Red));
                                boundingBoxArray.Append(new Vertex(new Vector2f(points[(i + 2) % 8] + position.x, points[(i + 3) % 8] + position.y), Color.Red));
                            }
                        }
                    }
                    window.Draw(boundingBoxArray);
                }
                camera.focusedEntity.position.Add(camera.moveVector.X, camera.moveVector.Y);
                //========================END entity debugging code

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
                    biomeText.DisplayedString = "Biome: " + tileCollection.GetTerrainTileName(chunkManager.GetTileFromWorld(worldX, worldY));
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
                input.FlushInput(); //Flush input cache
            }
        }
    }
}
