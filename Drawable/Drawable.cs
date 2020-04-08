﻿using System;
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
            None = 0,               //Object not drawn
            TerrainDecor = 1,       //Terrain decor is drawn before paths
            TerrainPath = 2,        //Placed paths
            Resource = 3,           //Some resources may have visible signs above ground
            EntityUnderItem = 4,    //Some entities may want to appear below an item
            Item = 5,               //just the entities
            EntitySorted = 6,       //Regular entity layer
            Air = 7,                //Stuff that is in the air
            IconOverlay = 8         //Would be useful for player hints
        }

        public Vector2f drawOffset { get; set; }

        public abstract void SetRotation(float rotation);
        public abstract void SetScale(float x, float y);
        public abstract void SetColor(byte r, byte g, byte b, byte a);
        public abstract Sprite GetSprite();
        public abstract void Update();
        public abstract void SetAnimationSpeed(float animationSpeed);
    }
}
