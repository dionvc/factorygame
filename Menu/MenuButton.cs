using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFML.System;
using SFML.Graphics;

namespace EngineeringCorpsCS
{
    class MenuButton : MenuComponent, IInputSubscriber
    {
        enum ButtonState
        {
            Normal,
            Hover,
            Held,
            Released
        }
        public delegate void ButtonAction();
        ButtonAction action;
        ButtonState buttonState;
        Color buttonHeld = Color.Green;
        Color buttonNormal = Color.Yellow;
        Color buttonHover = Color.Magenta;
        Color buttonColor = Color.Yellow;
        public MenuButton(Vector2f relativePosition, ButtonAction action, Vector2f componentSize)
        {
            this.position = relativePosition;
            this.action = action;
            this.size = componentSize;
            this.buttonState = ButtonState.Normal;
            this.collisionBox = new BoundingBox(this.size);
        }
        override public void Draw(RenderTexture gui, Vector2f origin)
        {
            RectangleShape test = new RectangleShape(size);
            test.Position = origin + position;
            test.FillColor = buttonColor;
            gui.Draw(test);
            for (int i = 0; i < attachedComponents.Count; i++)
            {
                attachedComponents[i].Draw(gui, origin + position);
            }
        }
        override public void HandleInput(InputManager input)
        {
            //TODO: move to translate and store absolute position of component precomputed?
            Vector2f origin = new Vector2f(0, 0);
            MenuComponent bubble = parent;
            Vector2f mousePos = input.GetMousePosition();
            while(bubble != null)
            {
                origin += bubble.position;
                bubble = bubble.parent;
            }
            if(buttonState != ButtonState.Held && BoundingBox.CheckPointMenuCollision(mousePos.X, mousePos.Y, collisionBox, (position + origin)))
            {
                buttonState = ButtonState.Hover;
                buttonColor = buttonHover;
            }
            else if(buttonState != ButtonState.Held)
            {
                buttonState = ButtonState.Normal;
                buttonColor = buttonNormal;
            }
            if(buttonState == ButtonState.Hover && input.mouseClick[InputBindings.primary])
            {
                buttonState = ButtonState.Held;
                buttonColor = buttonHeld;
            }
            if (buttonState == ButtonState.Held && input.mouseReleased[InputBindings.primary])
            {
                action();
                buttonState = ButtonState.Normal;
                buttonColor = buttonNormal;
            }
        }
    }
}
