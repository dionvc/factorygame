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
            //Vital stuff (could be migrated to some init function)
            RenderWindow window = new RenderWindow(new VideoMode(1280, 720), "Engineering Corps");
            window.SetFramerateLimit(60);
            window.Closed += (s, a) => window.Close();
            window.SetActive();
            TextureManager textureManager = new TextureManager();
            textureManager.LoadTextures();
            InputManager input = new InputManager(window);
            //Camera is the entry point for the game, first it subscribes to input, then an entity is added which makes the entity subscribe to input
            ChunkManager chunkManager = new ChunkManager();
            Camera camera = new Camera();
            
            //Terrain stuff
            TileManager tileManager = new TileManager(textureManager);
            Texture[] terrainTilesheets = tileManager.GetTerrainTilesheets();
            RenderStates[] terrainRenderStates = new RenderStates[terrainTilesheets.Length];
            for(int i = 0; i < terrainTilesheets.Length; i++)
            {
                terrainRenderStates[i] = new RenderStates(terrainTilesheets[i]);
            }
            Dictionary<int, VertexArray[]> terrainVertexArrays = new Dictionary<int, VertexArray[]>(); //terrain cache //TODO: clear periodically

            //Debug flags and stuff
            Font debugFont = new Font("SairaRegular.ttf");
            Text biomeText = new Text("default", debugFont, 32);
            Text fpsText = new Text("default", debugFont, 32);
            Clock clock = new Clock();
            float lastTime = 0.0f;
            bool drawBoundingBoxes = true;
            bool drawDebugText = true;

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

                
                //drawing terrain
                Vector2f origin = window.MapPixelToCoords(new Vector2i(0,0), camera.GetView());
                Vector2f extent = window.MapPixelToCoords(new Vector2i(1280, 720), camera.GetView());
                int[] begPos = ChunkManager.WorldToChunkCoords(origin.X, origin.Y);
                int[] endPos = ChunkManager.WorldToChunkCoords(extent.X, extent.Y);
                for(int i = begPos[0]; i <= endPos[0]; i++)
                {
                    for(int j = begPos[1]; j <= endPos[1]; j++)
                    {
                        int key = (i) * Props.worldSize + j;
                        if (terrainVertexArrays.TryGetValue(key, out _) == false)
                        {
                            terrainVertexArrays.Add(key, tileManager.GenerateTerrainVertexArray(chunkManager, new int[] { i, j }));
                        }
                        VertexArray[] vArr;
                        if (terrainVertexArrays.TryGetValue(key, out vArr)) {
                            for (int k = 0; k < vArr.Length; k++)
                            {
                                window.Draw(vArr[k], terrainRenderStates[k]);

                            }
                        }
                    }
                }
                
                //finish drawing terrain

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
                    float currentTime = clock.Restart().AsSeconds();
                    float fps = 1.0f / currentTime;
                    lastTime = currentTime;
                    biomeText.DisplayedString = "Biome: " + tileManager.GetTileName(chunkManager.GetTileFromWorld(camera.focusedEntity.position.x, camera.focusedEntity.position.y));
                    biomeText.Position = origin;
                    fpsText.DisplayedString = "FPS/TPS: " + fps;
                    fpsText.Position = new Vector2f(origin.X, origin.Y + 32);
                    window.Draw(biomeText);
                    window.Draw(fpsText);
                }
                
                //finish drawing entities
                window.Display();
                //waterquad.Clear();
                input.FlushInput(); //Flush input cache
            }
        }
    }
}
