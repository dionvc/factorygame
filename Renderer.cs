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
    class Renderer : IInputSubscriber
    {
        //Graphics variables
        VertexArray entityBoundingBoxArray; //must be reconstructed every frame TODO: split terrain bounding box array into cachable set
        Texture[] terrainTilesheets;
        RenderTexture GUI;
        RenderStates[] terrainRenderStates;
        List<Entity> drawList;

        //Collections
        Dictionary<int, VertexArray[]> terrainVertexArrays; //TODO: need to change these for multi-surface support
        Dictionary<int, VertexArray> minimapVertexArrays; //TODO: need to change these for multi-surface support
        Dictionary<int, VertexArray> tileBoundingBoxVertexArray;
        TileCollection tileCollection;
        SurfaceContainer surface;
        MenuContainer menuContainer;

        //Control variables
        bool drawBoundingBoxes = false;
        bool cullMinimap = true;
        int cullCounter = 0;
        int cullRate = 60;

        //TODO: Move to textureContainer
        Texture voidMinimap = new Texture(new Image(Props.chunkSize, Props.chunkSize, Color.Black));

        public Renderer(Window window, MenuContainer menuContainer)
        {
            this.menuContainer = menuContainer;
            GUI = new RenderTexture(window.Size.X, window.Size.Y);
        }
        public void InitializeForGame(SurfaceContainer chunkManager, TileCollection tileCollection)
        {
            this.surface = chunkManager;
            this.tileCollection = tileCollection;
            terrainVertexArrays = new Dictionary<int, VertexArray[]>(); //terrain cache //TODO: clear periodically
            minimapVertexArrays = new Dictionary<int, VertexArray>();
            tileBoundingBoxVertexArray = new Dictionary<int, VertexArray>();
            entityBoundingBoxArray = new VertexArray(PrimitiveType.Lines);
            terrainTilesheets = tileCollection.GetTerrainTilesheets();
            terrainRenderStates = new RenderStates[terrainTilesheets.Length];
            drawList = new List<Entity>();
            for (int i = 0; i < terrainTilesheets.Length; i++)
            {
                terrainRenderStates[i] = new RenderStates(terrainTilesheets[i]);
            }
        }
        public void RenderWorld(RenderWindow window, Camera camera)
        {
            window.SetView(camera.GetGameView()); //Set view
            Vector2f origin = window.MapPixelToCoords(new Vector2i(0, 0), camera.GetGameView());
            Vector2f extent = window.MapPixelToCoords(new Vector2i((int)window.Size.X, (int)window.Size.Y), camera.GetGameView());
            int[] begPos = SurfaceContainer.WorldToChunkCoords(origin.X, origin.Y);
            int[] endPos = SurfaceContainer.WorldToChunkCoords(extent.X, extent.Y);
            #region terrain drawing
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
            #endregion terrain drawing

            #region entity drawing
            for (int i = begPos[0]; i <= endPos[0]; i++)
            {
                for (int j = begPos[1]; j <= endPos[1]; j++)
                {

                    Chunk chunk = surface.GetChunk(i, j);
                    List<Entity> entityList = chunk.entityList;
                    for (int k = 0; k < entityList.Count; k++)
                    {
                        //TODO: More elegant clipping method needed (or even just an approximation method)
                        if (entityList[k].position.x > origin.X && entityList[k].position.x < extent.X
                            && entityList[k].position.y > origin.Y && entityList[k].position.y < extent.Y)
                        {
                            drawList.Add(entityList[k]);
                        }
                    }
                }
                    
            }
            drawList.Sort(delegate (Entity a, Entity b)
            {
                int ydiff = a.position.y.CompareTo(b.position.y);
                if (ydiff != 0)
                {
                    return ydiff;
                }
                else
                {
                    return a.position.x.CompareTo(b.position.x);
                }
            });
            foreach (Entity e in drawList)
            {
                Sprite drawSprite = e.drawArray[0].GetSprite();
                drawSprite.Position = new Vector2f(e.position.x, e.position.y) + e.drawArray[0].drawOffset;
                window.Draw(drawSprite);
            }
            drawList.Clear();
            #endregion entity drawing

            #region bounding box drawing
            if (drawBoundingBoxes == true)
            {
                for (int i = begPos[0]; i <= endPos[0]; i++)
                {
                    for (int j = begPos[1]; j <= endPos[1]; j++)
                    {
                        Chunk chunk = surface.GetChunk(i, j);
                        #region Tile bounding box drawing

                        int chunkIndex = i * Props.worldSize + j;
                        float[] pointsTile = surface.tileBox.GetPoints();
                        VertexArray vA;
                        if (!tileBoundingBoxVertexArray.TryGetValue(chunkIndex, out vA))
                        {
                            vA = tileCollection.GenerateTerrainBoundingBoxArray(chunk, chunkIndex, pointsTile);
                            tileBoundingBoxVertexArray.Add(chunkIndex, vA);
                        }
                        window.Draw(vA);
                        #endregion Tile bounding box drawing

                        #region Entity bounding box drawing
                        List<Entity> entityList = chunk.entityList;
                        for (int k = 0; k < entityList.Count; k++)
                        {
                            float[] pointsEntity = entityList[k].collisionBox.GetPoints();
                            Vector2 position = entityList[k].position;
                            for (int l = 0; l < pointsEntity.Length; l += 2)
                            {
                                entityBoundingBoxArray.Append(new Vertex(new Vector2f(pointsEntity[l] + position.x, pointsEntity[l + 1] + position.y), Color.Red));
                                entityBoundingBoxArray.Append(new Vertex(new Vector2f(pointsEntity[(l + 2) % 8] + position.x, pointsEntity[(l + 3) % 8] + position.y), Color.Red));
                            }
                        }
                        #endregion Entity bounding box drawing
                    }
                }
                window.Draw(entityBoundingBoxArray);
                entityBoundingBoxArray.Clear();
            }
            #endregion bounding box drawing
            cullCounter++;
            if(cullCounter > cullRate)
            {
                CullVertexCache(camera);
                cullCounter = 0;
            }
        }

        /// <summary>
        /// Gets minimap textures for a specified surface.  The position is the centered location. The range is the number of chunks x/y away from center to get.
        /// </summary>
        /// <param name="surface"></param>
        /// <param name="position"></param>
        /// <param name="ranges"></param>
        public void GenerateMinimapTextures(SurfaceContainer surface, Vector2 position, int xRange, int yRange, List<VertexArray> vertexArrays)
        {
            int[] chunkIndices = SurfaceContainer.WorldToChunkCoords(position);
            vertexArrays.Clear();
            for(int i = chunkIndices[0] - xRange ; i <= chunkIndices[0] + xRange; i++)
            {
                for(int j = chunkIndices[1] - yRange; j <= chunkIndices[1] + yRange; j++)
                {
                    Chunk chunk = surface.GetChunk((i * Props.worldSize) + j, false);
                    if (chunk != null)
                    {
                        VertexArray vA;
                        if (!minimapVertexArrays.TryGetValue(i * Props.worldSize + j, out vA))
                        {
                            vA = tileCollection.GenerateTerrainMinimap(chunk, (i * Props.worldSize) + j);
                            minimapVertexArrays.Add(i * Props.worldSize + j, vA);
                        }
                        vertexArrays.Add(vA);
                        //next get entities
                        List<Entity> entityList = chunk.entityList;
                        VertexArray entityArray = new VertexArray(PrimitiveType.Triangles);
                        int oX = i * Props.chunkSize;
                        int oY = j * Props.chunkSize;
                        foreach (Entity e in entityList)
                        {
                            int[] pos = SurfaceContainer.WorldToAbsoluteTileCoords(e.position.x, e.position.y);
                            float halfWidth = e.tileWidth / 2;
                            float halfHeight = e.tileHeight / 2;
                            entityArray.Append(new Vertex(new Vector2f(pos[0] - halfWidth , pos[1] - halfHeight), e.mapColor));
                            entityArray.Append(new Vertex(new Vector2f(pos[0] + halfWidth, pos[1] - halfHeight), e.mapColor));
                            entityArray.Append(new Vertex(new Vector2f(pos[0] - halfWidth, pos[1] + halfHeight), e.mapColor));

                            entityArray.Append(new Vertex(new Vector2f(pos[0] + halfWidth, pos[1] - halfHeight), e.mapColor));
                            entityArray.Append(new Vertex(new Vector2f(pos[0] + halfWidth, pos[1] + halfHeight), e.mapColor));
                            entityArray.Append(new Vertex(new Vector2f(pos[0] - halfWidth, pos[1] + halfHeight), e.mapColor));
                        }
                        if(entityArray.VertexCount > 0)
                        {
                            vertexArrays.Add(entityArray);
                        }
                    }
                    //
                    //foreach(Entity e in entityList)
                    //{
                    //int[] pos = SurfaceContainer.WorldToTileCoords(e.position.x, e.position.y);

                    //terrainImage.SetPixel((uint)pos[2], (uint)pos[3], e.mapColor);
                    /*
                    float[] pointList = e.collisionBox.GetPoints();
                    int[] min = SurfaceContainer.WorldToTileCoords(pointList[0] + e.position.x, pointList[1] + e.position.y);
                    int[] max = SurfaceContainer.WorldToTileCoords(pointList[4] + e.position.x, pointList[5] + e.position.y);
                    for(int k = min[2]; k <= max[2]; k++)
                    {
                        for(int l = min[3]; l <= max[3]; l++)
                        {
                            if (k < 0 || l < 0 || k > Props.chunkSize || l > Props.chunkSize)
                            {
                                continue;
                            }
                            terrainImage.SetPixel((uint)k, (uint)l, e.mapColor);
                        }
                    }
                    */
                    //}
                }
            }
        }

        public void RenderGUI(RenderWindow window, Camera camera)
        {
            window.SetView(camera.GetGUIView());
            GUI.Clear(Color.Transparent);
            menuContainer.RenderMenus(GUI);
            GUI.Display();
            Sprite GUISprite = new Sprite(GUI.Texture);
            window.Draw(GUISprite);
        }

        public void ResizeGUI(Object s, SizeEventArgs e)
        {
            GUI.Dispose();
            GUI = new RenderTexture(e.Width, e.Height);
        }

        /// <summary>
        /// Clears the entire cache of terrain vertices
        /// </summary>
        public void ClearVertexCache()
        {
            foreach(int key in terrainVertexArrays.Keys)
            {
                VertexArray[] vA;
                if(terrainVertexArrays.TryGetValue(key, out vA))
                {
                    foreach(VertexArray v in vA)
                    {
                        v.Dispose();
                    }
                }
            }
            terrainVertexArrays.Clear();
            foreach(int key in tileBoundingBoxVertexArray.Keys)
            {
                VertexArray vA;
                if(tileBoundingBoxVertexArray.TryGetValue(key, out vA))
                {
                    vA.Dispose();
                }
            }
            tileBoundingBoxVertexArray.Clear();
            foreach (int key in minimapVertexArrays.Keys)
            {
                VertexArray vA;
                if (tileBoundingBoxVertexArray.TryGetValue(key, out vA))
                {
                    vA.Dispose();
                }
            }
            minimapVertexArrays.Clear();

            entityBoundingBoxArray.Dispose();
            entityBoundingBoxArray.Clear();
        }
        
        /// <summary>
        /// Removes the terrain vertex array for a specified chunkIndex, if it is already present.  Rebuilding is handled by the RenderWorld function.
        /// </summary>
        /// <param name="chunkIndex"></param>
        public void RemoveCachedVertexArray(int chunkIndex)
        {
            
            if(terrainVertexArrays.ContainsKey(chunkIndex))
            {
                VertexArray[] vA;
                if(terrainVertexArrays.TryGetValue(chunkIndex, out vA))
                {
                    foreach(VertexArray v in vA)
                    {
                        v.Dispose();
                    }
                    terrainVertexArrays.Remove(chunkIndex);
                }
            }
            if(tileBoundingBoxVertexArray.ContainsKey(chunkIndex))
            {
                VertexArray vA;
                if(tileBoundingBoxVertexArray.TryGetValue(chunkIndex, out vA))
                {
                    vA.Dispose();
                    tileBoundingBoxVertexArray.Remove(chunkIndex);
                }
            }
            if(minimapVertexArrays.ContainsKey(chunkIndex))
            {
                VertexArray vA;
                if(minimapVertexArrays.TryGetValue(chunkIndex, out vA))
                {
                    vA.Dispose();
                    minimapVertexArrays.Remove(chunkIndex);
                }
            }
        }

        /// <summary>
        /// Culls vertex arrays that are too far away based on camera variables.
        /// </summary>
        public void CullVertexCache(Camera camera)
        {
            int[] cameraChunkCoords = SurfaceContainer.WorldToChunkCoords(camera.GetGameView().Center.X, camera.GetGameView().Center.Y);
            List<int> keysToRemove = new List<int>();

            foreach(int key in terrainVertexArrays.Keys)
            {
                int[] chunkIndices = SurfaceContainer.ChunkIndexToChunkCoords(key);
                if (Math.Abs(chunkIndices[0] - cameraChunkCoords[0]) > Props.vertexArrayCullingDistance ||
                    Math.Abs(chunkIndices[1] - cameraChunkCoords[1]) > Props.vertexArrayCullingDistance)
                {
                    keysToRemove.Add(key);
                }
            }
            foreach(int key in keysToRemove)
            {
                VertexArray[] vA;
                if(terrainVertexArrays.TryGetValue(key, out vA))
                {
                    foreach(VertexArray v in vA)
                    {
                        v.Dispose();
                    }
                }
                terrainVertexArrays.Remove(key);
            }
            keysToRemove.Clear();

            foreach(int key in tileBoundingBoxVertexArray.Keys)
            {
                int[] chunkIndices = SurfaceContainer.ChunkIndexToChunkCoords(key);
                if (Math.Abs(chunkIndices[0] - cameraChunkCoords[0]) > Props.vertexArrayCullingDistance ||
                    Math.Abs(chunkIndices[1] - cameraChunkCoords[1]) > Props.vertexArrayCullingDistance)
                {
                    keysToRemove.Add(key);
                }
            }
            foreach (int key in keysToRemove)
            {
                VertexArray vA;
                if(tileBoundingBoxVertexArray.TryGetValue(key, out vA))
                {
                    vA.Dispose();
                }
                tileBoundingBoxVertexArray.Remove(key);
            }
            keysToRemove.Clear();

            if(cullMinimap)
            {
                foreach (int key in minimapVertexArrays.Keys)
                {
                    int[] chunkIndices = SurfaceContainer.ChunkIndexToChunkCoords(key);
                    if (Math.Abs(chunkIndices[0] - cameraChunkCoords[0]) > Props.vertexArrayCullingDistance ||
                        Math.Abs(chunkIndices[1] - cameraChunkCoords[1]) > Props.vertexArrayCullingDistance)
                    {
                        keysToRemove.Add(key); 
                    }
                }
            }
            foreach (int key in keysToRemove)
            {
                VertexArray vA;
                if (minimapVertexArrays.TryGetValue(key, out vA))
                {
                    vA.Dispose();
                }
                minimapVertexArrays.Remove(key);
            }
        }

        public void DetachGameWorld()
        {
            tileCollection = null;
            surface = null;
        }

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
            if(input.keyPressed[InputBindings.showDebugMenu])
            {
                input.menuFactory.CreateDebugMenu(this);
            }
        }

        /// <summary>
        /// Useful for a debug menu.
        /// </summary>
        public void ToggleBoundingBoxRendering()
        {
            drawBoundingBoxes = !drawBoundingBoxes;
        }
        /// <summary>
        /// Useful for when displaying a controllable map in a menu.  Do not want to continuously cull the minimap and regenerate every frame.
        /// </summary>
        public void ToggleCullingMinimap()
        {

        }
    }
}
