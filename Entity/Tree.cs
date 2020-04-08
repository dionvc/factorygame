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
            Texture treeTextures = textureContainer.GetTexture("Tree01");
            drawArray = new Drawable[] { new Animation(treeTextures, 256, 256, 1, new IntRect(0,0,256,256), new Vector2f(0, -112    )) };
            collisionMask = CollisionLayer.EntityPhysical;
            mapColor = new Color(0, 255, 0);
        }
    }
}
