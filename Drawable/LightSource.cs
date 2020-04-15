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
        public float lightRange { get; protected set; } = 0.0f;
        public SurfaceContainer surface { get; protected set; }
        public Vector2 position { get; protected set; }
        public int centeredChunk { get; set; } = -1;
        public Sprite light;
        public Entity attachedEntity { get; set; }
        public bool on = false;



        virtual public void Update()
        {
            position = attachedEntity.position;
            light.Position = new Vector2f(position.x, position.y);
            surface.UpdateLightSource(this);
        }
    }
}
