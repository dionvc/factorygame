﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EngineeringCorpsCS
{
    class Player: Entity
    {
        public BoundingBox collisionBox;
        public Vector2 position;
        public Player()
        {
            collisionBox = new BoundingBox(-128, -128, 128, 128);
            collisionBox.SetRotation(45);
            position = new Vector2(128, 128);
        }
        public override void Update()
        {
        }
    }
}
