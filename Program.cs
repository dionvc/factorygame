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
            Texture rocktile = new Texture("rocksquare.png");
            Texture snowtile = new Texture("snowsquare.png");
            Texture sandtile = new Texture("sandsquare.png");
            Texture watertile = new Texture("watersquare.png");
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
            while (window.IsOpen)
            {
                
                window.Clear();
                window.DispatchEvents();
                camera.Update();
                window.SetView(camera.GetView());
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
                float[] points = camera.focusedEntity.collisionBox.GetPoints();
                VertexArray boundingBoxArray = new VertexArray(PrimitiveType.LineStrip);
                for (int i = 0; i < points.Length; i += 2)
                {
                    boundingBoxArray.Append(new Vertex(new Vector2f(points[i], points[i + 1])));
                    boundingBoxArray.Append(new Vertex(new Vector2f(points[(i+2)%8], points[(i+3)%8])));
                }
                window.Draw(boundingBoxArray);
                window.Display();
                waterquad.Clear();
            }
        }

        private static void Window_Closed(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }
    }
}
