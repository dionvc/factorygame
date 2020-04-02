using SFML.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EngineeringCorpsCS
{
    class BoundingBox
    {
        //Vector representation
        Vector2 topLeft { get; set; }
        Vector2 botRight { get; set; }
        //Constants
        float radiusApproximation { get; set; }
        float halfWidth { get; set; }
        float width { get; set; }
        float halfHeight { get; set; }
        float height { get; set; }

        //external representation rotated
        int rotation;
        Vector2 topLeftR;
        Vector2 topRightR;
        Vector2 botLeftR;
        Vector2 botRightR;

        Vector2[] normals = new Vector2[2] { new Vector2(0, 0), new Vector2(0, 0) };

        /// <summary>
        /// Create an axis aligned bounding box
        /// </summary>
        /// <param name="tx"></param>
        /// <param name="ty"></param>
        /// <param name="bx"></param>
        /// <param name="by"></param>
        public BoundingBox(float tx, float ty, float bx, float by)
        {
            topLeft = new Vector2(tx, ty);
            botRight = new Vector2(bx, by);
            
            this.SetRotation(0);
            this.CalculateConstants();
        }

        /// <summary>
        /// Creates bounding box uniformly around center of entity
        /// </summary>
        /// <param name="tx"></param>
        /// <param name="ty"></param>
        /// <param name="bx"></param>
        /// <param name="by"></param>
        public BoundingBox(float x, float y)
        {
            topLeft = new Vector2(-x/2, -y/2);
            botRight = new Vector2(x/2, y/2);

            this.SetRotation(0);
            this.CalculateConstants();
        }

        /// <summary>
        /// Creates a rotated bounding box uniformly around center of entity
        /// </summary>
        /// <param name="tx"></param>
        /// <param name="ty"></param>
        /// <param name="bx"></param>
        /// <param name="by"></param>
        public BoundingBox(float halfX, float halfY, int rotation)
        {
            topLeft = new Vector2(-halfX, -halfY);
            botRight = new Vector2(halfX, halfY);

            this.SetRotation(rotation);
            this.CalculateConstants();
        }

        /// <summary>
        /// Create a rotated bounding box
        /// </summary>
        /// <param name="tx"></param>
        /// <param name="ty"></param>
        /// <param name="bx"></param>
        /// <param name="by"></param>
        /// <param name="rotation"></param>
        public BoundingBox(float tx, float ty, float bx, float by, int rotation)
        {
            topLeft = new Vector2(tx, ty);
            botRight = new Vector2(bx, by);

            this.SetRotation(rotation);
            this.CalculateConstants();
        }

        /// <summary>
        /// Creates bounding box with center at top left.  Useful for menues.
        /// </summary>
        /// <param name="size"></param>
        public BoundingBox(Vector2f size)
        {
            topLeft = new Vector2(0, 0);
            botRight = new Vector2(size.X, size.Y);

            this.SetRotation(0);
            this.CalculateConstants();
        }

        private void CalculateConstants()
        {
            float r1 = topLeft.GetMagnitude();
            float r2 = botRight.GetMagnitude();
            radiusApproximation = Math.Max(r1, r2);
            height = Math.Abs(botRight.y - topLeft.y);
            width = Math.Abs(botRight.x - topLeft.x);
            halfHeight = height / 2;
            halfWidth = width / 2;
        }

        /// <summary>
        /// Sets the rotation of the bounding box, and also rotates an internal representation of the bounding box
        /// Does not destroy the original axis aligned bounding box definition
        /// </summary>
        /// <param name="rotation"></param>
        public void SetRotation(int rotation)
        {
            this.rotation = rotation;
            //Create the missing vectors from the provided vectors
            topRightR = new Vector2(botRight.x, topLeft.y);
            botLeftR = new Vector2(topLeft.x, botRight.y);

            topLeftR = topLeft.Rotate(rotation);
            botRightR = botRight.Rotate(rotation);
            topRightR.VRotate(rotation);
            botLeftR.VRotate(rotation);

            //calculate new normals
            normals[0].x = topRightR.x - topLeftR.x;
            normals[0].y = topRightR.y - topLeftR.y;
            normals[1].x = topRightR.x - botRightR.x;
            normals[1].y = topRightR.y - botRightR.y;
            normals[0].VNormalize();
            normals[1].VNormalize();
        }

        /// <summary>
        /// Get the rotation of bounding box in degrees
        /// </summary>
        /// <returns></returns>
        public int GetRotation()
        {
            return this.rotation;
        }

        /// <summary>
        /// Get the Vectors representing each point of the bounding box
        /// </summary>
        /// <returns></returns>
        public Vector2[] GetVectors()
        {
            return new[] {topLeftR, topRightR, botLeftR, botRightR };
        }

        /// <summary>
        /// Get the point pairs representing the bounding box, clockwise order
        /// </summary>
        /// <returns></returns>
        public float[] GetPoints()
        {
            return new[] { topLeftR.x, topLeftR.y, topRightR.x, topRightR.y, botRightR.x, botRightR.y, botLeftR.x, botLeftR.y };
        }

        /// <summary>
        /// Returns just 2 unit vector2's, each normals for 2 of the sides (first one is top and bottom sides, second is left and right sides)
        /// </summary>
        /// <returns>local xAxis, yAxis</returns>
        public Vector2[] GetNormals()
        {
            return normals;
        }

        /// <summary>
        /// Returns whether a collision between two collision boxes has occurred.  Also returns a pushback vector for bounding box 1 (0 for no collision).
        /// </summary>
        /// <param name="self"></param>
        /// <param name="other"></param>
        /// <param name="posSelf"></param>
        /// <param name="posOther"></param>
        /// <param name="pushBackSelf"></param>
        /// <returns>Truth on whether there was a collision</returns>
        public static bool CheckCollisionWithPushBack(BoundingBox self, BoundingBox other, Vector2 posSelf, Vector2 selfVelocity, Vector2 posOther, out Vector2 pushBackSelf) 
        {
            //Push back vector
            pushBackSelf = new Vector2(0,0);
            #region Circle culling check
            Vector2 d = new Vector2(posOther.x - posSelf.x - selfVelocity.x, posOther.y - posSelf.y - selfVelocity.y);
            if ((self.radiusApproximation + other.radiusApproximation) < d.GetMagnitude())
            {
                return false;
            }
            #endregion Circle culling check

            #region SimpleCollisionCheck (Broken)
            /*
            //2nd check AABB if both have theta = 0
            if (self.rotation == 0 && other.rotation == 0)
            {
                if (posSelf.x + selfVelocity.x < posOther.x + other.width &&
                    posSelf.x + selfVelocity.x + self.width > posOther.x &&
                    posSelf.y + selfVelocity.y < posOther.y + other.height &&
                    posSelf.y + selfVelocity.y + self.height > posOther.y)
                {
                    float overlapY = self.halfHeight + other.halfHeight - Math.Abs(d.y);
                    float overlapX = self.halfWidth + other.halfWidth - Math.Abs(d.x);
                    if (overlapX < overlapY) //push back along X
                    {
                        if (posSelf.x + selfVelocity.x < posOther.x)
                        {
                            overlapX *= -1;
                        }
                        pushBackSelf.x = overlapX;
                    }
                    else //push back along Y
                    {
                        if (posSelf.y + selfVelocity.y < posOther.y)
                        {
                            overlapY *= -1;
                        }
                        pushBackSelf.y = overlapY;
                    }
                    return true;
                }
                else
                {
                    return false;
                }
            }
            */
            #endregion

            #region Separating Axis Theorem check (final and most intensive check for accuracy)
            Vector2[] axis1 = self.GetNormals();
            Vector2[] axis2 = other.GetNormals();
            float overlapAmount = float.MaxValue;
            int index = 0;

            //Checking axis' of box1
            for (int i = 0; i < 2; i++) 
            {
                //Project half vectors onto normal vector
                float sP = Math.Abs(d.Dot(axis1[i]));
                float vP = Math.Abs(self.halfWidth * axis1[0].Dot(axis1[i])) + Math.Abs(self.halfHeight * axis1[1].Dot(axis1[i])) + Math.Abs(other.halfWidth * axis2[0].Dot(axis1[i])) + Math.Abs(other.halfHeight * axis2[1].Dot(axis1[i]));
                if (sP > vP) {
                    return false; //if the projection doesnt overlap then there is no collision
                }
                else if(vP - sP < overlapAmount)
                {
                    overlapAmount = vP - sP;
                    index = i;
                }
            }
            //Checking axis' of box2
            for (int i = 0; i < 2; i++)
            {
                //Project half vectors onto normal vector
                float sP = Math.Abs(d.Dot(axis2[i]));
                float vP = Math.Abs(self.halfWidth* axis1[0].Dot(axis2[i])) + Math.Abs(self.halfHeight * axis1[1].Dot(axis2[i])) + Math.Abs(other.halfWidth * axis2[0].Dot(axis2[i])) + Math.Abs(other.halfHeight * axis2[1].Dot(axis2[i]));
                if (sP > vP) {
                    return false; //if the projection doesnt overlap then there is no collision
                }
                else if (vP - sP < overlapAmount)
                {
                    overlapAmount = vP - sP;
                    index = i + 2;
                }
            }
            //Minimum translation vector calculation
            //TODO: fix.  As of now we have a list of indices with minimum translations.  Want to pick the one that allows sliding
            //Potential solution, if there are two equal overlap amounts, pick the overlap axis with the least effect on the velocity
            //Need to select correct direction when two possible pushback directions
            //1.  The velocity is the going to usually be the same in both the x and y
            //2.  Dotting the velocity onto the current axis will give the same result for both x and y then
            //3.  Cannot do much without exterior info (is there something above/below versus to the left and right, both is already solved)
            //4.  Could pass in a "preferred axis based on neighboring items"
            //5. for tiles, preferred axis is easy
            //6. for entities, it is much harder.
            if (index < 2) //push back along box 1 axis
            {
                pushBackSelf.x += axis1[index].x;
                pushBackSelf.y += axis1[index].y;
            }
            else //push back along box 2 axis
            {
                pushBackSelf.x += axis2[index - 2].x;
                pushBackSelf.y += axis2[index - 2].y;
            }
            pushBackSelf.Scale(overlapAmount);
            //Direction correction logic
            if ((posSelf.y > posOther.y && pushBackSelf.y < 0) || (posSelf.y < posOther.y && pushBackSelf.y > 0))
            {
                pushBackSelf.y *= -1;
            }
            if ((posSelf.x > posOther.x && pushBackSelf.x < 0) || (posSelf.x < posOther.x && pushBackSelf.x > 0))
            {
                pushBackSelf.x *= -1;
            }
            #endregion SAT Check
            return true; //all checks failed, boxes collide
        }

        /// <summary>
        /// Returns whether a collision between two collision boxes has occurred.
        /// </summary>
        /// <param name="self"></param>
        /// <param name="other"></param>
        /// <param name="posSelf"></param>
        /// <param name="posOther"></param>
        /// <returns>Truth on whether there was a collision</returns>
        public static bool CheckCollision(BoundingBox self, BoundingBox other, Vector2 posSelf, Vector2 selfVelocity, Vector2 posOther)
        {

            #region Circle culling check
            Vector2 d = new Vector2(posOther.x - posSelf.x - selfVelocity.x, posOther.y - posSelf.y - selfVelocity.y);
            if ((self.radiusApproximation + other.radiusApproximation) < d.GetMagnitude())
            {
                return false;
            }
            #endregion Circle culling check

            #region SimpleCollisionCheck (Broken)
            /*
            //2nd check AABB if both have theta = 0
            if (self.rotation == 0 && other.rotation == 0)
            {
                if (posSelf.x + selfVelocity.x < posOther.x + other.width &&
                    posSelf.x + selfVelocity.x + self.width > posOther.x &&
                    posSelf.y + selfVelocity.y < posOther.y + other.height &&
                    posSelf.y + selfVelocity.y + self.height > posOther.y)
                {
                    float overlapY = self.halfHeight + other.halfHeight - Math.Abs(d.y);
                    float overlapX = self.halfWidth + other.halfWidth - Math.Abs(d.x);
                    if (overlapX < overlapY) //push back along X
                    {
                        if (posSelf.x + selfVelocity.x < posOther.x)
                        {
                            overlapX *= -1;
                        }
                        pushBackSelf.x = overlapX;
                    }
                    else //push back along Y
                    {
                        if (posSelf.y + selfVelocity.y < posOther.y)
                        {
                            overlapY *= -1;
                        }
                        pushBackSelf.y = overlapY;
                    }
                    return true;
                }
                else
                {
                    return false;
                }
            }
            */
            #endregion

            #region Separating Axis Theorem check (final and most intensive check for accuracy)
            Vector2[] axis1 = self.GetNormals();
            Vector2[] axis2 = other.GetNormals();

            //Checking axis' of box1
            for (int i = 0; i < 2; i++)
            {
                //Project half vectors onto normal vector
                float sP = Math.Abs(d.Dot(axis1[i]));
                float vP = Math.Abs(self.halfWidth * axis1[0].Dot(axis1[i])) + Math.Abs(self.halfHeight * axis1[1].Dot(axis1[i])) + Math.Abs(other.halfWidth * axis2[0].Dot(axis1[i])) + Math.Abs(other.halfHeight * axis2[1].Dot(axis1[i]));
                if (sP > vP)
                {
                    return false; //if the projection doesnt overlap then there is no collision
                }
            }
            //Checking axis' of box2
            for (int i = 0; i < 2; i++)
            {
                //Project half vectors onto normal vector
                float sP = Math.Abs(d.Dot(axis2[i]));
                float vP = Math.Abs(self.halfWidth * axis1[0].Dot(axis2[i])) + Math.Abs(self.halfHeight * axis1[1].Dot(axis2[i])) + Math.Abs(other.halfWidth * axis2[0].Dot(axis2[i])) + Math.Abs(other.halfHeight * axis2[1].Dot(axis2[i]));
                if (sP > vP)
                {
                    return false; //if the projection doesnt overlap then there is no collision
                }
            }
            //No need for push back calculations.  Yay :)
            #endregion SAT Check
            return true; //all checks failed, boxes collide
        }

        /// <summary>
        /// Checks a x,y point for collision with a bounding box at a position.  Specifically for menues (uses graphics vectors)
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="box"></param>
        /// <param name="pos"></param>
        /// <returns></returns>
        public static bool CheckPointMenuCollision(float x, float y, BoundingBox box, Vector2f pos)
        {
            if(x <= pos.X + box.width && x >= pos.X &&
               y <= pos.Y + box.height && y >= pos.Y)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Gets the chunks the entity is possibly present in chunk indices
        /// </summary>
        /// <param name="self"></param>
        /// <param name="selfPos"></param>
        /// <returns></returns>
        public static int[] GetChunkBounds(BoundingBox self, Vector2 posSelf, SurfaceContainer surface)
        {
            float xMin = posSelf.x - self.radiusApproximation - Props.tileCollisionFactor * Props.tileSize;
            float xMax = posSelf.x + self.radiusApproximation + Props.tileCollisionFactor * Props.tileSize;
            float yMin = posSelf.y - self.radiusApproximation - Props.tileCollisionFactor * Props.tileSize;
            float yMax = posSelf.y + self.radiusApproximation + Props.tileCollisionFactor * Props.tileSize;
            int[] top = surface.WorldToChunkCoords(xMin, yMin);
            int[] bot = surface.WorldToChunkCoords(xMax, yMax);
            int yRange = (bot[1] - top[1]) + 1;
            int xRange = (bot[0] - top[0]) + 1;
            int[] ret = new int[xRange * yRange];
            int k = 0;
            for (int i = top[0]; i <= bot[0]; i++)
            {
                for (int j = top[1]; j <= bot[1]; j++)
                {
                    ret[k] = i * surface.worldSize + j;
                    k++;
                }
            }
            return ret;

            #region more complex chunk bound code
            /*if(self.rotation == 0)
            {
                int[] top = SurfaceContainer.WorldToChunkCoords(selfPos.x + self.topLeftR.x, selfPos.y + self.topLeftR.y);
                int[] bot = SurfaceContainer.WorldToChunkCoords(selfPos.x + self.botLeftR.y, selfPos.y + self.botLeftR.y);
                int yRange = (bot[1] - top[1]);
                int xRange = (bot[0] - top[0]);
                int[] ret = new int[xRange * yRange];
                int k = 0;
                for(int i = top[0]; i <= bot[0]; i++)
                {
                    for(int j = top[1]; j <= bot[1]; j++)
                    {
                        ret[k] = i * Props.worldSize + j;
                        k++;
                    }
                }
                return ret;
            }
            int[][] coords = new int[4][];
            coords[0] = SurfaceContainer.WorldToChunkCoords(selfPos.VAdd(self.topLeftR));
            coords[1] = SurfaceContainer.WorldToChunkCoords(selfPos.VAdd(self.topRightR));
            coords[2] = SurfaceContainer.WorldToChunkCoords(selfPos.VAdd(self.botLeftR));
            coords[3] = SurfaceContainer.WorldToChunkCoords(selfPos.VAdd(self.botRightR));
            int[] min = new int[] { coords[0][0], coords[0][1] };
            int[] max = new int[] { coords[3][0], coords[3][1] };
            for (int i = 0; i < 4; i++)
            {
                if(coords[i][0] <= min[0])
                {
                    min[0] = coords[i][0]; 
                }
                if(coords[i][0] >= max[0])
                {
                    max[0] = coords[i][0];
                }
                if(coords[i][1] <= min[1])
                {
                    min[1] = coords[i][1];
                }
                if(coords[i][1] >= max[1])
                {
                    max[1] = coords[i][1];
                }
            }*/
            #endregion more complex chunk bound code
        }

        /// <summary>
        /// Gets the tiles the entity could possibly occupy in tile indices
        /// </summary>
        /// <param name="self"></param>
        /// <param name="posSelf"></param>
        /// <returns></returns>
        public static int[][] GetTileBounds(BoundingBox self, Vector2 posSelf)
        {
            float xMin = posSelf.x - self.radiusApproximation - Props.tileCollisionFactor * Props.tileSize;
            float xMax = posSelf.x + self.radiusApproximation + Props.tileCollisionFactor * Props.tileSize;
            float yMin = posSelf.y - self.radiusApproximation - Props.tileCollisionFactor * Props.tileSize;
            float yMax = posSelf.y + self.radiusApproximation + Props.tileCollisionFactor * Props.tileSize;
            int[] top = SurfaceContainer.WorldToTileCoords(xMin, yMin);
            int[] bot = SurfaceContainer.WorldToTileCoords(xMax, yMax);
            int yRange = (bot[1] - top[1]) + 1;
            int xRange = (bot[0] - top[0]) + 1;
            int[][] ret = new int[xRange * yRange][];
            int x = 0;
            for (int i = top[0]; i <= bot[0]; i++)
            {
                for (int j = top[1]; j <= bot[1]; j++)
                {
                    int tileXmin = i == top[0] ? top[2] : 0;
                    int tileXmax = i == bot[0] ? bot[2] + 1 : Props.chunkSize;
                    int tileYmin = j == top[1] ? top[3] : 0;
                    int tileYmax = j == bot[1] ? bot[3] + 1 : Props.chunkSize;
                    ret[x] = new int[(tileXmax - tileXmin) * (tileYmax - tileYmin)];
                    int y = 0;
                    for(int k = tileXmin; k < tileXmax; k++)
                    {
                        for(int l = tileYmin; l < tileYmax; l++)
                        {
                            ret[x][y] = k * Props.chunkSize + l;
                            y++;
                        }
                    }
                    x++;
                }
            }
            return ret;
        }

        /// <summary>
        /// Standard collision check for moving objects
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="velocity"></param>
        public static void ApplyPhysicalCollision(Entity entity, Vector2 totalVelocity)
        {
            if ((entity.collisionMask & Base.CollisionLayer.TerrainSolid) != 0)
            {
                float speedModifier = entity.surface.tileCollection.GetTerrainTile(entity.surface.GetTileFromWorld(entity.position)).frictionModifier;
                totalVelocity.Scale(speedModifier);
            }
            int intervals = (int)Math.Ceiling(totalVelocity.GetMagnitude() / Props.maxVelocity);
            Vector2 scaledVelocity = totalVelocity.Copy();
            scaledVelocity.Scale(1.0f / intervals);
            for (int splits = 0; splits < intervals; splits++)
            {
                Vector2 velocity = scaledVelocity.Copy();
                Vector2 newEntityPos = new Vector2(entity.position.x + velocity.x, entity.position.y + velocity.y);
                int[] chunkList = BoundingBox.GetChunkBounds(entity.collisionBox, newEntityPos, entity.surface);
                int[][] tileList = BoundingBox.GetTileBounds(entity.collisionBox, newEntityPos);

                //Checks collisions with entities and tiles
                for (int i = 0; i < chunkList.Length; i++)
                {
                    Chunk chunk = entity.surface.GetChunk(chunkList[i], false);
                    Vector2 pushBack;

                    //entity collision checks
                    List<Entity> collisionList = chunk.entityCollisionList;
                    for (int j = 0; j < collisionList.Count; j++)
                    {
                        if ((collisionList[j].collisionMask & entity.collisionMask) != 0 && !collisionList[j].Equals(entity))
                        {

                            if (BoundingBox.CheckCollisionWithPushBack(entity.collisionBox, collisionList[j].collisionBox, entity.position, velocity, collisionList[j].position, out pushBack))
                            {
                                velocity.Add(pushBack);
                            }
                        }
                    }

                    //tile collision checks
                    //Perhaps switch to continually checking whether the player is colliding with a tile at his position until he isnt colliding with a tile?
                    //Would fix the situation where getting stuck in water still allows movement within
                    //TODO: try solution outline above
                    for (int j = 0; j < tileList[i].Length; j++)
                    {
                        Tile tile = entity.surface.tileCollection.GetTerrainTile(chunk.GetTile(tileList[i][j]));
                        if ((entity.collisionMask & tile.collisionMask) != 0)
                        {
                            Vector2 tilePos = entity.surface.WorldToTileVector(chunkList[i], tileList[i][j]);
                            if (BoundingBox.CheckCollisionWithPushBack(entity.collisionBox, entity.surface.tileBox, entity.position, velocity, tilePos, out pushBack))
                            {
                                velocity.Add(pushBack);
                            }
                        }
                    }
                }
                entity.position.Add(velocity);
            }
            //TODO: need to add total failure check, where the end result of the overall collision ends up inside of a colliding body, and in such a case set velocity to 0
            
            entity.surface.UpdateEntityInChunks(entity);
        }

        /// <summary>
        /// Simply checks whether there was a collision for an entity, based on its collision masks
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public static bool CheckForCollision(Entity entity)
        {
            int[] chunkList = BoundingBox.GetChunkBounds(entity.collisionBox, entity.position, entity.surface);
            int[][] tileList = BoundingBox.GetTileBounds(entity.collisionBox, entity.position);
            for (int i = 0; i < chunkList.Length; i++)
            {
                Chunk chunk = entity.surface.GetChunk(chunkList[i], false);
                //entity collision checks
                List<Entity> collisionList = chunk.entityCollisionList;
                for (int j = 0; j < collisionList.Count; j++)
                {
                    if ((collisionList[j].collisionMask & entity.collisionMask) != 0 && !ReferenceEquals(collisionList[j],entity))
                    {
                        if (BoundingBox.CheckCollision(entity.collisionBox, collisionList[j].collisionBox, entity.position, new Vector2(0,0), collisionList[j].position))
                        {
                            return true;
                        }
                    }
                }

                //tile collision checks
                //Perhaps switch to continually checking whether the player is colliding with a tile at his position until he isnt colliding with a tile?
                //Would fix the situation where getting stuck in water still allows movement within
                //TODO: try solution outline above
                for (int j = 0; j < tileList[i].Length; j++)
                {
                    Tile tile = entity.surface.tileCollection.GetTerrainTile(chunk.GetTile(tileList[i][j]));
                    if ((entity.collisionMask & tile.collisionMask) != 0)
                    {
                        Vector2 tilePos = entity.surface.WorldToTileVector(chunkList[i], tileList[i][j]);
                        if (BoundingBox.CheckCollision(entity.collisionBox, entity.surface.tileBox, entity.position, new Vector2(0,0), tilePos))
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Gets full list of all entities colliding with entity.
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public static bool GetCollisionList(Entity entity, out List<Entity> entities, out List<int> tileIndices)
        {
            //lists to keep track of collided entities
            entities = new List<Entity>();
            tileIndices = new List<int>();
            //list of chunks where collisions may have happened + tiles
            int[] chunkList = BoundingBox.GetChunkBounds(entity.collisionBox, entity.position, entity.surface);
            int[][] tileList = BoundingBox.GetTileBounds(entity.collisionBox, entity.position);
            for (int i = 0; i < chunkList.Length; i++)
            {
                Chunk chunk = entity.surface.GetChunk(chunkList[i], false);
                //entity collision checks
                List<Entity> collisionList = chunk.entityCollisionList;
                for (int j = 0; j < collisionList.Count; j++)
                {
                    if ((collisionList[j].collisionMask & entity.collisionMask) != 0 && !ReferenceEquals(collisionList[j], entity))
                    {
                        if (BoundingBox.CheckCollision(entity.collisionBox, collisionList[j].collisionBox, entity.position, new Vector2(0, 0), collisionList[j].position))
                        {
                            entities.Add(collisionList[j]);
                        }
                    }
                }

                //tile collision checks
                for (int j = 0; j < tileList[i].Length; j++)
                {
                    Tile tile = entity.surface.tileCollection.GetTerrainTile(chunk.GetTile(tileList[i][j]));
                    if ((entity.collisionMask & tile.collisionMask) != 0)
                    {
                        Vector2 tilePos = entity.surface.WorldToTileVector(chunkList[i], tileList[i][j]);
                        if (BoundingBox.CheckCollision(entity.collisionBox, entity.surface.tileBox, entity.position, new Vector2(0, 0), tilePos))
                        {
                            tileIndices.Add(tileList[i][j]);
                        }
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Only checks against entities with viable collision mask.  Returns list of cast entities.
        /// </summary>
        /// <typeparam name="TypeToTest"></typeparam>
        /// <param name="entity"></param>
        /// <param name="check"></param>
        /// <returns></returns>
        public static List<TypeToTest> GetCollisionListOfType<TypeToTest>(Entity entity) where TypeToTest: Entity
        {
            //lists to keep track of collided entities
            List<TypeToTest> list = new List<TypeToTest>();
            //list of chunks where collisions may have happened + tiles
            int[] chunkList = BoundingBox.GetChunkBounds(entity.collisionBox, entity.position, entity.surface);
            int[][] tileList = BoundingBox.GetTileBounds(entity.collisionBox, entity.position);
            for (int i = 0; i < chunkList.Length; i++)
            {
                Chunk chunk = entity.surface.GetChunk(chunkList[i], false);
                //entity collision checks
                List<Entity> collisionList = chunk.entityCollisionList;
                for (int j = 0; j < collisionList.Count; j++)
                {
                    TypeToTest checkEntity = collisionList[j] as TypeToTest;
                    if (checkEntity != null && (collisionList[j].collisionMask & entity.collisionMask) != 0 && !ReferenceEquals(collisionList[j], entity) &&
                        BoundingBox.CheckCollision(entity.collisionBox, collisionList[j].collisionBox, entity.position, new Vector2(0, 0), collisionList[j].position))
                    {
                        list.Add(checkEntity);
                    }
                }
            }
            return list;
        }
    }
}