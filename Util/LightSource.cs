using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFML.System;
using SFML.Graphics;

namespace EngineeringCorpsCS
{
    class LightSource
    {
        public float lightRange { get; protected set; } = 0.0f;
        public SurfaceContainer surface { get; protected set; }
        public Vector2 position { get; protected set; }
        public int centeredChunk { get; set; } = -1;
        public Sprite light;
        private Entity attachedEntity;

        public LightSource(Vector2 position, SurfaceContainer surface, float lightRange, Texture texture, Entity attachedEntity)
        {
            this.position = position;
            this.surface = surface;
            this.lightRange = lightRange;
            this.attachedEntity = attachedEntity;
            surface.UpdateLightSource(this);
            light = new Sprite(texture);
            light.Scale = new Vector2f(lightRange / light.TextureRect.Width, lightRange / light.TextureRect.Height);
            light.Position = new Vector2f(position.x, position.y);
        }

        public void Update()
        {
            position = attachedEntity.position;
            light.Position = new Vector2f(position.x - light.Scale.X * light.TextureRect.Width/2, position.y - light.Scale.Y * light.TextureRect.Height/2);
            surface.UpdateLightSource(this);
        }
    }
}
