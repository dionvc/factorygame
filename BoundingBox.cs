using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EngineeringCorpsCS
{
    class BoundingBox
    {
        Vector2 topLeft;
        Vector2 botRight;
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

        public float GetWidth()
        {
            return Math.Abs(botRight.x - topLeft.x);
        }
        public float GetHeight()
        {
            return Math.Abs(botRight.y - topLeft.y);
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

        
        public static bool CheckCollision(BoundingBox box1, BoundingBox box2, Vector2 p1, Vector2 p2) 
        {
            float r1 = box1.topLeft.GetMagnitude();
            float r2 = box2.topLeft.GetMagnitude();
            float d = (float)Math.Sqrt((p2.x - p1.x) * (p2.x - p1.x) + (p2.y - p1.y) * (p2.y - p1.y)); // calc d between two objects
            if (r1 + r2 > d)
            {
                return false; //need to determine if other things need to be returned maybe
            }
            //1st check (circle check)
            //2nd check AABB if both have theta = 0
            if(box1.rotation != 0 && box2.rotation != 0)
            {
                if (box1.topLeft.x < box2.topLeft.x + box2.botRight.x &&
                   box1.topLeft.x + box1.botRight.x > box2.topLeft.x &&
                   box1.topLeft.y < box2.topLeft.y + box2.botRight.y &&
                   box1.topLeft.y + box1.botRight.y > box2.topLeft.y)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return true;
                //Separating Axis Theorem check
            }
        }
        
    }
}
