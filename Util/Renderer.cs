﻿using System;
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
        VertexArray entityBoundingBoxArray;
        Texture[] terrainTilesheets;
        RenderTexture GUI;
        SpriteBatch lightingBatch;
        RenderStates[] terrainRenderStates;
        RenderStates guiState;
        List<Entity> drawList;
        SpriteBatch entityBatch;

        //Collections
        Dictionary<int, VertexArray[]> terrainVertexArrays; //TODO: need to change these for multi-surface support
        Dictionary<int, VertexArray> minimapVertexArrays; //TODO: need to change these for multi-surface support
        Dictionary<int, VertexArray> tileBoundingBoxVertexArray;
        TileCollection tileCollection;
        MenuContainer menuContainer;

        //Control variables
        bool drawBoundingBoxes = false;
        bool cullMinimap = true;
        int cullCounter = 0;
        int cullRate = 60;

        /// <summary>
        /// Indicates whether the internal vertex arrays have been modified.  Necessary to avoid protected memory access violations.
        /// </summary>
        public bool modifiedVertexArrays { get; protected set; } = false;

        public Renderer(RenderWindow window, MenuContainer menuContainer, Texture guiElements)
        {
            this.menuContainer = menuContainer;
            GUI = new RenderTexture(window.Size.X, window.Size.Y);
            guiState = new RenderStates(guiElements);
            lightingBatch = new SpriteBatch(window, BlendMode.Multiply);
            entityBatch = new SpriteBatch(window, BlendMode.Alpha);
        }
        public void InitializeForGame(TileCollection tileCollection)
        {
            //Vertex array caches
            terrainVertexArrays = new Dictionary<int, VertexArray[]>();
            minimapVertexArrays = new Dictionary<int, VertexArray>();
            tileBoundingBoxVertexArray = new Dictionary<int, VertexArray>();

            //dynamic vertex array
            entityBoundingBoxArray = new VertexArray(PrimitiveType.Lines);

            //internal references (tile collection is a single time instantiated array of reusable objects.
            this.tileCollection = tileCollection;
            terrainTilesheets = tileCollection.GetTerrainTilesheets();
            //Creation of renderstates for terrain drawing
            terrainRenderStates = new RenderStates[terrainTilesheets.Length];
            drawList = new List<Entity>();
            for (int i = 0; i < terrainTilesheets.Length; i++)
            {
                terrainRenderStates[i] = new RenderStates(terrainTilesheets[i]);
            }
        }
        public void RenderWorld(RenderWindow window, Camera camera, SurfaceContainer surface)
        {
            window.SetView(camera.GetGameView()); //Set view
            Vector2f origin = window.MapPixelToCoords(new Vector2i(0, 0), camera.GetGameView());
            Vector2f extent = window.MapPixelToCoords(new Vector2i((int)window.Size.X, (int)window.Size.Y), camera.GetGameView());
            int[] begPos = surface.WorldToChunkCoords(origin.X, origin.Y);
            int[] endPos = surface.WorldToChunkCoords(extent.X, extent.Y);
            #region terrain drawing
            for (int i = begPos[0]; i <= endPos[0]; i++)
            {
                for (int j = begPos[1]; j <= endPos[1]; j++)
                {
                    int key = (i) * surface.worldSize + j;
                    if (terrainVertexArrays.TryGetValue(key, out _) == false)
                    {
                        terrainVertexArrays.Add(key, tileCollection.GenerateTerrainVertices(surface, new int[] { i, j }));
                    }
                    VertexArray[] vArr;
                    if (terrainVertexArrays.TryGetValue(key, out vArr))
                    {
                        for (int k = 0; k < vArr.Length; k++)
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
                        //TODO: add Drawing box check for culling
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
            entityBatch.Initialize(camera.GetGameView(), Color.Transparent);
            foreach (Entity e in drawList)
            {
                e.drawArray[0].Draw(entityBatch, e.position.internalVector);
            }
            Sprite sprite = entityBatch.Finalize();
            window.SetView(camera.GetGUIView());
            window.Draw(sprite);
            window.SetView(camera.GetGameView());
            drawList.Clear();
            #endregion entity drawing

            #region lighting drawing
            lightingBatch.Initialize(camera.GetGameView(), new Color(0, 0, 0, surface.GetDarkness()));
            for (int i = begPos[0]; i <= endPos[0]; i++)
            {
                for (int j = begPos[1]; j <= endPos[1]; j++)
                {
                    Chunk chunk = surface.GetChunk(i, j);
                    List<LightSource> lightSources = chunk.lightSources;
                    for (int k = 0; k < lightSources.Count; k++)
                    {
                        lightingBatch.Draw(lightSources[k].light);
                    }
                }
            }
            Sprite lightingSprite = lightingBatch.Finalize();
            window.SetView(camera.GetGUIView());
            window.Draw(lightingSprite);
            window.SetView(camera.GetGameView());
            #endregion

            #region bounding box drawing
            if (drawBoundingBoxes == true)
            {
                for (int i = begPos[0]; i <= endPos[0]; i++)
                {
                    for (int j = begPos[1]; j <= endPos[1]; j++)
                    {
                        Chunk chunk = surface.GetChunk(i, j);
                        #region Tile bounding box drawing

                        int chunkIndex = i * surface.worldSize + j;
                        float[] pointsTile = surface.tileBox.GetPoints();
                        VertexArray vA;
                        if (!tileBoundingBoxVertexArray.TryGetValue(chunkIndex, out vA))
                        {
                            vA = tileCollection.GenerateTerrainBoundingBoxArray(surface, chunk, chunkIndex, pointsTile);
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
                    Chunk chunk = surface.GetChunk((i * surface.worldSize) + j, false);
                    if (chunk != null)
                    {
                        VertexArray vA;
                        if (!minimapVertexArrays.TryGetValue(i * surface.worldSize + j, out vA))
                        {
                            vA = tileCollection.GenerateTerrainMinimap(chunk, (i * surface.worldSize) + j, surface.worldSize);
                            minimapVertexArrays.Add(i * surface.worldSize + j, vA);
                        }
                        vertexArrays.Add(vA);
                        //next get entities
                        //This is very dynamic so there is no point caching it
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
                }
            }
        }

        public void GeneratePollutionVertexArray(SurfaceContainer surface, Vector2 position, int xRange, int yRange, VertexArray vA)
        {
            int[] chunkIndices = SurfaceContainer.WorldToChunkCoords(position);
            vA.Clear();
            for (int i = chunkIndices[0] - xRange; i <= chunkIndices[0] + xRange; i++)
            {
                for (int j = chunkIndices[1] - yRange; j <= chunkIndices[1] + yRange; j++)
                {
                    byte val = Convert.ToByte(surface.GetInterpolatedPollution(i, j) * 255 / Props.maxPollution);
                    Color topLeft = new Color(val, 0, 0, (byte)(val/2));
                    val = Convert.ToByte(surface.GetInterpolatedPollution(i + 1, j) * 255 / Props.maxPollution);
                    Color topRight = new Color(val, 0, 0, (byte)(val/2));
                    val = Convert.ToByte(surface.GetInterpolatedPollution(i, j + 1) * 255 / Props.maxPollution);
                    Color botLeft = new Color(val, 0, 0, (byte)(val/2));
                    val = Convert.ToByte(surface.GetInterpolatedPollution(i + 1, j + 1) * 255 / Props.maxPollution);
                    Color botRight = new Color(val, 0, 0, (byte)(val/2));
                    int oX = i * Props.chunkSize;
                    int oY = j * Props.chunkSize;
                    vA.Append(new Vertex(new Vector2f(oX, oY), topLeft));
                    vA.Append(new Vertex(new Vector2f(oX + Props.chunkSize, oY), topRight));
                    vA.Append(new Vertex(new Vector2f(oX, oY + Props.chunkSize), botLeft));
                    vA.Append(new Vertex(new Vector2f(oX + Props.chunkSize, oY), topRight));
                    vA.Append(new Vertex(new Vector2f(oX + Props.chunkSize, oY + Props.chunkSize), botRight));
                    vA.Append(new Vertex(new Vector2f(oX, oY + Props.chunkSize), botLeft));
                }
            }
        }

        public void RenderGUI(RenderWindow window, Camera camera)
        {
            window.SetView(camera.GetGUIView());
            GUI.Clear(Color.Transparent);
            menuContainer.RenderMenus(GUI, guiState);
            GUI.Display();
            Sprite GUISprite = new Sprite(GUI.Texture);
            window.Draw(GUISprite);
            cullCounter++;
        }

        public void RenderHeldItem(RenderWindow window, Camera camera, ItemStack heldItem, InputManager input)
        {
            if(heldItem == null)
            {
                return;
            }
            window.SetView(camera.GetGUIView());
            Vector2f pos = input.GetMousePosition();
            RectangleShape item = new RectangleShape(new Vector2f(32,32));
            item.Position = pos;
            window.Draw(item);
        }

        public void HandleResize(Object s, SizeEventArgs e)
        {
            GUI.Dispose();
            GUI = new RenderTexture(e.Width, e.Height);

            lightingBatch.HandleResize(s, e);
            entityBatch.HandleResize(s, e);
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
                        if (v != null)
                        {
                            v.Dispose();
                        }
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
                modifiedVertexArrays = true;
            }
        }

        public void CheckCullVertexCache(Camera camera, SurfaceContainer surface)
        {
            if (cullCounter > cullRate)
            {
                CullVertexCache(camera, surface);
                cullCounter = 0;
            }
            modifiedVertexArrays = false;
        }
        /// <summary>
        /// Culls vertex arrays that are too far away based on camera variables.
        /// </summary>
        public void CullVertexCache(Camera camera, SurfaceContainer surface)
        {
            int[] cameraChunkCoords = surface.WorldToChunkCoords(camera.GetGameView().Center.X, camera.GetGameView().Center.Y);
            List<int> keysToRemove = new List<int>();

            foreach(int key in terrainVertexArrays.Keys)
            {
                int[] chunkIndices = surface.ChunkIndexToChunkCoords(key);
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
                int[] chunkIndices = surface.ChunkIndexToChunkCoords(key);
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
                    int[] chunkIndices = surface.ChunkIndexToChunkCoords(key);
                    if (Math.Abs(chunkIndices[0] - cameraChunkCoords[0]) > Props.vertexArrayCullingDistance ||
                        Math.Abs(chunkIndices[1] - cameraChunkCoords[1]) > Props.vertexArrayCullingDistance)
                    {
                        keysToRemove.Add(key); 
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
        }

        public void DetachGameWorld()
        {
            tileCollection = null;
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
            if(input.GetKeyPressed(InputBindings.showDebugMenu, true))
            {
                input.menuFactory.CreateDebugMenu();
            }
        }

        /// <summary>
        /// Useful for a debug menu.
        /// </summary>
        public void ToggleBoundingBoxRendering(string tag)
        {
            //TODO: change to accept a bool and use a checkmark box with this
            drawBoundingBoxes = !drawBoundingBoxes;
        }
        /// <summary>
        /// Useful for when displaying a controllable map in a menu.  Do not want to continuously cull the minimap and regenerate every frame.
        /// </summary>
        public void ToggleCullingMinimap(string tag)
        {
            cullMinimap = !cullMinimap;
        }
    }
}
