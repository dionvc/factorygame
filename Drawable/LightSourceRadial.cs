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
        public LightSourceRadial(Vector2 position, SurfaceContainer surface, float lightRange, Texture texture, IntRect bounds)
        {
            this.texture = texture;
            this.bounds = bounds;
            this.position = position;
            this.surface = surface;
            surface.UpdateLightSource(this);
            lightScale = new Vector2f(lightRange / bounds.Width, lightRange / bounds.Height);
            origin = new Vector2f(bounds.Width / 2, bounds.Height/2);
        }
    }
}
