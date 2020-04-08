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
        public delegate void PanelAction(string tag);
        /// <summary>
        /// Key which closes the menu panel.  Defaults to none.
        /// </summary>
        public Keyboard.Key closePanelKey { get; set; } = InputBindings.nullKey;
        public PanelAction ClosePanelAction { get; set; } = null;

        public string tag { get; set; } = "";

        VertexArray vertexArray;
        public MenuPanel(Vector2f relativePosition, Vector2f componentSize, FloatRect bounds, float borderSize, PanelAction closePanelAction)
        {
            this.size = componentSize;
            this.position = relativePosition;
            this.ClosePanelAction = closePanelAction;
            collisionBox = new BoundingBox(this.size);
            panelState = PanelState.Normal;
            vertexArray = CreateMenuGraphicArrayWithBorder(bounds, borderSize);
        }

        
        override public void Draw(RenderTexture gui, Vector2f origin, RenderStates guiState)
        {
            //Construct transform
            Transform t = new Transform(1, 0, 0, 0, 1, 0, 0, 0, 1);
            t.Translate(origin + position);
            Transform original = guiState.Transform;
            guiState.Transform = t;
            gui.Draw(vertexArray, guiState);
            //Draw children
            base.Draw(gui, origin, guiState);
            //Return to original transform
            guiState.Transform = original;
        }
        public override void HandleInput(InputManager input)
        {
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
            if (input.GetMouseClicked(InputBindings.primary, false) && collided)
            {
                container.PushMenuToFront(this);
            }
            base.HandleInput(input);
            if(closePanelKey != InputBindings.nullKey && input.GetKeyPressed(closePanelKey, true))
            {
                ClosePanelAction?.Invoke(tag);
                container.RemoveMenu(this);
                return;
            }
            
            
            
            if (panelState == PanelState.Dragging && input.GetMouseReleased(InputBindings.primary, false))
            {
                panelState = PanelState.Normal;
            }
            if (panelState == PanelState.Normal && input.GetMouseClicked(InputBindings.primary, false) && collided)
            {
                input.GetMouseClicked(InputBindings.primary, true);
                panelState = PanelState.Dragging;
                this.Translate(input.GetMouseDiff());
            }
            if (panelState == PanelState.Dragging)
            {
                this.Translate(input.GetMouseDiff());
            }
        }

        public void ClosePanel(string tag)
        {
            if (tag == null)
            {
                ClosePanelAction.Invoke(this.tag);
            }
            else
            {
                ClosePanelAction.Invoke(tag);
            }
            container.RemoveMenu(this);
        }
    }
}
