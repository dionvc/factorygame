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
        public MenuButton(Vector2f relativePosition, ButtonAction action, Vector2f componentSize)
        {
            this.position = relativePosition;
            this.action = action;
            this.size = componentSize;
            this.buttonState = ButtonState.Normal;
            this.collisionBox = new BoundingBox(this.size);
            attachedComponents = new List<MenuComponent>();
        }
        override public void Draw(RenderTexture gui, Vector2f origin)
        {
            RectangleShape test = new RectangleShape(size);
            test.Position = origin + position;
            test.FillColor = Color.Yellow;
            gui.Draw(test);
            for (int i = 0; i < attachedComponents.Count; i++)
            {
                attachedComponents[i].Draw(gui, origin + position);
            }
        }

        override public void Translate(Vector2f translation)
        {
            position += translation;
        }

        public void AttachComponent(MenuComponent component)
        {
            attachedComponents.Add(component);
        }

        public void SubscribeToInput(InputManager input)
        {
            input.AddInputSubscriber(this, false);
        }

        public void UnsubscribeToInput(InputManager input)
        {
            input.RemoveInputSubscriber(this, false);
        }

        public void HandleInput(InputManager input)
        {
            if(buttonState != ButtonState.Held && BoundingBox.CheckPointMenuCollision(input.mouseX, input.mouseY, collisionBox, position))
            {
                buttonState = ButtonState.Hover;
            }
            else if(buttonState != ButtonState.Held)
            {
                buttonState = ButtonState.Normal;
            }
            if(buttonState == ButtonState.Hover && input.mouseClick[InputBindings.primary])
            {
                buttonState = ButtonState.Held;
            }
            if (buttonState == ButtonState.Held && input.mouseReleased[InputBindings.primary])
            {
                action();
            }
        }
    }
}
