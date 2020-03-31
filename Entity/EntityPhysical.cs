using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EngineeringCorpsCS
{
    class EntityPhysical: Entity
    {
        /// <summary>
        /// The health of the entity.
        /// </summary>
        public float health { 
            get => health;
            set {
                if (health > fullHealth) 
                {
                    health = fullHealth;
                } 
                else
                {
                    health = value;
                }
            } 
        }
        public float fullHealth;
        override public void OnClick()
        {

        }
    }
}
