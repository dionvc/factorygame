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
        /// <summary>
        /// Key which closes the menu panel.  Defaults to none.
        /// </summary>
        public Keyboard.Key closePanelKey { get; set; }
        public MenuPanel(Vector2f relativePosition, Vector2f componentSize, bool[] sizeScaling)
        {
            this.position = relativePosition;
            this.size = componentSize;
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
            for (int i = 0; i < attachedComponents.Count; i++)
            {
                attachedComponents[i].Draw(gui, origin + position);
            }
        }
        public override void HandleInput(InputManager input)
        {
            base.HandleInput(input);
            if(input.ConsumeKeyPress(closePanelKey))
            {
                container.RemoveMenu(this);
                return;
            }
            //TODO: move to translate and store absolute position of component precomputed?
            Vector2f origin = new Vector2f(0, 0);
            MenuComponent bubble = parent;
            while (bubble != null)
            {
                origin += bubble.position;
                bubble = bubble.parent;
            }
            if (panelState == PanelState.Dragging && input.mouseReleased[InputBindings.primary])
            {
                panelState = PanelState.Normal;
            }
            if (panelState == PanelState.Normal && input.mouseClick[InputBindings.primary] && BoundingBox.CheckPointMenuCollision(input.mouseX, input.mouseY, collisionBox, position + origin))
            {
                panelState = PanelState.Dragging;
                this.Translate(new Vector2f(input.mouseXdiff, input.mouseYdiff));
            }
            if (panelState == PanelState.Dragging)
            {
                this.Translate(new Vector2f(input.mouseXdiff, input.mouseYdiff));
            }
        }
    }
}
