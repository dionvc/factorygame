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
    class Renderer: IInputSubscriber
    {
        //Graphics variables
        VertexArray boundingBoxArray; //must be reconstructed every frame TODO: split terrain bounding box array into cachable set
        Texture[] terrainTilesheets;
        RenderTexture GUI;
        RenderStates[] terrainRenderStates;
        List<Entity> drawList;

        //Collections
        Dictionary<int, VertexArray[]> terrainVertexArrays;
        TileCollection tileCollection;
        SurfaceContainer surface;
        MenuContainer menuContainer;

        //Control variables
        bool drawBoundingBoxes = true;

        public Renderer( Window window, MenuContainer menuContainer)
        {
            this.menuContainer = menuContainer;
            GUI = new RenderTexture(window.Size.X, window.Size.Y);
        }
        public void InitializeForGame(SurfaceContainer chunkManager, TileCollection tileCollection)
        {
            this.surface = chunkManager;
            this.tileCollection = tileCollection;
            terrainVertexArrays = new Dictionary<int, VertexArray[]>(); //terrain cache //TODO: clear periodically
            boundingBoxArray = new VertexArray(PrimitiveType.Lines);
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
            
            for (int i = begPos[0]; i <= endPos[0]; i++)
            {
                for (int j = begPos[1]; j <= endPos[1]; j++)
                {

                    Chunk chunk = surface.GetChunk(i, j);
                    #region Tile bounding box drawing
                    if (drawBoundingBoxes == true)
                    {
                        float[] pointsTile = surface.tileBox.GetPoints();
                        for (int k = 0; k < Props.chunkSize; k++)
                        {
                            for (int l = 0; l < Props.chunkSize; l++)
                            {
                                Tile tile = tileCollection.GetTerrainTile(chunk.GetTile(k, l));
                                if ((tile.collisionMask & Base.CollisionLayer.TerrainSolid) != 0)
                                {

                                    Vector2 position = SurfaceContainer.WorldToTileVector(i * Props.worldSize + j, k * Props.chunkSize + l);
                                    if (position.x > origin.X && position.x < extent.X &&
                                        position.y > origin.Y && position.y < extent.Y)
                                    {
                                        for (int x = 0; x < pointsTile.Length; x += 2)
                                        {
                                            boundingBoxArray.Append(new Vertex(new Vector2f(pointsTile[x] + position.x, pointsTile[x + 1] + position.y), Color.Blue));
                                            boundingBoxArray.Append(new Vertex(new Vector2f(pointsTile[(x + 2) % 8] + position.x, pointsTile[(x + 3) % 8] + position.y), Color.Blue));
                                        }
                                    }
                                }
                            }
                        }
                    }
                    #endregion Tile bounding box drawing
                    List<Entity> entityList = chunk.entityList;
                    for (int k = 0; k < entityList.Count; k++)
                    {
                        //TODO: More elegant clipping method needed (or even just an approximation method)
                        if (entityList[k].position.x > origin.X && entityList[k].position.x < extent.X
                            && entityList[k].position.y > origin.Y && entityList[k].position.y < extent.Y)
                        {
                            drawList.Add(entityList[k]);
                            #region Entity bounding box drawing
                            if (drawBoundingBoxes == true)
                            {
                                float[] pointsEntity = entityList[k].collisionBox.GetPoints();
                                Vector2 position = entityList[k].position;
                                for (int l = 0; l < pointsEntity.Length; l += 2)
                                {
                                    boundingBoxArray.Append(new Vertex(new Vector2f(pointsEntity[l] + position.x, pointsEntity[l + 1] + position.y), Color.Red));
                                    boundingBoxArray.Append(new Vertex(new Vector2f(pointsEntity[(l + 2) % 8] + position.x, pointsEntity[(l + 3) % 8] + position.y), Color.Red));
                                }
                            }
                            #endregion Entity bounding box drawing
                        }
                    }
                }
                if (drawBoundingBoxes == true) {
                    window.Draw(boundingBoxArray);
                    boundingBoxArray.Clear();
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

        /// <summary>
        /// Clears the entire cache of terrain vertices
        /// </summary>
        public void ClearTerrainVertexCache()
        {
            terrainVertexArrays.Clear();
        }
        
        /// <summary>
        /// Removes the terrain vertex array for a specified chunkIndex, if it is already present.  Rebuilding is handled by the RenderWorld function.
        /// </summary>
        /// <param name="chunkIndex"></param>
        public void RemoveCachedTerrainVertexArray(int chunkIndex)
        {
            if(terrainVertexArrays.ContainsKey(chunkIndex))
            {
                terrainVertexArrays.Remove(chunkIndex);
            }
        }

        /// <summary>
        /// Culls terrain vertex arrays that are too far away based on camera variables.
        /// </summary>
        public void CullTerrainVertexCache(Camera camera)
        {
            //TODO: Finish culling algorithm
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

        public void ToggleBoundingBoxRendering()
        {
            drawBoundingBoxes = !drawBoundingBoxes;
        }
    }
}
