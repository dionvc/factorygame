using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EngineeringCorpsCS
{
    class Machine:EntityPhysical
    {
        Animation working; //This animation plays when working and stops when idle
        Recipe activeRecipe;
        int recipeProgress = 0;
        public Machine(string name, Animation working)
        {
            this.name = name;
            this.working = working;
        }

        public override void Update()
        {
            if(activeRecipe != null)
            {

            }
        }
        public override Entity Clone()
        {
            Machine clone = new Machine(this.name, this.working.Clone());
            return clone;
        }
    }
}
