using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFML.Graphics;
using SFML.System;

namespace EngineeringCorpsCS
{
    class MenuPanel : MenuComponent
    {

        public MenuPanel()
        {
            position = new Vector2(150, 150);
            scale = new Vector2f(1, 1);
            collisionBox = new BoundingBox(0, 0, 150, 150);
        }
        override public void Draw(RenderTexture gui, Vector2f origin)
        {
            RectangleShape test = new RectangleShape(new Vector2f(150, 150));
            test.Position = position.drawVec;
            test.FillColor = Color.Red;
            gui.Draw(test);
        }

        public void Translate(float x, float y)
        {
            position.x += x;
            position.y += y;
        }
    }
}
