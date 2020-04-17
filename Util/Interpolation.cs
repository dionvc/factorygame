using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EngineeringCorpsCS

{
    class Interpolation
    {

        /// <summary>
        /// Returns an increment amount to allow linear interpolation between two angles in a specified number of ticks.
        /// </summary>
        /// <param name="startAngle"></param>
        /// <param name="endAngle"></param>
        /// <param name="ticks"></param>
        /// <returns></returns>
        public static  float EulerInterpolation(float startAngle, float endAngle, int ticks)
        {
            return (((((endAngle - startAngle) % 360) + 540) % 360) - 180) / ticks;
        }
    }
}
