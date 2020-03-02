using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EngineeringCorpsCS
{
    class Player: Entity
    {
        public Player(Vector2 pos)
        {
            position = pos;
            collisionBox = new BoundingBox(-64, -64, 64, 64);
        }
    }
}
