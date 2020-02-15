using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EngineeringCorpsCS
{
    abstract class Entity: Base
    {   
        public string name { get; }
        public string type { get; }
        //Consider what should be passed to entity by chunk (perhaps the chunk coordinates?)
    }
}
