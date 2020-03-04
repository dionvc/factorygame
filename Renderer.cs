using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFML.Graphics;
using SFML.System;

namespace EngineeringCorpsCS
{
    class Renderer
    {
        Dictionary<int, VertexArray[]> terrainVertexArrays; //can be cached
        VertexArray boundingBoxArray; //must be reconstructed every frame
        Texture[] terrainTilesheets;
        RenderStates[] terrainRenderStates;
        TileCollection tileCollection;
        SurfaceContainer surface;
        bool drawBoundingBoxes = true;

        public Renderer(TileCollection tileCollection, SurfaceContainer chunkManager)
        {
            terrainVertexArrays = new Dictionary<int, VertexArray[]>(); //terrain cache //TODO: clear periodically
            boundingBoxArray = new VertexArray(PrimitiveType.Lines);
            terrainTilesheets = tileCollection.GetTerrainTilesheets();
            terrainRenderStates = new RenderStates[terrainTilesheets.Length];
            this.surface = chunkManager;
            this.tileCollection = tileCollection;
            for (int i = 0; i < terrainTilesheets.Length; i++)
            {
                terrainRenderStates[i] = new RenderStates(terrainTilesheets[i]);
            }
        }
        public void RenderWorld(RenderWindow window, Camera camera)
        {
            Vector2f origin = window.MapPixelToCoords(new Vector2i(0, 0), camera.GetView());
            Vector2f extent = window.MapPixelToCoords(new Vector2i((int)window.Size.X, (int)window.Size.Y), camera.GetView());
            int[] begPos = SurfaceContainer.WorldToChunkCoords(origin.X, origin.Y);
            int[] endPos = SurfaceContainer.WorldToChunkCoords(extent.X, extent.Y);
            for (int i = begPos[0]; i <= endPos[0]; i++)
            {
                for (int j = begPos[1]; j <= endPos[1]; j++)
                {
                    int key = (i) * Props.worldSize + j;
                    if (terrainVertexArrays.TryGetValue(key, out _) == false)
                    {
                        terrainVertexArrays.Add(key, tileCollection.GenerateTerrainVertexArray(surface, new int[] { i, j }));
                    }
                    VertexArray[] vArr;
                    if (terrainVertexArrays.TryGetValue(key, out vArr))
                    {
                        for (int k = 1; k < vArr.Length; k++)
                        {
                            window.Draw(vArr[k], terrainRenderStates[k]);

                        }
                    }
                }
            }
            if (drawBoundingBoxes == true)
            {
                for (int i = begPos[0]; i <= endPos[0]; i++)
                {
                    for (int j = begPos[1]; j <= endPos[1]; j++)
                    {
                        List<Entity> boxList = surface.GetChunk(i, j).entityList;
                        for (int k = 0; k < boxList.Count; k++)
                        {
                            if (boxList[k].position.x > origin.X && boxList[k].position.x < extent.X
                                && boxList[k].position.y > origin.Y && boxList[k].position.y < extent.Y)
                            {
                                float[] points = boxList[k].collisionBox.GetPoints();
                                Vector2 position = boxList[k].position;
                                for (int l = 0; l < points.Length; l += 2)
                                {
                                    boundingBoxArray.Append(new Vertex(new Vector2f(points[l] + position.x, points[l + 1] + position.y), Color.Red));
                                    boundingBoxArray.Append(new Vertex(new Vector2f(points[(l + 2) % 8] + position.x, points[(l + 3) % 8] + position.y), Color.Red));
                                }
                            }
                        }
                    }
                }
                window.Draw(boundingBoxArray);
                boundingBoxArray.Clear();
            }
        }

        /// <summary>
        /// Collects visible chunks into list
        /// </summary>
        /// <param name="camX"></param>
        /// <param name="camY"></param>
        /// <param name="camW"></param>
        /// <param name="camH"></param>
        /// <param name="chunkM"></param>
        void GetVisibleChunks(float camX, float camY, float camW, float camH, SurfaceContainer chunkM)
        {
            int[] top = SurfaceContainer.WorldToChunkCoords(camX, camY);
            int[] bot = SurfaceContainer.WorldToChunkCoords(camX + camW, camY + camH);
            List<Chunk> cList = new List<Chunk>();
            for(int i = top[0]; i < bot[0]; i++)
            {
                for(int j = top[1]; j < top[1]; j++)
                {
                    cList.Add(chunkM.GetChunk(i, j));
                }
            }
        }

        //Construct vertex arrays for each visible chunk and each visible tile (cull tiles)
        //Store these vertex arrays

        //Check every frame the camera location

        //Update visible chunks if camera location changes enough

        //Get entities every frame from visible chunks and cull, then sort
    }
}
