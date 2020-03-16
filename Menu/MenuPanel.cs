using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFML.Graphics;
using SFML.System;

namespace EngineeringCorpsCS
{
    class MenuPanel : MenuComponent, IInputSubscriber
    {
        
        public MenuPanel(Vector2f relativePosition, Vector2f componentSize, bool[] sizeScaling)
        {
            this.position = relativePosition;
            this.size = componentSize;
            this.scale = new Vector2f(1, 1);
            collisionBox = new BoundingBox(this.size);
        }
        override public void Draw(RenderTexture gui, Vector2f origin)
        {
            RectangleShape test = new RectangleShape(size);
            test.Position = origin + position;
            test.FillColor = Color.Red;
            gui.Draw(test);
            for (int i = 0; i < attachedComponents.Count; i++)
            {
                attachedComponents[i].Draw(gui, origin + position);
            }
        }
    }
}
