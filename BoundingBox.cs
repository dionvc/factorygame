using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EngineeringCorpsCS
{
    class BoundingBox
    {
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

        
        public static bool CheckCollision(BoundingBox box1, BoundingBox box2, Vector2 p1, Vector2 p2) 
        {
            //1st check (circle check)
            float r1 = box1.topLeft.GetMagnitude();
            float r2 = box2.topLeft.GetMagnitude();
            float r3 = box1.botRight.GetMagnitude();
            float r4 = box2.botRight.GetMagnitude();
            float d = (float)Math.Sqrt((p2.x - p1.x) * (p2.x - p1.x) + (p2.y - p1.y) * (p2.y - p1.y)); // calc d between two objects
            if (Math.Max(r1,r3) + Math.Max(r2,r4) < d)
            {
                return false; //need to determine if other things need to be returned maybe
            }
            //2nd check AABB if both have theta = 0
            if(box1.rotation == 0 && box2.rotation == 0)
            {
                if (p1.x + box1.topLeft.x > p2.x + box2.botRight.x || p1.x + box1.botRight.x > p2.x + box2.topLeft.x)
                {
                    return false;
                }
                else if (p1.y + box1.topLeft.y < p2.y + box2.botRight.y || p1.y + box1.botRight.y < p2.y + box2.topLeft.y)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            else
            {
                Vector2[] axis1 = box1.GetNormals();
                Vector2[] axis2 = box2.GetNormals();
                float halfW1 = box1.GetHalfWidth();
                float halfH1 = box1.GetHalfHeight();
                float halfW2 = box2.GetHalfWidth();
                float halfH2 = box2.GetHalfHeight();
                Vector2 T = p2.Copy();
                T.Subtract(p1);
                //Separating Axis Theorem check (final and most intensive check for accuracy)

                //Checking axis' of box1
                for (int i = 0; i < 2; i++) 
                {
                    //Project half vectors onto normal vector
                    if (Math.Abs(T.Dot(axis1[i])) > Math.Abs(halfW1 * axis1[0].Dot(axis1[i])) + Math.Abs(halfH1 * axis1[1].Dot(axis1[i])) + Math.Abs(halfW2 * axis2[0].Dot(axis1[i])) + Math.Abs(halfH2 * axis2[1].Dot(axis1[i]))) {
                        return false; //if the projection doesnt overlap then there is no collision
                    }
                }
                //Checking axis' of box2
                for (int i = 0; i < 2; i++)
                {
                    //Project half vectors onto normal vector
                    if (Math.Abs(T.Dot(axis2[i])) > Math.Abs(halfW1 * axis1[0].Dot(axis2[i])) + Math.Abs(halfH1 * axis1[1].Dot(axis2[i])) + Math.Abs(halfW2 * axis2[0].Dot(axis2[i])) + Math.Abs(halfH2 * axis2[1].Dot(axis2[i]))) {
                        return false; //if the projection doesnt overlap then there is no collision
                    }
                }
                return true; //all checks failed, boxes collide
            }
        }
        
    }
}