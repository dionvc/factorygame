using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFML.Graphics;
using SFML.System;

namespace EngineeringCorpsCS
{
    class StaticSprite : Drawable
    {
        Texture texture;
        IntRect textureFrame;
        float rotation;
        Vector2f origin;
        Sprite staticSprite;

        public StaticSprite(Texture texture, IntRect bounds, DrawLayer drawLayer)
        {
            this.texture = texture;
            this.origin = new Vector2f(bounds.Width / 2, bounds.Height / 2);
            this.color = new Color(255, 255, 255, 255);
            textureFrame = bounds;
            drawOffset = new Vector2f(0, 0);
        }

        public StaticSprite(Texture texture, IntRect bounds, Vector2f drawOffset)
        {
            this.texture = texture;
            this.origin = new Vector2f(bounds.Width / 2, bounds.Height / 2);
            this.color = new Color(255, 255, 255, 255);
            textureFrame = bounds;
            this.drawOffset = drawOffset;
        }

        public new StaticSprite Clone()
        {
            StaticSprite cloned = new StaticSprite(this.texture, this.textureFrame, new Vector2f(this.drawOffset.X, this.drawOffset.Y));
            cloned.drawLayer = this.drawLayer;
            cloned.color = this.color;
            return cloned;
        }

        public override void Draw(SpriteBatch spriteBatch, Vector2f position)
        {
            spriteBatch.Draw(texture, position + drawOffset, textureFrame, color, scale, origin, rotation);
        }

        public Sprite GetSprite()
        {
            if(staticSprite == null)
            {
                staticSprite = new Sprite(texture, textureFrame);
            }
            return staticSprite;
        }

        public override void SetRotation(float rotation)
        {
            this.rotation = rotation;
        }
    }
}
