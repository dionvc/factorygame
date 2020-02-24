using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EngineeringCorpsCS
{
    class BoundingBox
    {
        public static ChunkManager chunkManager { get;  set; }
        public static TileCollection tileCollection{ get; set; }
        Vector2 topLeft { get; set; }
        Vector2 botRight { get; set; }
        int rotation;
        Vector2 topLeftR;
        Vector2 topRightR;
        Vector2 botLeftR;
        Vector2 botRightR;

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

            //Rotated vectors with 0 rotation
            topLeftR = topLeft.Copy();
            botRightR = botRight.Copy();
            topRightR = new Vector2(botRight.x, topLeft.y);
            botLeftR = new Vector2(topLeft.x, botRight.y);
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

            //Rotated vectors with 0 rotation
            topLeftR = topLeft.Copy();
            botRightR = botRight.Copy();
            topRightR = new Vector2(botRight.x, topLeft.y);
            botLeftR = new Vector2(topLeft.x, botRight.y);
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
        /// Gets width of original box (corresponding to local x-axis)
        /// </summary>
        /// <returns></returns>
        public float GetWidth()
        {
            return Math.Abs(botRight.x - topLeft.x);
        }
        public float GetHalfWidth()
        {
            return (Math.Abs(botRight.x - topLeft.x)/2);
        }
        /// <summary>
        /// Gets height of original box (corresponding ot local y-axis)
        /// </summary>
        /// <returns></returns>
        public float GetHeight()
        {
            return Math.Abs(botRight.y - topLeft.y);
        }
        public float GetHalfHeight()
        {
            return (Math.Abs(botRight.y - topLeft.y)/2);
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
            Vector2 xAxis = new Vector2(topRightR.x - topLeftR.x, topRightR.y - topLeftR.y);
            Vector2 yAxis = new Vector2(topRightR.x - botRightR.x, topRightR.y - botRightR.y);
            xAxis.VNormalize();
            yAxis.VNormalize();
            return new[] { xAxis, yAxis };
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
            float r1 = self.topLeft.GetMagnitude();
            float r2 = other.topLeft.GetMagnitude();
            float r3 = self.botRight.GetMagnitude();
            float r4 = other.botRight.GetMagnitude();
            Vector2 d = posOther.Copy();
            d.Subtract(posSelf);
            d.Subtract(selfVelocity);
            if ((Math.Max(r1,r3) + Math.Max(r2,r4)) < d.GetMagnitude())
            {
                return false;
            }
            //2nd check AABB if both have theta = 0
            if(self.rotation == 0 && other.rotation == 0)
            {
                float halfW1 = self.GetHalfWidth();
                float halfH1 = self.GetHalfHeight();
                float halfW2 = other.GetHalfWidth();
                float halfH2 = other.GetHalfHeight();
                if (posSelf.x + selfVelocity.x < posOther.x + (2*halfW2) &&
                    posSelf.x + selfVelocity.x + (2*halfW1) > posOther.x &&
                    posSelf.y + selfVelocity.y < posOther.y + (2*halfH2) &&
                    posSelf.y + selfVelocity.y + (2*halfH1) > posOther.y)
                {

                    float overlapY = self.GetHalfHeight() + other.GetHalfHeight() - Math.Abs(d.y);
                    float overlapX = self.GetHalfWidth() + other.GetHalfWidth() - Math.Abs(d.x);
                    Console.WriteLine(overlapX + " : " + overlapY);
                    if(overlapX < overlapY && overlapX > 0) //push back along X
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
                float halfW1 = self.GetHalfWidth();
                float halfH1 = self.GetHalfHeight();
                float halfW2 = other.GetHalfWidth();
                float halfH2 = other.GetHalfHeight();
                List<float> overlapAmount = new List<float>() { 0, 0, 0, 0 };
                //Separating Axis Theorem check (final and most intensive check for accuracy)

                //Checking axis' of box1
                for (int i = 0; i < 2; i++) 
                {
                    //Project half vectors onto normal vector
                    float sP = Math.Abs(d.Dot(axis1[i]));
                    float vP = Math.Abs(halfW1 * axis1[0].Dot(axis1[i])) + Math.Abs(halfH1 * axis1[1].Dot(axis1[i])) + Math.Abs(halfW2 * axis2[0].Dot(axis1[i])) + Math.Abs(halfH2 * axis2[1].Dot(axis1[i]));
                    if (sP > vP) {
                        return false; //if the projection doesnt overlap then there is no collision
                    }
                    else
                    {
                        overlapAmount[i] = vP - sP;
                    }
                }
                //Checking axis' of box2
                for (int i = 0; i < 2; i++)
                {
                    //Project half vectors onto normal vector
                    float sP = Math.Abs(d.Dot(axis2[i]));
                    float vP = Math.Abs(halfW1 * axis1[0].Dot(axis2[i])) + Math.Abs(halfH1 * axis1[1].Dot(axis2[i])) + Math.Abs(halfW2 * axis2[0].Dot(axis2[i])) + Math.Abs(halfH2 * axis2[1].Dot(axis2[i]));
                    if (sP > vP) {
                        return false; //if the projection doesnt overlap then there is no collision
                    }
                    else
                    {
                        overlapAmount[i + 2] = vP - sP;
                    }
                }
                //Minimum translation vector calculation
                float minOverlap = Math.Abs(overlapAmount.Min());
                int index = overlapAmount.IndexOf(minOverlap);
                if (index < 2) //push back along box 1 axis
                {
                    axis1[index].Scale(minOverlap);
                    pushBackSelf = axis1[index];
                }
                else //push back along box 2 axis
                {
                    axis2[index - 2].Scale(minOverlap);
                    pushBackSelf = axis2[index - 2];
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
        /// Checks for collision between character and tiles
        /// </summary>
        /// <param name="box"></param>
        /// <param name="pos"></param>
        /// <returns></returns>
        public static bool CheckTileCollision(BoundingBox box, Vector2 pos, Base.CollisionLayer collisionMask)
        {
            if((tileCollection.GetTerrainTile(chunkManager.GetTileFromWorld(pos.x, pos.y)).collisionMask & collisionMask) != 0) {
                return true;
            }
            return false;
        }
        
    }
}