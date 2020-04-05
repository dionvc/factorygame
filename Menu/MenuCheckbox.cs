using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFML.Graphics;
using SFML.System;

namespace EngineeringCorpsCS
{
    enum CheckboxState
    {
        Checked,
        Unchecked
    }
    class MenuCheckbox : MenuComponent
    {
        public delegate bool ApplyState(string tag, bool state);
        public MenuCheckbox(Vector2f componentSize, bool initialState)
        {

        }
        public override void Draw(RenderTexture gui, Vector2f origin, RenderStates guiState)
        {
            base.Draw(gui, origin, guiState);
        }

        public override void HandleInput(InputManager input)
        {
            base.HandleInput(input);

        }
    }
}
