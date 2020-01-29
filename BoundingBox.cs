using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EngineeringCorpsCS
{
    class BoundingBox
    {
        float[] boundBox;
        int rotation;
        BoundingBox(float tx, float ty, float bx, float by)
        {
            boundBox = new[] { tx,ty,bx,by};
        }
        BoundingBox(float tx, float ty, float bx, float by, int rotation)
        {
            boundBox = new[] { tx, ty, bx, by };
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

        public float[] GetBox()
        {
            return boundBox;
        }

        
        public static bool CheckCollision(BoundingBox box1, BoundingBox box2, Vector2 p1, Vector2 p2) 
        {

            float r1 = (float)Math.Sqrt((x2 - x1) * (x2 - x1) + (y2 - y1) * (y2 - y1));
            float r2 = (float)Math.Sqrt((s2 - s1) * (s2 - s1) + (w2 - w1) * (w2 - w1));
            float d = (float)Math.Sqrt((p2x - p1x) * (p2x - p1x) + (p2y - p1y) * (p2y - p1y)); // calc d between two objects
            if (r1 + r2 > d)
            {
                return false; //need to determine if other things need to be returned maybe
            }
            //1st check (circle check)
            //2nd check AABB if both have theta = 0
            if (rect1.x < rect2.x + rect2.width &&
               rect1.x + rect1.width > rect2.x &&
               rect1.y < rect2.y + rect2.height &&
               rect1.y + rect1.height > rect2.y) {
            }
        }
        
    }
}
