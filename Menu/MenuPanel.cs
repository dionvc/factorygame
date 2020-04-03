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

        VertexArray panelGraphic;
        public MenuPanel(Vector2f relativePosition, Vector2f componentSize, FloatRect bounds, float borderSize)
        {
            Initialize(relativePosition, componentSize);
            this.scale = new Vector2f(1, 1);
            collisionBox = new BoundingBox(this.size);
            panelState = PanelState.Normal;

            panelGraphic = new VertexArray(PrimitiveType.Triangles);
            //Topleft
            panelGraphic.Append(new Vertex(new Vector2f(0, 0), new Vector2f(0, 0)));
            panelGraphic.Append(new Vertex(new Vector2f(borderSize, 0), new Vector2f(32, 0)));
            panelGraphic.Append(new Vertex(new Vector2f(0, borderSize), new Vector2f(0, 32)));
            panelGraphic.Append(new Vertex(new Vector2f(borderSize, 0), new Vector2f(32, 0)));
            panelGraphic.Append(new Vertex(new Vector2f(borderSize, borderSize), new Vector2f(32, 32)));
            panelGraphic.Append(new Vertex(new Vector2f(0, borderSize), new Vector2f(0, 32)));
            //Topright
            panelGraphic.Append(new Vertex(new Vector2f(size.X - borderSize, 0), new Vector2f(64, 0)));
            panelGraphic.Append(new Vertex(new Vector2f(size.X, 0), new Vector2f(96, 0)));
            panelGraphic.Append(new Vertex(new Vector2f(size.X - borderSize, borderSize), new Vector2f(64, 32)));
            panelGraphic.Append(new Vertex(new Vector2f(size.X, 0), new Vector2f(96, 0)));
            panelGraphic.Append(new Vertex(new Vector2f(size.X, borderSize), new Vector2f(96, 32)));
            panelGraphic.Append(new Vertex(new Vector2f(size.X - borderSize, borderSize), new Vector2f(64, 32)));
            //Top
            panelGraphic.Append(new Vertex(new Vector2f(borderSize, 0), new Vector2f(32, 0)));
            panelGraphic.Append(new Vertex(new Vector2f(size.X - borderSize, 0), new Vector2f(64, 0)));
            panelGraphic.Append(new Vertex(new Vector2f(borderSize, borderSize), new Vector2f(32, 32)));
            panelGraphic.Append(new Vertex(new Vector2f(size.X - borderSize, 0), new Vector2f(64, 0)));
            panelGraphic.Append(new Vertex(new Vector2f(size.X - borderSize, borderSize), new Vector2f(64, 32)));
            panelGraphic.Append(new Vertex(new Vector2f(borderSize, borderSize), new Vector2f(32, 32)));
            //Center
            panelGraphic.Append(new Vertex(new Vector2f(borderSize, borderSize), new Vector2f(32, 32)));
            panelGraphic.Append(new Vertex(new Vector2f(size.X-borderSize, borderSize), new Vector2f(64, 32)));
            panelGraphic.Append(new Vertex(new Vector2f(borderSize, size.Y - borderSize), new Vector2f(32, 64)));
            panelGraphic.Append(new Vertex(new Vector2f(size.X-borderSize, borderSize), new Vector2f(64, 32)));
            panelGraphic.Append(new Vertex(new Vector2f(size.X-borderSize, size.Y-borderSize), new Vector2f(64, 64)));
            panelGraphic.Append(new Vertex(new Vector2f(borderSize, size.Y-borderSize), new Vector2f(32, 64)));
            //Left
            panelGraphic.Append(new Vertex(new Vector2f(0, borderSize), new Vector2f(0, 32)));
            panelGraphic.Append(new Vertex(new Vector2f(borderSize, borderSize), new Vector2f(32, 32)));
            panelGraphic.Append(new Vertex(new Vector2f(0, size.Y-borderSize), new Vector2f(0, 64)));
            panelGraphic.Append(new Vertex(new Vector2f(borderSize, borderSize), new Vector2f(32, 32)));
            panelGraphic.Append(new Vertex(new Vector2f(borderSize, size.Y-borderSize), new Vector2f(32, 64)));
            panelGraphic.Append(new Vertex(new Vector2f(0, size.Y-borderSize), new Vector2f(0, 64)));
            //Right
            panelGraphic.Append(new Vertex(new Vector2f(size.X-borderSize, borderSize), new Vector2f(64, 32)));
            panelGraphic.Append(new Vertex(new Vector2f(size.X, borderSize), new Vector2f(96, 32)));
            panelGraphic.Append(new Vertex(new Vector2f(size.X-borderSize, size.Y - borderSize), new Vector2f(64, 64)));
            panelGraphic.Append(new Vertex(new Vector2f(size.X, borderSize), new Vector2f(96, 32)));
            panelGraphic.Append(new Vertex(new Vector2f(size.X, size.Y - borderSize), new Vector2f(96, 64)));
            panelGraphic.Append(new Vertex(new Vector2f(size.X-borderSize, size.Y - borderSize), new Vector2f(64, 64)));
            //bottomleft
            panelGraphic.Append(new Vertex(new Vector2f(0, size.Y - borderSize), new Vector2f(0, 64)));
            panelGraphic.Append(new Vertex(new Vector2f(borderSize, size.Y -borderSize), new Vector2f(32, 64)));
            panelGraphic.Append(new Vertex(new Vector2f(0, size.Y), new Vector2f(0, 96)));
            panelGraphic.Append(new Vertex(new Vector2f(borderSize, size.Y - borderSize), new Vector2f(32, 64)));
            panelGraphic.Append(new Vertex(new Vector2f(borderSize, size.Y), new Vector2f(32, 96)));
            panelGraphic.Append(new Vertex(new Vector2f(0, size.Y), new Vector2f(0, 96)));
            //bottomright
            panelGraphic.Append(new Vertex(new Vector2f(size.X - borderSize, size.Y - borderSize), new Vector2f(64, 64)));
            panelGraphic.Append(new Vertex(new Vector2f(size.X, size.Y - borderSize), new Vector2f(96, 64)));
            panelGraphic.Append(new Vertex(new Vector2f(size.X - borderSize, size.Y), new Vector2f(64, 96)));
            panelGraphic.Append(new Vertex(new Vector2f(size.X, size.Y - borderSize), new Vector2f(96, 64)));
            panelGraphic.Append(new Vertex(new Vector2f(size.X, size.Y), new Vector2f(96, 96)));
            panelGraphic.Append(new Vertex(new Vector2f(size.X - borderSize, size.Y), new Vector2f(64, 96)));
            //bottom
            panelGraphic.Append(new Vertex(new Vector2f(borderSize, size.Y - borderSize), new Vector2f(32, 64)));
            panelGraphic.Append(new Vertex(new Vector2f(size.X - borderSize, size.Y - borderSize), new Vector2f(64, 64)));
            panelGraphic.Append(new Vertex(new Vector2f(borderSize, size.Y), new Vector2f(32, 96)));
            panelGraphic.Append(new Vertex(new Vector2f(size.X - borderSize, size.Y - borderSize), new Vector2f(64, 64)));
            panelGraphic.Append(new Vertex(new Vector2f(size.X - borderSize, size.Y), new Vector2f(64, 96)));
            panelGraphic.Append(new Vertex(new Vector2f(0, size.Y), new Vector2f(32, 96)));


        }
        override public void Draw(RenderTexture gui, Vector2f origin, RenderStates guiState)
        {
            RectangleShape test = new RectangleShape(size);
            test.Position = origin + position;
            test.FillColor = Color.Red;
            //Construct transform
            Transform t = new Transform(1, 0, 0, 0, 1, 0, 0, 0, 1);
            t.Translate(origin + position);
            Transform original = guiState.Transform;
            guiState.Transform = t;
            gui.Draw(panelGraphic, guiState);
            //Draw children
            base.Draw(gui, origin, guiState);
            //Return to original transform
            guiState.Transform = original;
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
