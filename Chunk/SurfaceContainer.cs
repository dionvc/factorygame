using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EngineeringCorpsCS
{
    //stores collection of chunks, manages saving, loading, and generating chunks
    //stores collection of active chunks (to run update on)
    
    class SurfaceContainer
    {
        Chunk[] chunks;
        List<Chunk> activeChunks;
        SurfaceGenerator surfaceGenerator;
        Dictionary<int[], List<Entity>> queuedCollisionEntities;
        //note: the clock is modeled with midday being 0:00 so that a bell curve can be easily modeled around midnight
        //midnight is assumed to be at halfway through the day
        public int timeOfDay;
        public int timeOfMidday; //when the clock rolls over
        public int lengthOfNight; //the total length of night as one standard deviation
        private byte levelOfDarkness; //the maximum byte value for darkness

        private int emissionUpdateRate = 60;
        private int emissionCounter = 0;
        public BoundingBox tileBox { get; protected set; }
        public TileCollection tileCollection { get; protected set; }
        public int worldSize { get; protected set; }

        public Vector2 spawnPoint { get; protected set; }
        int startingRadius = 50;

        public SurfaceContainer(TileCollection tileCollection, SurfaceGenerator surfaceGenerator)
        {
            this.worldSize = surfaceGenerator.surfaceSize / Props.chunkSize;
            tileBox = new BoundingBox(-16, -16, 16, 16);
            this.tileCollection = tileCollection;
            chunks = new Chunk[worldSize * worldSize];
            this.surfaceGenerator = surfaceGenerator;
            activeChunks = new List<Chunk>();
            spawnPoint = new Vector2(worldSize * Props.chunkSize * Props.tileSize / 2, worldSize * Props.chunkSize * Props.tileSize / 2);
            this.timeOfMidday = surfaceGenerator.lengthOfDay;
            this.lengthOfNight = surfaceGenerator.lengthOfNight;
            this.levelOfDarkness = surfaceGenerator.levelOfDarkness;
            timeOfDay = 0;

            queuedCollisionEntities = new Dictionary<int[], List<Entity>>();
        }

        public void Update()
        {
            timeOfDay++;
            timeOfDay %= timeOfMidday;
            if (emissionCounter > emissionUpdateRate)
            {
                for (int i = 0; i < activeChunks.Count; i++)
                {
                    activeChunks[i].Update();
                    if (activeChunks[i].pollutionValue >= Props.maxPollution || activeChunks[i].pollutionValue <= 0)
                    {
                        activeChunks.RemoveAt(i);
                        i--;
                    }
                }
                emissionCounter = 0;
            }
            emissionCounter++;
        }
        public void GenerateTerrain(int x, int y)
        {
            Chunk chunk = new Chunk();
            chunk.GenerateTerrain((x) * Props.chunkSize, (y) * Props.chunkSize, surfaceGenerator);
            SetChunk(x * worldSize + y, chunk);
            List<Entity> queuedEntities;
            if (queuedCollisionEntities.TryGetValue(new int[] { x, y }, out queuedEntities))
            {
                for(int i = 0; i < queuedEntities.Count; i++)
                {
                    chunk.AddEntityCollisionCheck(queuedEntities[i]);
                }
            }
            chunk.GenerateEntities(x, y, surfaceGenerator, this);
            activeChunks.Add(chunk);
        }

        public void GenerateStartingArea()
        {
            int oX = (int) (spawnPoint.x / (Props.tileSize * Props.chunkSize));
            int oY = (int)(spawnPoint.y / (Props.tileSize * Props.chunkSize));
            int tileX = (int)(spawnPoint.x / (Props.tileSize));
            int tileY = (int)(spawnPoint.y / (Props.tileSize));
            for (int x = oX - 2; x <= oX + 2; x++)
            {
                for (int y = oY - 2; y <= oY + 2; y++)
                {
                    int chunkTileX = x * Props.chunkSize;
                    int chunkTileY = y * Props.chunkSize;
                    Chunk chunk = new Chunk();
                    chunk.GenerateTerrain((x) * Props.chunkSize, (y) * Props.chunkSize, surfaceGenerator);
                    SetChunk(x * worldSize + y, chunk);
                    for(int i = 0; i < Props.chunkSize; i++)
                    {
                        for(int j = 0; j < Props.chunkSize; j++)
                        {
                            float dist = (float) Math.Sqrt(((chunkTileX + i - tileX) * (chunkTileX + i - tileX)) + ((chunkTileY + j - tileY) * (chunkTileY + j - tileY)));
                            if (dist < startingRadius) 
                            {
                                chunk.SetTile(i, j, 2);
                            }
                        }
                    }
                    chunk.GenerateEntities(x, y, surfaceGenerator, this);
                    activeChunks.Add(chunk);
                }
            }
        }
        public Chunk GetChunk(int x, int y)
        {
            if(x < 0 || x >= worldSize || y < 0 || y >= worldSize)
            {
                return null;
            }
            if (chunks[x * worldSize + y] != null)
            {
                return chunks[x * worldSize + y];
            }
            else
            {
                GenerateTerrain(x, y);
                return chunks[x * worldSize + y];
            }
        }
        public Chunk GetChunk(int[] xy)
        {
            if (xy[0] < 0 || xy[0] >= worldSize || xy[1] < 0 || xy[1] >= worldSize)
            {
                return null;
            }
            if (chunks[xy[0] * worldSize + xy[1]] != null)
            {
                return chunks[xy[0] * worldSize + xy[1]];
            }
            else
            {
                GenerateTerrain(xy[0], xy[1]);
                return chunks[xy[0] * worldSize + xy[1]];
            }
        }

        public Chunk GetChunk(int chunkIndex, bool generate)
        {
            if(chunkIndex < 0 || chunkIndex > (worldSize * worldSize) - 1)
            {
                return null;
            }
            if (chunks[chunkIndex] != null || generate == false)
            {
                return chunks[chunkIndex];
            }
            else
            {
                int[] chunkXY = ChunkIndexToChunkCoords(chunkIndex);
                GenerateTerrain(chunkXY[0] , chunkXY[1]);
                return chunks[chunkIndex];
            }
        }

        public void SetChunk(int chunkIndex, Chunk chunk)
        {
            chunks[chunkIndex] = chunk;
        }

        /// <summary>
        /// Called on creation of an entity
        /// </summary>
        /// <param name="entity"></param>
        public void InitiateEntityInChunks(Entity entity)
        {
            int chunkIndex = WorldToChunkIndex(entity.position);
            int[] cXY = WorldToChunkCoords(entity.position);
            entity.centeredChunk = chunkIndex;
            entity.surface = this;
            GetChunk(chunkIndex, true).AddEntityToChunk(entity);
            int[] newCollisionChunks = BoundingBox.GetChunkBounds(entity.collisionBox, entity.position, entity.surface);
            foreach (int x in newCollisionChunks)
            {
                Chunk collisionChunk = GetChunk(x, false);
                if(collisionChunk == null)
                {
                    List<Entity> queuedEntities;
                    if(queuedCollisionEntities.TryGetValue(cXY, out queuedEntities))
                    {
                        queuedEntities.Add(entity);
                    }
                    else
                    {
                        queuedEntities = new List<Entity>();
                        queuedEntities.Add(entity);
                        queuedCollisionEntities.Add(cXY, queuedEntities);
                    }
                }
                else
                {
                    collisionChunk.AddEntityCollisionCheck(entity);
                }
            }
            entity.collisionChunks = newCollisionChunks;
        }

        /// <summary>
        /// Moves references of an entity between chunks.  Both its main chunk and chunks it can collide in.  Should be called by all movement functionality.
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="prevPos"></param>
        /// <param name="transPos"></param>
        /// <param name="centeredChunk"></param>
        /// <param name="collisionChunks"></param>
        public void UpdateEntityInChunks(Entity entity)
        {
            //The entity's draw/main chunk is updated first
            int newChunkIndex = WorldToChunkIndex(entity.position);
            if (entity.centeredChunk != newChunkIndex)
            {
                GetChunk(entity.centeredChunk, false).RemoveEntityFromChunk(entity);
                entity.centeredChunk = newChunkIndex;
                GetChunk(newChunkIndex, true).AddEntityToChunk(entity);
            }
            //Now, the entity's collision chunks are computed
            int[] newCollisionChunks  = BoundingBox.GetChunkBounds(entity.collisionBox, entity.position, entity.surface);
            foreach (int x in newCollisionChunks)
            {
                if(!entity.collisionChunks.Contains(x))
                {
                    GetChunk(x, true).AddEntityCollisionCheck(entity);
                }
            }
            foreach (int x in entity.collisionChunks)
            {
                if(!newCollisionChunks.Contains(x))
                {
                    GetChunk(x, false).RemoveEntityCollisionCheck(entity);
                }
            }
            entity.collisionChunks = newCollisionChunks;
        }


        /// <summary>
        /// Called on destruction or transferring to another surface
        /// </summary>
        /// <param name="entity"></param>
        public void RemoveEntity(Entity entity)
        {
            GetChunk(entity.centeredChunk, false).RemoveEntityFromChunk(entity);
            foreach (int x in entity.collisionChunks)
            {
                GetChunk(x, false).RemoveEntityCollisionCheck(entity);
            }
        }

        /// <summary>
        /// Updates a light source into a chunk
        /// </summary>
        /// <param name="lightSource"></param>
        public void UpdateLightSource(LightSource lightSource)
        {
            int chunkIndex = WorldToChunkIndex(lightSource.position);
            if(chunkIndex != lightSource.centeredChunk)
            {
                Chunk removeChunk = GetChunk(lightSource.centeredChunk, false);
                if (removeChunk != null) {
                    removeChunk.RemoveLightSource(lightSource);
                }
                lightSource.centeredChunk = chunkIndex;
                GetChunk(lightSource.centeredChunk, true).AddLightSource(lightSource);
            }
        }

        /// <summary>
        /// Removes a light source from a chunk
        /// </summary>
        /// <param name="lightSource"></param>
        public void RemoveLightSource(LightSource lightSource)
        {
            GetChunk(WorldToChunkIndex(lightSource.position), false).RemoveLightSource(lightSource);
        }
        
        /// <summary>
        /// Interpolates the pollution for the four surrounding chunks of cX, cY
        /// </summary>
        /// <param name="cX"></param>
        /// <param name="cY"></param>
        /// <returns></returns>
        public float GetInterpolatedPollution(int cX, int cY)
        {
            if(cX < 0 || cY < 0 || cX > worldSize || cY > worldSize)
            {
                return 0;
            }
            Chunk chunk = GetChunk(cX * worldSize + cY, false);
            float interpPollution = 0;
            int chunkCounter = 0;
            if(chunk != null)
            {
                interpPollution += chunk.pollutionValue;
                chunkCounter++;
            }
            chunk = GetChunk((cX - 1) * worldSize + cY, false);
            if(chunk != null)
            {
                interpPollution += chunk.pollutionValue;
                chunkCounter++;
            }
            chunk = GetChunk(cX * worldSize + cY, false);
            if(chunk != null)
            {
                interpPollution += chunk.pollutionValue;
                chunkCounter++;
            }
            chunk = GetChunk(cX * worldSize + cY, false);
            if(chunk!= null)
            {
                interpPollution += chunk.pollutionValue;
                chunkCounter++;
            }
            if(chunkCounter == 0)
            {
                return 0;
            }
            return interpPollution / chunkCounter;
        }

        /// <summary>
        /// Gets the value of darkness as a drawable byte.
        /// </summary>
        /// <returns></returns>
        public byte GetDarkness()
        {
            double a = timeOfDay - (timeOfMidday / 2);
            double b = (2 * lengthOfNight * lengthOfNight);
            double c = - a * a / b;
            double darkness = levelOfDarkness * Math.Exp(c) - 1;
            return Convert.ToByte(darkness);
        }

        /// <summary>
        /// Gets the tile bounding the provided vector2
        /// </summary>
        /// <param name="pos"></param>
        /// <returns></returns>
        public byte GetTileFromWorld(Vector2 pos)
        {
            int chunkIndex = WorldToChunkIndex(pos);
            int tileIndex = WorldToTileIndex(pos);
            return GetChunk(chunkIndex, true).GetTile(tileIndex);
        }

        /// <summary>
        /// Gets a tile value from world coordinates provided as integers.
        /// </summary>
        /// <param name="cXY"></param>
        /// <param name="i"></param>
        /// <param name="j"></param>
        /// <returns></returns>
        public byte GetTileFromWorldInt(int[] cXY, int i, int j)
        {
            int iN = (i % Props.chunkSize + Props.chunkSize) % Props.chunkSize;
            int jN = (j % Props.chunkSize + Props.chunkSize) % Props.chunkSize;
            Chunk chunk = GetChunk(new int[] { cXY[0] + (int)Math.Floor(i * 1.0 / Props.chunkSize), cXY[1] + (int)Math.Floor(j * 1.0 / Props.chunkSize) });
            if (chunk == null)
            {
                return 0; //void
            }
            return chunk.GetTile(iN, jN);
        }

        public byte GetTileFromWorldInt(int tileX, int tileY)
        {
            Chunk chunk = GetChunk(tileX / Props.chunkSize, tileY / Props.chunkSize);
            if (chunk == null)
            {
                return 0;
            }
            return chunk.GetTile(tileX % Props.chunkSize, tileY % Props.chunkSize);
        }

        public Vector2 WorldToTileVector(int chunkIndex, int tileIndex)
        {
            float x = 16.5f + (chunkIndex / worldSize) * Props.chunkSize * Props.tileSize;
            float y = 16.5f + (chunkIndex % worldSize) * Props.chunkSize * Props.tileSize;
            x += (tileIndex / Props.chunkSize) * Props.tileSize;
            y += (tileIndex % Props.chunkSize) * Props.tileSize;
            return new Vector2(x, y);
        }

        /// <summary>
        /// Returns the chunk's index at a specified world coordinate
        /// </summary>
        /// <param name="pos"></param>
        /// <returns></returns>
        public int WorldToChunkIndex(Vector2 pos)
        {
            int[] wC = WorldToChunkCoords(pos);
            return wC[0] * worldSize + wC[1];
        }

        /// <summary>
        /// Returns the top left corner chunk coordinates of a specified chunk by index
        /// </summary>
        /// <param name="chunkIndex"></param>
        /// <returns></returns>
        public int[] ChunkIndexToChunkCoords(int chunkIndex)
        {
            return new int[] { (chunkIndex / worldSize), (chunkIndex % worldSize) };
        }

        #region Coordinate conversions TODO: Cull this section down to "index" type functions
        /// <summary>
        /// Converts world coordinates to chunk coordinates
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns>x,y chunk coords</returns>
        public int[] WorldToChunkCoords(float x, float y)
        {
            x = x < 0 ? x = 0 : x;
            y = y < 0 ? y = 0 : y;
            x = x >= worldSize * Props.chunkSize * Props.tileSize ? x = (worldSize-1) * Props.chunkSize * Props.tileSize : x;
            y = y >= worldSize * Props.chunkSize * Props.tileSize ? y = (worldSize-1) * Props.chunkSize * Props.tileSize : y;
            return new int[] {(int) Math.Floor(x/(Props.tileSize * Props.chunkSize)), (int) Math.Floor(y/(Props.tileSize * Props.chunkSize))};
        }

        /// <summary>
        /// Converts a vector2 to chunk coords
        /// </summary>
        /// <param name="pos"></param>
        /// <returns></returns>
        public static int[] WorldToChunkCoords(Vector2 pos)
        {
            return new int[] { (int)Math.Floor(pos.x / (Props.tileSize * Props.chunkSize)), (int)Math.Floor(pos.y / (Props.tileSize * Props.chunkSize)) };
        }

        
        /// <summary>
        /// Returns the chunk's index at a specified world coordinate
        /// </summary>
        /// <param name="pos"></param>
        /// <returns></returns>
        public int WorldToChunkIndex(float x, float y)
        {
            int[] wC = WorldToChunkCoords(x, y);
            return wC[0] * worldSize + wC[1];
        }

        /// <summary>
        /// Converts chunk coordinates to world coordinates
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns>x,y world coords</returns>
        public static int[] ChunkToWorldCoords(int x, int y)
        {
            return new int[] {x * Props.tileSize * Props.chunkSize, y * Props.chunkSize};
        }

        /// <summary>
        /// Converts world coordinates to chunk coordinates
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns>tile x, tile y, chunk x, chunk y</returns>
        public static int[] WorldToTileCoords(float x, float y)
        {
            int xR = (int)Math.Floor(x/Props.tileSize);
            int yR = (int)Math.Floor(y/Props.tileSize);
            xR = xR < 0 ? 0 : xR;
            yR = yR < 0 ? 0 : yR;
            return new int[] { xR/Props.chunkSize, yR/Props.chunkSize, xR % Props.chunkSize, yR % Props.chunkSize };
        }
        public int[] WorldToAbsoluteTileCoords(float x, float y)
        {
            int xR = (int)Math.Floor(x / Props.tileSize);
            int yR = (int)Math.Floor(y / Props.tileSize);
            return new int[] { xR, yR };
        }

        public static int WorldToTileIndex(Vector2 pos)
        {
            int xR = ((int)Math.Floor(pos.x / Props.tileSize)) % Props.chunkSize; //coords in tile
            int yR = ((int)Math.Floor(pos.y / Props.tileSize)) % Props.chunkSize; //coords in tile
            return xR * Props.chunkSize + yR;
        }
        public static int[] TileToWorldCoords(int x, int y, int cx, int cy)
        {
            return new int[] { (cx * Props.chunkSize + x) * Props.tileSize, (cy * Props.chunkSize + x) * Props.tileSize };
        }
        #endregion Coordinate Conversions
    }
}
