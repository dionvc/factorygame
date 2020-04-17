using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EngineeringCorpsCS
{
    class EntityGhost: EntityPhysical
    {
        public EntityGhost(BoundingBox box, Vector2 position, SurfaceContainer surface)
        {
            this.position = position;
            this.collisionBox = box;
            this.collisionMask = CollisionLayer.TerrainSolid | CollisionLayer.EntityPhysical;
            this.surface = surface;
        }

        public override Entity Clone()
        {
            throw new NotImplementedException();
        }
    }
}
