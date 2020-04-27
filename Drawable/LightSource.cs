using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFML.System;
using SFML.Graphics;

namespace EngineeringCorpsCS
{
    abstract class LightSource
    {
        public SurfaceContainer surface { get; protected set; }
        public Vector2 position { get; protected set; }
        public int centeredChunk { get; set; } = -1;
        public Entity attachedEntity { get; set; }
        protected Texture texture;
        protected IntRect bounds;
        public Color color { get; set; } = new Color(255, 255, 255, 255);
        public Vector2f lightScale { get; set; }
        public Vector2f origin { get; set; }
        public float rotation;
        public bool on = false;

        virtual public void Draw(SpriteBatch lightBatch)
        {
            lightBatch.Draw(texture, position.internalVector, bounds, color, lightScale, origin, rotation);
        }

        virtual public void Update()
        {
            position = attachedEntity.position;
            surface.UpdateLightSource(this);
        }

        public LightSource Clone()
        {
            throw new UnauthorizedAccessException();
        }
    }
}
