using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFML.Graphics;
using SFML.System;

namespace EngineeringCorpsCS
{
    abstract class Drawable
    {
        public enum DrawLayer
        {
            None = 0,
            Terrain = 1,            //All terrain is drawn on one layer
            TerrainDecor = 2,       //Terrain decor is drawn before paths
            TerrainPath = 3,        //Placed paths
            Resource = 4,           //Some resources may have visible signs above ground
            EntityUnderItem = 5,
            Item = 6,
            EntitySorted = 7,
            Overlay = 8,
            Air = 8,
            IconOverlay = 9
        }

        public Vector2f drawOffset { get; set; }

        public abstract void SetRotation(float rotation);
        public abstract void SetScale(float x, float y);
        public abstract void SetColor(byte r, byte g, byte b, byte a);
        public abstract Sprite GetSprite();
        public abstract void Update();
        public abstract void SetAnimationSpeed(float animationSpeed);
        public abstract void SetBehavior(string behavior);
    }
}
