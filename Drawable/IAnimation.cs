using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EngineeringCorpsCS
{
    interface IAnimation
    {
        Drawable.AnimationBehavior behavior { get; set; }
        float animationSpeed { get; set; }
        void Update();
    }
}
