using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EngineeringCorpsCS
{
    class TestTilePlacer: IInputSubscriber
    {
        private SurfaceContainer surface;
        private Renderer renderer;
        /// <summary>
        /// This is a test entity!!!
        /// </summary>
        public TestTilePlacer(SurfaceContainer surface, Renderer renderer)
        {
            this.surface = surface;
            this.renderer = renderer;
        }

        public void SubscribeToInput(InputManager input)
        {
            input.AddInputSubscriber(this, false);
        }

        public void UnsubscribeToInput(InputManager input)
        {
            input.AddInputSubscriber(this, false);
        }

        public void HandleInput(InputManager input)
        {
            if(input.mouseClick[InputBindings.primary])
            {

            }
        }
    }
}
