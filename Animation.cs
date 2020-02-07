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
