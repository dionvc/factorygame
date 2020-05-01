using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFML.Graphics;
using SFML.System;

namespace EngineeringCorpsCS
{
    class LightSourceFlicker:LightSource
    {
        int time = 0;
        public LightSourceFlicker(float lightRange, Texture texture, IntRect bounds)
        {
            this.texture = texture;
            this.bounds = bounds;
            this.surface = surface;
            lightScale = new Vector2f(lightRange / bounds.Width, lightRange / bounds.Height);
            origin = new Vector2f(bounds.Width / 2, bounds.Height / 2);
        }

        public LightSourceFlicker(Vector2f lightScale, Texture texture, IntRect bounds)
        {
            this.texture = texture;
            this.bounds = bounds;
            this.surface = surface;
            this.lightScale = lightScale;
            origin = new Vector2f(bounds.Width / 2, bounds.Height / 2);
        }

        public override void Draw(SpriteBatch lightBatch)
        {
            time++;
            Vector2f flickerScale = lightScale * (float)((Math.Sin(time/5.0f) + 0.5 * Math.Sin(time)/10.0f)/15 + 1.0f);
            lightBatch.Draw(texture, position.internalVector, bounds, color, flickerScale, origin, rotation);
        }

        public new LightSourceFlicker Clone()
        {
            return new LightSourceFlicker(this.lightScale, this.texture, this.bounds);
        }
    }
}
