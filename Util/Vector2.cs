﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFML.System;

namespace EngineeringCorpsCS
{
    class Vector2
    {
        public float x { get; set; }
        public float y { get; set; }
        Vector2f _internalVector;
        public Vector2f internalVector { get
            {
                _internalVector.X = this.x;
                _internalVector.Y = this.y;
                return _internalVector;
            } protected set
            {
                _internalVector = value;
            } }
        public Vector2(float x, float y)
        {
            this.x = x;
            this.y = y;
            internalVector = new Vector2f(x, y);
        }

        public Vector2(float radius, int angle, bool placeHolder)
        {
            this.x = radius * (float)Math.Cos(angle);
            this.y = radius * (float)Math.Sin(angle);
        }

        public Vector2(Vector2f pos)
        {
            internalVector = pos;
            this.x = pos.X;
            this.y = pos.Y;
        }

        /// <summary>
        /// Sets the vector2 to some quantity.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public void Set(float x, float y)
        {
            this.x = x;
            this.y = y;
        }

        public static Vector2 operator +(Vector2 a, Vector2 b) {
            return new Vector2(a.x + b.x, a.y + b.y);
        }

        public static Vector2 operator -(Vector2 a, Vector2 b)
        {
            return new Vector2(a.x - b.x, a.y - b.y);
        }

        /// <summary>
        /// Permanently adds a Vector2 to the calling Vector2
        /// </summary>
        /// <param name="other"></param>
        public Vector2 Add(Vector2 other)
        {
            this.x += other.x;
            this.y += other.y;
            return this;
        }
        
        /// <summary>
        /// Permanently adds desired x and y components to a Vector2
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public void Add(float x, float y)
        {
            this.x += x;
            this.y += y;
        }

        /// <summary>
        /// Returns a new Vector2 representing the addition
        /// </summary>
        /// <param name="other"></param>
        public Vector2 VAdd(Vector2 other)
        {
            return new Vector2(this.x + other.x, this.y + other.y);
        }

        /// <summary>
        /// Permanently subtracts desired Vector2 from a Vector2
        /// </summary>
        /// <param name="other"></param>
        public void Subtract(Vector2 other)
        {
            this.x -= other.x;
            this.y -= other.y;
        }

        /// <summary>
        /// Permanently subtracts desired x and y components from a Vector2
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public void Subtract(float x, float y)
        {
            this.x -= x;
            this.y -= y;
        }

        public void Scale(float s)
        {
            this.x *= s;
            this.y *= s;
        }

        public void Scale(float sx, float sy)
        {
            this.x *= sx;
            this.y *= sy;
        }
        /// <summary>
        /// Permanently rotates a Vector2 by a number of degrees around the origin
        /// </summary>
        /// <param name="rotation"></param>
        public void VRotate(float rotation)
        {
            if(rotation == 0)
            {
                return;
            }
            rotation = (rotation * (float)Math.PI) / 180;
            float tempX = this.x;
            float tempY = this.y;
            this.x = (float) ( tempX * Math.Cos(rotation) - tempY * Math.Sin(rotation) );
            this.y = (float) ( tempX * Math.Sin(rotation) + tempY * Math.Cos(rotation) );
        }

        /// <summary>
        /// Returns a new Vector2 representing a rotated version of the old Vector2
        /// </summary>
        /// <param name="rotation"></param>
        /// <returns></returns>
        public Vector2 Rotate(float rotation)
        {
            if(rotation == 0)
            {
                return new Vector2(this.x, this.y);
            }
            rotation = (rotation * (float)Math.PI) / 180;
            float tempX = this.x;
            float tempY = this.y;
            return new Vector2((float)(tempX * Math.Cos(rotation) - tempY * Math.Sin(rotation)), (float)(tempX * Math.Sin(rotation) + tempY * Math.Cos(rotation)));
        }

        /// <summary>
        /// Returns the magnitude of the Vector2
        /// </summary>
        /// <returns></returns>
        public float GetMagnitude()
        {
            return (float) Math.Sqrt((this.x * this.x) + (this.y * this.y));
        }

        public float GetDistance(Vector2 other)
        {
            float xDiff = this.x - other.x;
            float yDiff = this.y - other.y;
            return (float)Math.Sqrt((xDiff * xDiff) + (yDiff * yDiff));

        }

        /// <summary>
        /// Permanently normalize the instance vector2 to a unit vector
        /// </summary>
        public void VNormalize()
        {
            float mag = this.GetMagnitude();
            this.x = this.x / mag;
            this.y = this.y / mag;
        }

        /// <summary>
        /// Calculates the angle between the x axis and the Vector2
        /// </summary>
        /// <returns></returns>
        public float GetRotation()
        {
            float angle = (float) Math.Atan2(this.y, this.x);
            angle = angle * 180 / (float)Math.PI;
            return (angle + 360.0f) % 360.0f;
        }

        /// <summary>
        /// Dots the instance Vector2 with the argument Vector2
        /// </summary>
        /// <param name="projectOnVector"></param>
        /// <returns></returns>
        public float Dot(Vector2 projectOnVector)
        {
            return this.x * projectOnVector.x + this.y * projectOnVector.y;
        }

        public Vector2 Copy()
        {
            return new Vector2(this.x, this.y);
        }
    }
}
