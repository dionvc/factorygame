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
        public Tree(Vector2 pos, SurfaceContainer surface, TextureAtlases textureAtlases)
        {
            position = pos;
            collisionBox = new BoundingBox(32, 32);
            surface.InitiateEntityInChunks(this);
            IntRect bounds;
            Texture treeTextures = textureAtlases.GetTexture("Tree01", out bounds);
            drawArray = new Drawable[] { new Animation(treeTextures, 256, 256, 1, bounds, new Vector2f(0, -112)) };
            collisionMask = CollisionLayer.EntityPhysical;
            mapColor = new Color(32, 160, 0);
        }
    }
}
