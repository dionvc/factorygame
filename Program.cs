using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFML.Graphics;
using SFML.Window;

namespace EngineeringCorpsCS
{
    class Program
    {
        //Render queue?
        //BackRender queue?
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
                Console.WriteLine(pos[0] + ": " + pos[1]);
                for (int i = 0; i < Props.chunkSize; i++)
                {
                    for(int j = 0; j < Props.chunkSize; j++)
                    {
                        if(chunk.GetTile(i,j) == 0)
                        {
                            water.Position = new SFML.System.Vector2f((cX + i) * 128, (cY + j) * 128);
                            window.Draw(water);
                        }
                        if(chunk.GetTile(i,j) == 1)
                        {
                            sand.Position = new SFML.System.Vector2f((cX + i) * 128, (cY + j) * 128);
                            window.Draw(sand);
                        }
                        if (chunk.GetTile(i, j) == 2)
                        {
                            rock.Position = new SFML.System.Vector2f((cX + i) * 128, (cY + j) * 128);
                            window.Draw(rock);
                        }
                        if (chunk.GetTile(i, j) == 3)
                        {
                            snow.Position = new SFML.System.Vector2f((cX + i) * 128, (cY + j) * 128);
                            window.Draw(snow);
                        }

                    }
                }
                window.Display();
            }
        }
    }
}
