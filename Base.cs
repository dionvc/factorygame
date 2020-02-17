﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EngineeringCorpsCS
{
    class Base
    {

        /// <summary>
        /// Describes the layer on which the instance in question will be drawn
        /// </summary>
        public enum DrawLayer
        {
            None = 0,
            Terrain = 1,            //All terrain is drawn on one layer
            TerrainDecor = 2,
            TerrainPath = 3,        //Placed paths
            Resource = 4,           //Some resources may have visible signs above ground
            Item = 5,
            EntitySorted = 6,
            Overlay = 7,
            Air = 8,
            IconOverlay = 9,
            GUI = 10
        }

        /// <summary>
        /// Describes the layer(s) on which the instance in question is collidable
        /// </summary>
        [Flags]
        public enum CollisionLayer
        {
            None = 0,               //Doesn't collide with anything
            Void = 1,               //Collides with void (no terrain)
            Resource = 2,           //Resource layer is below ground
            TerrainSolid = 4,       //For water, or super rocky terrain?
            Terrain = 8,            //For regular terrain
            TerrainDecor = 16,      //For decorations like grass, etc
            TerrainPath = 32,       //For placed paths
            Item = 64,              //For items on the ground
            EntityStatic = 128,
            EntityMoving = 256,
            All = Void | Resource | TerrainSolid | Terrain | TerrainPath | Item | EntityStatic | EntityMoving
        }
        public string name { get; protected set; }
    }
}
