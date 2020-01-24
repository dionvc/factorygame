using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EngineeringCorpsCS
{
    interface IEntity
    {

        //Consider what should be passed to entity by chunk (perhaps the chunk coordinates?)
        void Update();
    }
}
