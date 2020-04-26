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
        public delegate void ButtonAction(string tag);
        ButtonAction action;
        ButtonState buttonState;
        Color buttonHeld = Color.Green;
        Color buttonNormal = Color.Yellow;
        Color buttonHover = Color.Magenta;
        Color buttonColor = Color.Yellow;

        public string tag { get; set; } = "";
        public MenuButton(Vector2f componentSize, ButtonAction action)
        {
            Initialize(componentSize);
            this.action = action;
            this.buttonState = ButtonState.Normal;
            this.collisionBox = new BoundingBox(this.size);
        }
        override public void Draw(RenderTexture gui, Vector2f origin, RenderStates guiState)
        {
            RectangleShape test = new RectangleShape(size);
            test.Position = origin + position;
            test.FillColor = buttonColor;
            gui.Draw(test);
            base.Draw(gui, origin, guiState);
        }
        override public void HandleInput(InputManager input)
        {
            base.HandleInput(input);

            Vector2f mousePos; 
            bool mouse = input.GetMousePosition(out mousePos);
            Vector2f origin = new Vector2f(0, 0);
            MenuComponent bubble = parent;
            while (bubble != null)
            {
                origin += bubble.position;
                bubble = bubble.parent;
            }
            if (mouse && buttonState != ButtonState.Held && BoundingBox.CheckPointMenuCollision(mousePos.X, mousePos.Y, collisionBox, (position + origin)))
            {
                buttonState = ButtonState.Hover;
                buttonColor = buttonHover;
            }
            else if(buttonState != ButtonState.Held)
            {
                buttonState = ButtonState.Normal;
                buttonColor = buttonNormal;
            }
            if(buttonState == ButtonState.Hover && mouse && input.GetMouseClicked(InputBindings.primary, true))
            {
                buttonState = ButtonState.Held;
                buttonColor = buttonHeld;
            }
            if (buttonState == ButtonState.Held && mouse && input.GetMouseReleased(InputBindings.primary, false))
            {
                action?.Invoke(tag);
                buttonState = ButtonState.Normal;
                buttonColor = buttonNormal;
            }
        }
    }
}
