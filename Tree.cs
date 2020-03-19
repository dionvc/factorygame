using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFML.Graphics;
using SFML.System;

namespace EngineeringCorpsCS
{
    class Tree: EntityPhysical
    {
        public Tree(Vector2 pos, SurfaceContainer surface, TextureContainer textureContainer)
        {
            position = pos;
            collisionBox = new BoundingBox(32, 32);
            surface.InitiateEntityInChunks(this);
            Texture[] treeTextures = new Texture[] { textureContainer.GetTexture("Tree01") };
            drawArray = new Drawable[] { new Animation(treeTextures, new Vector2i(256, 256), new Vector2f(0, 0), new Vector2f(0, -112), new Vector2f(1, 1), 1, "Forward", 0.0f) };
            collisionMask = CollisionLayer.EntityPhysical;
        }
    }
}
