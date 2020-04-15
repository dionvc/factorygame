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
            this.position = position;
            this.surface = surface;
            this.lightRange = lightRange;
            surface.UpdateLightSource(this);
            light = new Sprite(texture, bounds);
            light.Scale = new Vector2f(lightRange / light.TextureRect.Width, lightRange / light.TextureRect.Height);
            light.Position = new Vector2f(position.x, position.y);
            light.Origin = new Vector2f(light.TextureRect.Width / 2, light.TextureRect.Height/2);
        }
    }
}
