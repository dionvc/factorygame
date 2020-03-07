using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFML.Graphics;
using SFML.System;

namespace EngineeringCorpsCS
{
    class Renderer: IInputSubscriber
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
                        Chunk chunk = surface.GetChunk(i, j);
                        for (int k = 0; k < Props.chunkSize; k++)
                        {
                            for(int l = 0; l < Props.chunkSize; l ++)
                            {
                                Tile tile = tileCollection.GetTerrainTile(chunk.GetTile(k, l));
                                if((tile.collisionMask & Base.CollisionLayer.TerrainSolid) != 0)
                                {
                                    float[] points = surface.tileBox.GetPoints();
                                    Vector2 position = SurfaceContainer.WorldToTileVector(i * Props.worldSize + j, k * Props.chunkSize + l);
                                    for (int x = 0; x < points.Length; x += 2)
                                    {
                                        boundingBoxArray.Append(new Vertex(new Vector2f(points[x] + position.x, points[x + 1] + position.y), Color.Blue));
                                        boundingBoxArray.Append(new Vertex(new Vector2f(points[(x + 2) % 8] + position.x, points[(x + 3) % 8] + position.y), Color.Blue));
                                    }
                                }
                            }
                        }
                        List<Entity> boxList = chunk.entityList;
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
        //Construct vertex arrays for each visible chunk and each visible tile (cull tiles)
        //Store these vertex arrays

        //Check every frame the camera location

        //Update visible chunks if camera location changes enough

        //Get entities every frame from visible chunks and cull, then sort

        public void SubscribeToInput(InputManager input)
        {
            input.AddInputSubscriber(this, false);
        }
        public void UnsubscribeToInput(InputManager input)
        {
            input.RemoveInputSubscriber(this, false);
        }
        public void HandleInput(InputManager input)
        {
            if(input.keyPressed[InputBindings.showBoundingBoxes])
            {
                drawBoundingBoxes = !drawBoundingBoxes;
            }
        }
    }
}
