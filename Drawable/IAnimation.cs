using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFML.Graphics;

namespace EngineeringCorpsCS
{
    interface IAnimation
    {
        void Update();
        Sprite GetAnimationFrame();
        void SetAnimationSpeed(float animationSpeed);
        void SetBehavior(string behavior);
    }
}
