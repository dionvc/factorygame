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
        VertexArray buttonGraphic;
        RectangleShape highlight;
        Color buttonHeld = new Color(0, 64, 0, 128);
        Color buttonNormal = new Color(0,0,0,0);
        Color buttonHover = new Color(0, 128, 0, 128);
        Color buttonColor = new Color(0, 0, 0, 128);

        public string tag { get; set; } = "";
        public MenuButton(Vector2i componentSize, ButtonAction action)
        {
            Initialize(componentSize);
            this.action = action;
            this.buttonState = ButtonState.Normal;
            this.collisionBox = new BoundingBox(this.size);
            buttonGraphic = CreateMenuGraphicArrayWithBorder(new FloatRect(320, 0, 96, 96), 20);
            highlight = new RectangleShape(new Vector2f(componentSize.X, componentSize.Y));
        }
        override public void Draw(RenderTexture gui, Vector2i origin, RenderStates guiState)
        {
            Transform t = new Transform(1, 0, 0, 0, 1, 0, 0, 0, 1);
            Vector2f pos = new Vector2f((origin + position).X, (origin + position).Y);
            t.Translate(pos);
            Transform original = guiState.Transform;
            guiState.Transform = t;
            gui.Draw(buttonGraphic, guiState);
            highlight.FillColor = buttonColor;
            highlight.Position = pos;
            gui.Draw(highlight);
            base.Draw(gui, origin, guiState);
        }
        override public void HandleInput(InputManager input)
        {
            base.HandleInput(input);

            Vector2f mousePos; 
            bool mouse = input.GetMousePosition(out mousePos);
            Vector2i origin = new Vector2i(0, 0);
            MenuComponent bubble = parent;
            while (bubble != null)
            {
                origin += bubble.position;
                bubble = bubble.parent;
            }
            Vector2f pos = new Vector2f((position + origin).X, (origin + position).Y);
            if (mouse && buttonState != ButtonState.Held && BoundingBox.CheckPointMenuCollision(mousePos.X, mousePos.Y, collisionBox, pos))
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
