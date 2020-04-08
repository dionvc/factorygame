using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFML.Graphics;
using SFML.System;

namespace EngineeringCorpsCS
{
    class Tile: Base
    {
        public Texture tileSheet { get; protected set; } //texture with terrain tiles
        public IntRect textureRect { get; protected set; }
        public byte tileType { get; protected set; } //number specifying the type of the tile (corresponding to the type in the terrain array and tileCollection collection)
        public CollisionLayer collisionMask { get; protected set; } //Whatever layers are set here is what will determine its collision type
        public float frictionModifier { get; protected set; } = 1.0f; //modifier for speed of entities on this tile
        public float moistureAffinity { get; protected set; }
        public float elevationAffinity { get; protected set; }
        public float temperatureAffinity { get; protected set; }

        public Color shade { get; protected set; } //Apply a color to the tile
        public Color mapColor { get; protected set; } //Color that will be represented in minimap
        

        public Tile(Texture tileSheet, IntRect textureRect, byte tileType, string name, float frictionModifier, Color shade, Color mapColor, CollisionLayer collisionMask, float mA, float eA, float tA)
        {
            this.tileSheet = tileSheet;
            this.textureRect = textureRect;
            this.tileType = tileType;
            this.frictionModifier = frictionModifier;
            this.shade = shade;
            this.mapColor = mapColor;
            this.collisionMask = collisionMask;
            this.name = name;
            this.moistureAffinity = mA;
            this.elevationAffinity = eA;
            this.temperatureAffinity = tA;
        }
    }
}
