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
            //test of logic for tile transition assignment
            byte t = 66;
            byte z = 96;
            Console.WriteLine(((t & z)==0) ? 1 : 0 * 1);
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

            //Debug testing stuff
            Sprite rock = new Sprite(textureManager.GetTexture("asdf"));
            rock.Scale = new Vector2f(0.5f, 0.5f);
            Sprite snow = new Sprite(textureManager.GetTexture("snowsquare"));
            Sprite sand = new Sprite(textureManager.GetTexture("sandsquare"));
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
            players.Add(camera.focusedEntity);
            VertexArray waterquad = new VertexArray(PrimitiveType.Quads, 4);
            RenderStates state = new RenderStates(textureManager.GetTexture("watersquare"));
            //End Debug testing stuff
            bool drawBoundingBoxes = true;
            while (window.IsOpen)
            {
                window.Clear();
                window.DispatchEvents();
                camera.Update();
                window.SetView(camera.GetView());

                //drawing terrain
                int[] pos = ChunkManager.WorldToChunkCoords(camera.GetView().Center.X, camera.GetView().Center.Y);
                for (int c = 0; c < 9; c++)
                {
                    Chunk chunk = chunkManager.GetChunk(pos[0] + (c) / 3 - 1, pos[1] + (c) % 3 - 1);
                    if(chunk != null) 
                    {
                        int cX = (pos[0] + (c) / 3 - 1) * Props.chunkSize;
                        int cY = (pos[1] + (c) % 3 - 1) * Props.chunkSize;
                        for (int i = 0; i < Props.chunkSize; i++)
                        {
                            for (int j = 0; j < Props.chunkSize; j++)
                            {
                                if (chunk.GetTile(i, j) == 0)
                                {
                                    waterquad.Append(new Vertex(new Vector2f((cX + i) * 128.0f, (cY + j) * 128.0f),
                                        new Vector2f(0.0f, 0.0f)));
                                    waterquad.Append(new Vertex(new Vector2f((cX + i + 1) * 128.0f, (cY + j) * 128.0f),
                                        new Vector2f(128.0f, 0.0f)));
                                    waterquad.Append(new Vertex(new Vector2f((cX + i + 1) * 128.0f, (cY + j + 1) * 128.0f),
                                        new Vector2f(128.0f, 128.0f)));
                                    waterquad.Append(new Vertex(new Vector2f((cX + i) * 128.0f, (cY + j + 1) * 128.0f),
                                        new Vector2f(0.0f, 128.0f)));
                                }
                                if (chunk.GetTile(i, j) == 1)
                                {
                                    sand.Position = new Vector2f((cX + i) * 128, (cY + j) * 128);
                                    window.Draw(sand);
                                }
                                if (chunk.GetTile(i, j) == 2)
                                {
                                    rock.Position = new Vector2f((cX + i) * 128, (cY + j) * 128);
                                    window.Draw(rock);
                                }
                                if (chunk.GetTile(i, j) == 3)
                                {
                                    snow.Position = new Vector2f((cX + i) * 128, (cY + j) * 128);
                                    window.Draw(snow);
                                }

                            }
                        }
                    }
                }
                window.Draw(waterquad, state);
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
                //finish drawing entities
                window.Display();
                waterquad.Clear();
                input.FlushInput(); //Flush input cache
            }
        }
    }
}
