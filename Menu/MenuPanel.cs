using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFML.Graphics;
using SFML.System;
using SFML.Window;

namespace EngineeringCorpsCS
{
    class MenuPanel : MenuComponent, IInputSubscriber
    {
        enum PanelState
        {
            Dragging,
            Normal
        }
        PanelState panelState;
        public delegate void Action();
        /// <summary>
        /// Key which closes the menu panel.  Defaults to none.
        /// </summary>
        public Keyboard.Key closePanelKey { get; set; } = Keyboard.Key.F15;
        public Action ClosePanelAction { get; set; } = null;
        public MenuPanel(Vector2f relativePosition, Vector2f componentSize)
        {
            Initialize(relativePosition, componentSize);
            this.scale = new Vector2f(1, 1);
            collisionBox = new BoundingBox(this.size);
            panelState = PanelState.Normal;
        }
        override public void Draw(RenderTexture gui, Vector2f origin)
        {
            RectangleShape test = new RectangleShape(size);
            test.Position = origin + position;
            test.FillColor = Color.Red;
            gui.Draw(test);
            base.Draw(gui, origin);
        }
        public override void HandleInput(InputManager input)
        {
            base.HandleInput(input);
            if(input.GetKeyPressed(closePanelKey, true))
            {
                ClosePanelAction?.Invoke();
                container.RemoveMenu(this);
                return;
            }
            
            //Computing absolute position to check collision
            Vector2f mousePos = input.GetMousePosition();
            Vector2f origin = new Vector2f(0, 0);
            MenuComponent bubble = parent;
            while (bubble != null)
            {
                origin += bubble.position;
                bubble = bubble.parent;
            }
            //If the menu is collided with then it will consume the mouse position so that it does not interact with anything else
            bool collided = BoundingBox.CheckPointMenuCollision(mousePos.X, mousePos.Y, collisionBox, position + origin);
            
            if (panelState == PanelState.Dragging && input.GetMouseReleased(InputBindings.primary, false))
            {
                panelState = PanelState.Normal;
            }
            if (panelState == PanelState.Normal && input.GetMouseClicked(InputBindings.primary, false) && collided)
            {
                input.GetMouseClicked(InputBindings.primary, true);
                container.PushMenuToFront(this);
                panelState = PanelState.Dragging;
                this.Translate(input.GetMouseDiff());
            }
            if (panelState == PanelState.Dragging)
            {
                this.Translate(input.GetMouseDiff());
            }
        }
    }
}
