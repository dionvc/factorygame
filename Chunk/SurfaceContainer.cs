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
        FastNoise elevationNoise;
        FastNoise moistureNoise;
        FastNoise temperatureNoise;
        public BoundingBox tileBox { get; protected set; }
        public TileCollection tileCollection { get; protected set; }

        public SurfaceContainer(TileCollection tileCollection)
        {
            tileBox = new BoundingBox(-16, -16, 16, 16);
            this.tileCollection = tileCollection;
            chunks = new Chunk[Props.worldSize * Props.worldSize];
            Random r = new Random(DateTime.Now.Second);
            elevationNoise = new FastNoise();
            elevationNoise.SetNoiseType(FastNoise.NoiseType.Perlin);
            elevationNoise.SetSeed(r.Next(0, 10000));

            moistureNoise = new FastNoise();
            moistureNoise.SetNoiseType(FastNoise.NoiseType.Perlin);
            moistureNoise.SetSeed(r.Next(0, 10000));

            temperatureNoise = new FastNoise();
            temperatureNoise.SetNoiseType(FastNoise.NoiseType.Perlin);
            temperatureNoise.SetSeed(r.Next(0, 10000));
        }

        public void Update()
        {
            for(int i = 0; i < activeChunks.Count; i++)
            {
                activeChunks[i].Update();
            }
        }
        public void GenerateTerrain(int x, int y)
        {
            Chunk chunk = new Chunk();
            chunk.GenerateTerrain((x) * Props.chunkSize, (y) * Props.chunkSize, elevationNoise, moistureNoise, temperatureNoise);
            SetChunk(x, y, chunk);
        }
        public Chunk GetChunk(int x, int y)
        {
            if(x < 0 || x >= Props.worldSize || y < 0 || y >= Props.worldSize)
            {
                return null;
            }
            if (chunks[x * Props.worldSize + y] != null)
            {
                return chunks[x * Props.worldSize + y];
            }
            else
            {
                GenerateTerrain(x, y);
                return chunks[x * Props.worldSize + y];
            }
        }
        public Chunk GetChunk(int[] xy)
        {
            if (xy[0] < 0 || xy[0] >= Props.worldSize || xy[1] < 0 || xy[1] >= Props.worldSize)
            {
                return null;
            }
            if (chunks[xy[0] * Props.worldSize + xy[1]] != null)
            {
                return chunks[xy[0] * Props.worldSize + xy[1]];
            }
            else
            {
                GenerateTerrain(xy[0], xy[1]);
                return chunks[xy[0] * Props.worldSize + xy[1]];
            }
        }

        public Chunk GetChunk(int chunkIndex)
        {
            if(chunkIndex < 0 || chunkIndex > (Props.worldSize * Props.worldSize))
            {
                return null;
            }
            if (chunks[chunkIndex] != null)
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

        public void SetChunk(int x, int y, Chunk chunk)
        {
            chunks[x * Props.worldSize + y] = chunk;
        }

        /// <summary>
        /// Called on creation of an entity
        /// </summary>
        /// <param name="entity"></param>
        public void InitiateEntityInChunks(Entity entity)
        {
            int chunkIndex = WorldToChunkIndex(entity.position);
            entity.centeredChunk = chunkIndex;
            entity.surface = this;
            GetChunk(chunkIndex).AddEntityToChunk(entity);
            int[] newCollisionChunks = BoundingBox.GetChunkBounds(entity.collisionBox, entity.position);
            foreach (int x in newCollisionChunks)
            {
                GetChunk(x).AddEntityCollisionCheck(entity);
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
        public void UpdateEntityInChunks(Entity entity, Vector2 transPos)
        {
            //The entity's draw/main chunk is updated first
            Vector2 newPos = entity.position.VAdd(transPos);
            int newChunkIndex = WorldToChunkIndex(newPos);
            if (entity.centeredChunk != newChunkIndex)
            {
                GetChunk(entity.centeredChunk).RemoveEntityFromChunk(entity);
                entity.centeredChunk = newChunkIndex;
                GetChunk(newChunkIndex).AddEntityToChunk(entity);
            }
            //Now, the entity's collision chunks are computed
            int[] newCollisionChunks  = BoundingBox.GetChunkBounds(entity.collisionBox, newPos);
            foreach (int x in newCollisionChunks)
            {
                if(!entity.collisionChunks.Contains(x))
                {
                    GetChunk(x).AddEntityCollisionCheck(entity);
                }
            }
            foreach (int x in entity.collisionChunks)
            {
                if(!newCollisionChunks.Contains(x))
                {
                    GetChunk(x).RemoveEntityCollisionCheck(entity);
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
            GetChunk(entity.centeredChunk).RemoveEntityFromChunk(entity);
            foreach (int x in entity.collisionChunks)
            {
                GetChunk(x).RemoveEntityCollisionCheck(entity);
            }
        }

        

        #region Coordinate conversions TODO: Cull this section down to "index" type functions
        /// <summary>
        /// Converts world coordinates to chunk coordinates
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns>x,y chunk coords</returns>
        public static int[] WorldToChunkCoords(float x, float y)
        {
            x = x < 0 ? x = 0 : x;
            y = y < 0 ? y = 0 : y;
            //TODO: add extremity chunk check (eg x > worldsize... or whatever)
            return new int[] {(int) Math.Floor(x/(Props.tileSize * Props.chunkSize)), (int) Math.Floor(y/(Props.tileSize * Props.chunkSize))};
        }

        public static int[] WorldToChunkCoords(Vector2 pos)
        {
            return new int[] { (int)Math.Floor(pos.x / (Props.tileSize * Props.chunkSize)), (int)Math.Floor(pos.y / (Props.tileSize * Props.chunkSize)) };
        }

        /// <summary>
        /// Returns the chunk's index at a specified world coordinate
        /// </summary>
        /// <param name="pos"></param>
        /// <returns></returns>
        public static int WorldToChunkIndex(Vector2 pos)
        {
            int[] wC = WorldToChunkCoords(pos);
            return wC[0] * Props.worldSize + wC[1];
        }
        /// <summary>
        /// Returns the chunk's index at a specified world coordinate
        /// </summary>
        /// <param name="pos"></param>
        /// <returns></returns>
        public static int WorldToChunkIndex(float x, float y)
        {
            int[] wC = WorldToChunkCoords(x, y);
            return wC[0] * Props.worldSize + wC[1];
        }

        /// <summary>
        /// Returns the top left corner world coordinates of a specified chunk by index
        /// </summary>
        /// <param name="chunkIndex"></param>
        /// <returns></returns>
        public static int[] ChunkIndexToChunkCoords(int chunkIndex)
        {
            return new int[] { (chunkIndex / Props.worldSize), (chunkIndex % Props.worldSize) };
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
            return new int[] { xR/Props.chunkSize, yR/Props.chunkSize, xR % Props.chunkSize, yR % Props.chunkSize };
        }

        public static int[] WorldToAbsoluteTileCoords(float x, float y)
        {
            int xR = (int)Math.Floor(x / Props.tileSize);
            int yR = (int)Math.Floor(y / Props.tileSize);
            return new int[] { xR, yR };
        }
        
        public static Vector2 WorldToTileVector(int chunkIndex, int tileIndex)
        {
            float x = 16.5f + (chunkIndex / Props.worldSize) * Props.chunkSize * Props.tileSize;
            float y = 16.5f + (chunkIndex % Props.worldSize) * Props.chunkSize * Props.tileSize;
            x += (tileIndex / Props.chunkSize) * Props.tileSize;
            y += (tileIndex % Props.chunkSize) * Props.tileSize;
            return new Vector2(x, y);
        }
        
        public static int WorldToTileIndex(Vector2 pos)
        {
            int xR = ((int)Math.Floor(pos.x / Props.tileSize)) % Props.chunkSize; //coords in tile
            int yR = ((int)Math.Floor(pos.y / Props.tileSize)) % Props.chunkSize; //coords in tile
            return xR * Props.chunkSize + yR;
        }

        public byte GetTileFromWorld(float x, float y)
        {
            int[] xyij = WorldToTileCoords(x, y);
            Chunk chunk = GetChunk(new int[] { xyij[0], xyij[1] });
            if(chunk == null)
            {
                return 0;
            }
            return chunk.GetTile(xyij[2], xyij[3]);
        }

        public byte GetTileFromWorld(Vector2 pos)
        {
            int chunkIndex = WorldToChunkIndex(pos);
            int tileIndex = WorldToTileIndex(pos);
            return GetChunk(chunkIndex).GetTile(tileIndex);
        }

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

        public static int[] TileToWorldCoords(int x, int y, int cx, int cy)
        {
            return new int[] { (cx * Props.chunkSize + x) * Props.tileSize, (cy * Props.chunkSize + x) * Props.tileSize };
        }
        #endregion Coordinate Conversions
    }
}
