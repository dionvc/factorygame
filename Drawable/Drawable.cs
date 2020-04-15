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
            None = 0,               //Object not drawn
            TerrainDecor = 1,       //Terrain decor is drawn before paths
            TerrainPath = 2,        //Placed paths
            Resource = 3,           //Some resources may have visible signs above ground
            Shadow = 4,
            EntityUnderItem = 5,    //Some entities may want to appear below an item
            Item = 6,               //just the entities
            EntitySorted = 7,       //Regular entity layer
            Air = 8,                //Stuff that is in the air
            IconOverlay = 9         //Would be useful for player hints
        }
        public enum AnimationBehavior
        {
            Forward,
            Backward,
            ForwardAndBackward
        }
        public DrawLayer drawLayer { get; set; }
        public Vector2f drawOffset { get; set; }
        public Vector2f scale { get; set; } = new Vector2f(1.0f, 1.0f);
        public Vector2f position { get; set; }

        public Color color { get; set; } = new Color(255, 255, 255, 255);

        public abstract void SetRotation(float rotation);
        public abstract void Draw(SpriteBatch spriteBatch, Vector2f position);
    }
}
