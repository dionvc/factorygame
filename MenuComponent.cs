using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFML.System;
using SFML.Graphics;

namespace EngineeringCorpsCS
{
    abstract class MenuComponent
    {
        public Vector2 position { get; protected set; }
        public Vector2 size { get; protected set; }
        public bool fixedWidth = true;
        public bool fixedHeight = false;
        protected Vector2f scale;
        public BoundingBox collisionBox { get; protected set; }

        abstract public void Draw(RenderTexture gui, Vector2f origin);
    }
}
