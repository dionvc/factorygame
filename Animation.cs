using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFML.Graphics;
using SFML.System;

namespace EngineeringCorpsCS
{
    class Animation: IAnimation
    {
        enum AnimationBehavior
        {
            Forward = 1,
            Backward = 1,
            ForwardAndBackward = -1
        }
        AnimationBehavior behavior;
        Sprite animationFrame;
        Texture[] textureRefs;
        Vector2i size; //stores x (width), y (height) of sprite to capture from texture
        Vector2i texturePos; //position on texture
        Vector2i textureSize;
        Vector2i offset; //offset of sprite from center of entity in pixels 
        int incrementAmount = 1;
        public void Update()
        {

        }

        public Sprite GetAnimationFrame()
        {
            return null;
        }

        public void SetAnimationSpeed(float animationSpeed)
        {

        }

        public void SetBehavior(string behavior)
        {
            switch (behavior)
            {
                case ("Forward"):
                    this.behavior = AnimationBehavior.Forward;
                    this.incrementAmount = 1;
                    break;
                case ("Backward"):
                    this.behavior = AnimationBehavior.Backward;
                    this.incrementAmount = -1;
                    break;
                case ("ForwardAndBackward"):
                    this.behavior = AnimationBehavior.ForwardAndBackward;
                    this.incrementAmount = 1;
                    break;
                default:
                    this.behavior = AnimationBehavior.Forward;
                    this.incrementAmount = 1;
                    break;
            }
        }
    }
}
