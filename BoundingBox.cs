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
        BoundingBox(float tx, float ty, float bx, float by)
        {
            topLeft = new Vector2(tx, ty);
            botRight = new Vector2(bx, by);
        }
        BoundingBox(float tx, float ty, float bx, float by, int rotation)
        {
            topLeft = new Vector2(tx, ty);
            botRight = new Vector2(bx, by);
            this.rotation = rotation;
        }
        public void SetRotation(int rotation)
        {
            this.rotation = rotation;
        }
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

        public Vector2[] GetPoints()
        {
            Vector2 topRight = new Vector2(-topLeft.x, topLeft.y);
            Vector2 botLeft = new Vector2(-botRight.x, botRight.y);
            return new[] {topLeft.Rotate(rotation), topRight.Rotate(rotation), botLeft.Rotate(rotation), botRight.Rotate(rotation) };
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
            if(box1.rotation != 0 || box2.rotation != 0)
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
                //Separating Axis Theorem check
            }
        }
        
    }
}
