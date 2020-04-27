using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFML.Graphics;
using SFML.System;

namespace EngineeringCorpsCS
{
    class LightSourceDirectional : LightSource
    {
        float rotationIncrement = 0.0f;
        float targetRotation = 0.0f;
        int interpolationTime = 4;
        public LightSourceDirectional(SurfaceContainer surface, float lightRange, float lightWidth, Texture texture, IntRect bounds)
        {
            this.texture = texture;
            this.bounds = bounds;
            this.surface = surface;
            this.rotation = 0.0f;
            lightScale = new Vector2f(lightWidth / bounds.Width, lightRange / bounds.Height);
            origin = new Vector2f(bounds.Width / 2, bounds.Height);
        }

        public LightSourceDirectional(SurfaceContainer surface, Vector2f lightScale, Texture texture, IntRect bounds)
        {
            this.texture = texture;
            this.bounds = bounds;
            this.surface = surface;
            this.rotation = 0.0f;
            this.lightScale = new Vector2f(lightScale.X, lightScale.Y);
            this.origin = new Vector2f(bounds.Width / 2, bounds.Height);
        }

        public new LightSourceDirectional Clone()
        {
            return new LightSourceDirectional(this.surface, this.lightScale, this.texture, this.bounds);
        }
        
        public override void Update()
        {
            rotation += rotationIncrement;
            if(Math.Abs(rotation - targetRotation + 360)%360 <= 2)
            {
                rotationIncrement = 0.0f;
                rotation = targetRotation;
            }
            position = attachedEntity.position;
            surface.UpdateLightSource(this);
        }

        public void SetDirection(float rotation)
        {
            rotationIncrement = Interpolation.EulerInterpolation(this.rotation, rotation, interpolationTime);
        }
    }
}
