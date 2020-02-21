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
            TextureManager textureManager = new TextureManager();
            textureManager.LoadTextures();
            ChunkManager chunkManager = new ChunkManager();
            TileCollection tileCollection = new TileCollection(textureManager);
            InputManager input = new InputManager(window);
            Renderer gameRenderer = new Renderer(tileCollection, chunkManager);
            BoundingBox.chunkManager = chunkManager;
            BoundingBox.tileCollection = tileCollection;

            //Debug flags and stuff
            Font debugFont = new Font("SairaRegular.ttf");
            Text biomeText = new Text("default", debugFont, 32);
            Text fpsText = new Text("default", debugFont, 32);
            Text coordinates = new Text("default", debugFont, 32);
            Clock clock = new Clock();
            float lastTime = 0.0f;
            bool drawBoundingBoxes = true;
            bool drawDebugText = true;
            RenderTexture GUI = new RenderTexture(window.Size.X, window.Size.Y);

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
                players.Add(new Player());
                players[i].position = new Vector2(random.Next(0, 512), random.Next(0, 512));
                players[i].collisionBox.SetRotation(random.Next(0, 360));
            }
            #endregion

            //TODO: compartmentalize
            players.Add(camera.focusedEntity);
            
            
            while (window.IsOpen)
            {
                window.Clear();
                window.DispatchEvents();
                camera.Update(); //TODO: compartmentalize into input manager
                window.SetView(camera.GetView());


                //drawing game world (terrain, entities)
                gameRenderer.RenderWorld(window, camera);

                //draw GUI
                //gameRenderer.RenderGUI();

                //Animation Draw Test

                //wavesTest.Update();
                //window.Draw(wavesTest.GetAnimationFrame());
                multiTest.Update();
                window.Draw(multiTest.GetAnimationFrame());

                //End Animation Draw Test

                //Draw bounding boxes
                if (drawBoundingBoxes == true)
                {
                    VertexArray boundingBoxArray = new VertexArray(PrimitiveType.Lines);
                    for (int j = 0; j < players.Count; j++)
                    {
                        float[] points = players[j].collisionBox.GetPoints();
                        Vector2 position = players[j].position;
                        if(!players[j].Equals(camera.focusedEntity) && BoundingBox.CheckCollision(players[j].collisionBox, camera.focusedEntity.collisionBox, position, camera.focusedEntity.position))
                        {
                            Console.WriteLine("Collision Detected");
                        }
                        for (int i = 0; i < points.Length; i += 2)
                        {
                            boundingBoxArray.Append(new Vertex(new Vector2f(points[i] + position.x, points[i + 1] + position.y), Color.Red));
                            boundingBoxArray.Append(new Vertex(new Vector2f(points[(i + 2) % 8] + position.x, points[(i + 3) % 8] + position.y), Color.Red));
                        }
                    }
                    window.Draw(boundingBoxArray);
                }
                if(drawDebugText == true)
                {
                    GUI.Clear(Color.Transparent);
                    
                    float currentTime = clock.Restart().AsSeconds();
                    float fps = 1.0f / currentTime;
                    lastTime = currentTime;
                    float worldX = camera.GetView().Center.X;
                    float worldY = camera.GetView().Center.Y;
                    int[] cXY = ChunkManager.WorldToChunkCoords(worldX, worldY);
                    biomeText.DisplayedString = "Biome: " + tileCollection.GetTileName(chunkManager.GetTileFromWorld(worldX, worldY));
                    biomeText.Position = new Vector2f(0, 0);
                    fpsText.DisplayedString = "FPS/TPS: " + fps;
                    fpsText.Position = new Vector2f(0, 32);
                    coordinates.DisplayedString = "World Coordinates: " + worldX + ", " + worldY + "\nChunk Coordinates:" + cXY[0] + ", " + cXY[1] + "\nTile Coordinates:" + (int)(worldX / Props.tileSize) % Props.chunkSize + ", " + (int)(worldY / Props.tileSize) % Props.chunkSize;
                    coordinates.Position = new Vector2f(0, 64);
                    GUI.Draw(biomeText);
                    GUI.Draw(fpsText);
                    GUI.Draw(coordinates);
                    GUI.Display();
                }
                Sprite GUISprite = new Sprite(GUI.Texture);
                GUISprite.Position = new Vector2f(-640,-360);
                View GUIView = new View(new Vector2f(0, 0), new Vector2f(1280, 720));
                window.SetView(GUIView);
                window.Draw(GUISprite);
                //finish drawing entities
                window.Display();
                //waterquad.Clear();
                input.FlushInput(); //Flush input cache
            }
        }
    }
}
