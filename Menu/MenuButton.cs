using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFML.System;
using SFML.Graphics;

namespace EngineeringCorpsCS.Menu
{
    class MenuButton : MenuComponent
    {
        public delegate void buttonAction(MenuButton button);
        buttonAction action;
        public MenuButton(buttonAction action)
        {
            this.action = action;
        }
        override public void Draw(RenderTexture gui, Vector2f origin)
        {

        }

        override public void Translate(Vector2f translation)
        {
            position += translation;
        }
    }
}
