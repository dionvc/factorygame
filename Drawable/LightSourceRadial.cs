using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFML.Graphics;
using SFML.System;

namespace EngineeringCorpsCS
{
    class LightSourceRadial : LightSource
    {
        public LightSourceRadial(SurfaceContainer surface, float lightRange, Texture texture, IntRect bounds)
        {
            this.texture = texture;
            this.bounds = bounds;
            this.surface = surface;
            lightScale = new Vector2f(lightRange / bounds.Width, lightRange / bounds.Height);
            origin = new Vector2f(bounds.Width / 2, bounds.Height/2);
        }

        public LightSourceRadial(SurfaceContainer surface, Vector2f lightScale, Texture texture, IntRect bounds)
        {
            this.texture = texture;
            this.bounds = bounds;
            this.surface = surface;
            this.lightScale = lightScale;
            origin = new Vector2f(bounds.Width / 2, bounds.Height / 2);
        }

        public new LightSourceRadial Clone()
        {
            return new LightSourceRadial(this.surface, this.lightScale, this.texture, this.bounds);
        }
    }
}
