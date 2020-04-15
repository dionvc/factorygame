using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFML.Graphics;
using SFML.System;

namespace EngineeringCorpsCS
{
    class LightSourceDirectional : LightSource
    {
        float lightWidth = 1.0f;
        public LightSourceDirectional(Vector2 position, SurfaceContainer surface, float lightRange, float lightWidth, Texture texture, IntRect bounds)
        {
            this.position = position;
            this.surface = surface;
            this.lightRange = lightRange;
            this.lightWidth = lightWidth;
            surface.UpdateLightSource(this);
            light = new Sprite(texture, bounds);
            light.Scale = new Vector2f(lightWidth / light.TextureRect.Width, lightRange / light.TextureRect.Height);
            light.Position = new Vector2f(position.x, position.y);
            light.Origin = new Vector2f(light.TextureRect.Width / 2, light.TextureRect.Height);
        }
        
        public override void Update()
        {
            position = attachedEntity.position;
            light.Position = new Vector2f(position.x, position.y);
            surface.UpdateLightSource(this);
        }

        public void SetDirection(float rotation)
        {
            light.Rotation = rotation;
        }
    }
}
