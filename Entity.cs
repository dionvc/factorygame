using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EngineeringCorpsCS
{
    abstract class Entity
    {
        protected string name;
        //Consider what should be passed to entity by chunk (perhaps the chunk coordinates?)
        public abstract void Update();
    }
}
