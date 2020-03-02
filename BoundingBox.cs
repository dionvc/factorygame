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
        public BoundingBox(float halfX, float halfY)
        {
            topLeft = new Vector2(-halfX, -halfY);
            botRight = new Vector2(halfX, halfY);

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
        public static bool CheckCollision(BoundingBox self, BoundingBox other, Vector2 posSelf, Vector2 selfVelocity, Vector2 posOther, out Vector2 pushBackSelf) 
        {
            //1st check (circle check)
            pushBackSelf = new Vector2(0,0);
            Vector2 d = new Vector2(posOther.x - posSelf.x - selfVelocity.x, posOther.y - posSelf.y - selfVelocity.y);
            if ((self.radiusApproximation + other.radiusApproximation) < d.GetMagnitude())
            {
                return false;
            }
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
                    if(overlapX < overlapY) //push back along X
                    {
                        if(posSelf.x < posOther.x)
                        {
                            overlapX *= -1;
                        }
                        pushBackSelf.x = overlapX;
                    }
                    else //push back along Y
                    {
                        if(posSelf.y < posOther.y)
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
            else
            {
                Vector2[] axis1 = self.GetNormals();
                Vector2[] axis2 = other.GetNormals();
                float overlapAmount = float.MaxValue;
                int index = 0;
                //Separating Axis Theorem check (final and most intensive check for accuracy)

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
                    else if(vP - sP < overlapAmount)
                    {
                        overlapAmount = vP - sP;
                        index = i + 2;
                    }
                }
                //Minimum translation vector calculation
                
                if (index < 2) //push back along box 1 axis
                {
                    pushBackSelf.x = axis1[index].x;
                    pushBackSelf.y = axis1[index].y;
                    pushBackSelf.Scale(overlapAmount);
                }
                else //push back along box 2 axis
                {
                    pushBackSelf.x = axis2[index - 2].x;
                    pushBackSelf.y = axis2[index - 2].y;
                    pushBackSelf.Scale(overlapAmount);
                }
                //Direction correction logic
                if((posSelf.x < posOther.x && pushBackSelf.x > 0) || (posSelf.x > posOther.x && pushBackSelf.x < 0))
                {
                    pushBackSelf.x *= -1;
                }
                if((posSelf.y > posOther.y && pushBackSelf.y < 0) || (posSelf.y < posOther.y && pushBackSelf.y > 0))
                {
                    pushBackSelf.y *= -1;
                }
                
                return true; //all checks failed, boxes collide
            }
        }
        /// <summary>
        /// Checks for collision between character and tiles, also returns the tiletype collided with
        /// </summary>
        /// <param name="box"></param>
        /// <param name="pos"></param>
        /// <returns></returns>
        public static bool CheckTerrainTileCollision(BoundingBox box, Vector2 pos, Vector2 velocity, Base.CollisionLayer collisionMask, TileCollection tileCollection, SurfaceContainer chunkManager, out Tile tile)
        {
            tile = tileCollection.GetTerrainTile(chunkManager.GetTileFromWorld(pos.x, pos.y));
            if ((tile.collisionMask & collisionMask) != 0) {
                return true;
            }
            return false;
        }
        
        public static bool CheckPointMenuCollision(float x, float y, BoundingBox box, Vector2f pos)
        {
            if(x <= pos.X + box.width && x >= pos.X &&
               y <= pos.Y + box.height && y >= pos.Y)
            {
                return true;
            }
            return false;
        }

        public static int[] GetChunkBounds(BoundingBox self, Vector2 selfPos)
        {
            float xMin = selfPos.x - self.radiusApproximation;
            float xMax = selfPos.x + self.radiusApproximation;
            float yMin = selfPos.y - self.radiusApproximation;
            float yMax = selfPos.y + self.radiusApproximation;
            int[] top = SurfaceContainer.WorldToChunkCoords(xMin, yMin);
            int[] bot = SurfaceContainer.WorldToChunkCoords(xMax, yMax);
            int yRange = (bot[1] - top[1]);
            int xRange = (bot[0] - top[0]);
            int[] ret = new int[xRange * yRange];
            int k = 0;
            for (int i = top[0]; i <= bot[0]; i++)
            {
                for (int j = top[1]; j <= bot[1]; j++)
                {
                    ret[k] = i * Props.worldSize + j;
                    k++;
                }
            }
            return ret;

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
        }
    }
}