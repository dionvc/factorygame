using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EngineeringCorpsCS
{
    interface IInputSubscriber
    {
        /// <summary>
        /// Adds an entity to the input handling queue
        /// </summary>
        /// <param name="input"></param>
        void SubscribeToInput(InputManager input);

        /// <summary>
        /// Removes an entity from the input handling queue
        /// </summary>
        /// <param name="input"></param>
        void UnsubscribeToInput(InputManager input);
        /// <summary>
        /// Where input will be processed by an entity
        /// </summary>
        /// <param name="input"></param>
        void HandleInput(InputManager input);
    }
}
