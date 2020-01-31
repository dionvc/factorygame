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
            RenderWindow window = new RenderWindow(new VideoMode(1280,720), "test");
            string g = "Graphics/";
            Texture rocktile = new Texture(g+"rocksquare.png");
            Texture snowtile = new Texture(g+"snowsquare.png");
            Texture sandtile = new Texture(g+"sandsquare.png");
            Texture watertile = new Texture(g+"watersquare.png");
            Sprite rock = new Sprite(rocktile);
            Sprite snow = new Sprite(snowtile);
            Sprite sand = new Sprite(sandtile);
            Sprite water = new Sprite(watertile);
            window.SetActive();
            ChunkManager chunkManager = new ChunkManager();
            Camera camera = new Camera();
            window.SetFramerateLimit(60);
            window.Closed += (s, a) => window.Close();
            VertexArray waterquad = new VertexArray(PrimitiveType.Quads, 4);
            RenderStates state = new RenderStates(watertile);
            bool drawBoundingBoxes = true;
            while (window.IsOpen)
            {
                
                window.Clear();
                window.DispatchEvents();
                camera.Update();
                window.SetView(camera.GetView());

                //drawing terrain
                int[] pos = ChunkManager.WorldToChunkCoords(camera.GetView().Center.X, camera.GetView().Center.Y);
                Chunk chunk = chunkManager.GetChunk(pos[0], pos[1]);
                int cX = pos[0] * Props.chunkSize;
                int cY = pos[1] * Props.chunkSize;
                for (int i = 0; i < Props.chunkSize; i++)
                {
                    for (int j = 0; j < Props.chunkSize; j++)
                    {
                        if (chunk.GetTile(i, j) == 0)
                        {
                            waterquad.Append(new Vertex(new Vector2f((cX+i) * 128.0f, (cY+j) * 128.0f),
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
                window.Draw(waterquad, state);
                //finish drawing terrain

                //draw entities
                if (drawBoundingBoxes == true)
                {
                    float[] points = camera.focusedEntity.collisionBox.GetPoints();
                    Vector2 position = camera.focusedEntity.position;
                    VertexArray boundingBoxArray = new VertexArray(PrimitiveType.LineStrip);
                    for (int i = 0; i < points.Length; i += 2)
                    {
                        boundingBoxArray.Append(new Vertex(new Vector2f(points[i] + position.x, points[i + 1] + position.y)));
                        boundingBoxArray.Append(new Vertex(new Vector2f(points[(i + 2) % 8] + position.x, points[(i + 3) % 8] + position.y)));
                    }
                    window.Draw(boundingBoxArray);
                }
                //finish drawing entities
                window.Display();
                waterquad.Clear();
            }
        }
    }
}
