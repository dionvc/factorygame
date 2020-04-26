using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFML.System;
using SFML.Graphics;

namespace EngineeringCorpsCS
{
    class MenuComponent: IInputSubscriber
    {
        public Vector2f position { get; protected set; }
        public Vector2f size { get; protected set; }
        public MenuComponent parent { get; protected set; }
        public BoundingBox collisionBox { get; protected set; }
        public List<MenuComponent> attachedComponents { get; protected set; } = new List<MenuComponent>();
        public MenuContainer container { get; set; }
        public bool lockedPosition { get; set; } = false;
        protected string pivot1 = "center";
        protected string pivot2 = "center";
        protected string pivot3 = "inside";
        protected float margin = 0.0f; //specify margin of attachment.  If attaching to a component, then the sign matters.

        public void Initialize(Vector2f size)
        {
            this.position = new Vector2f(0,0);
            this.size = size;
        }
        virtual public void Draw(RenderTexture gui, Vector2f origin, RenderStates guiState)
        {
            for (int i = 0; i < attachedComponents.Count; i++)
            {
                attachedComponents[i].Draw(gui, origin + position, guiState);
            }
        }

        /// <summary>
        /// Creates a vertex array for the graphic.  Assumes the graphic is constructed from a 3x3 uniform tileset.  Does use stretching for sides and center.
        /// </summary>
        /// <param name="vertexArray"></param>
        /// <param name="bounds"></param>
        /// <param name="borderSize"></param>
        public VertexArray CreateMenuGraphicArrayWithBorder(FloatRect bounds, float borderSize)
        {
            VertexArray vertexArray = new VertexArray(PrimitiveType.Triangles);
            float pR = 0.5f;
            float oX = bounds.Left; //origin x
            float oY = bounds.Top;
            float iX = bounds.Width / 3; //increment x
            float iY = bounds.Height / 3;
            //Topleft
            vertexArray.Append(new Vertex(new Vector2f(0, 0), new Vector2f(oX, oY)));
            vertexArray.Append(new Vertex(new Vector2f(borderSize, 0), new Vector2f(oX + iX, oY)));
            vertexArray.Append(new Vertex(new Vector2f(0, borderSize), new Vector2f(oX, oY + iY)));
            vertexArray.Append(new Vertex(new Vector2f(borderSize, 0), new Vector2f(oX + iX, oY)));
            vertexArray.Append(new Vertex(new Vector2f(borderSize, borderSize), new Vector2f(oX + iX, oY + iY)));
            vertexArray.Append(new Vertex(new Vector2f(0, borderSize), new Vector2f(oX, oY + iY)));
            //Topright
            vertexArray.Append(new Vertex(new Vector2f(size.X - borderSize, 0), new Vector2f(oX + 2 * iX, oY)));
            vertexArray.Append(new Vertex(new Vector2f(size.X, 0), new Vector2f(oX + 3 * iX, oY)));
            vertexArray.Append(new Vertex(new Vector2f(size.X - borderSize, borderSize), new Vector2f(oX + 2 * iX, oY + iY)));
            vertexArray.Append(new Vertex(new Vector2f(size.X, 0), new Vector2f(oX + 3 * iX, oY)));
            vertexArray.Append(new Vertex(new Vector2f(size.X, borderSize), new Vector2f(oX + 3 * iX, oY + iY)));
            vertexArray.Append(new Vertex(new Vector2f(size.X - borderSize, borderSize), new Vector2f(oX + 2 * iX, oY + iY)));
            //Top
            vertexArray.Append(new Vertex(new Vector2f(borderSize, 0), new Vector2f(oX + iX, oY)));
            vertexArray.Append(new Vertex(new Vector2f(size.X - borderSize, 0), new Vector2f(oX + 2 * iX, oY)));
            vertexArray.Append(new Vertex(new Vector2f(borderSize, borderSize), new Vector2f(oX + iX, oY + iY)));
            vertexArray.Append(new Vertex(new Vector2f(size.X - borderSize, 0), new Vector2f(oX + 2 * iX, oY)));
            vertexArray.Append(new Vertex(new Vector2f(size.X - borderSize, borderSize), new Vector2f(oX + 2 * iX, oY + iY)));
            vertexArray.Append(new Vertex(new Vector2f(borderSize, borderSize), new Vector2f(oX + iX, oY + iY)));
            //Center
            vertexArray.Append(new Vertex(new Vector2f(borderSize, borderSize), new Vector2f(oX + iX, oY + iY)));
            vertexArray.Append(new Vertex(new Vector2f(size.X - borderSize, borderSize), new Vector2f(oX + 2 * iX, oY + iY)));
            vertexArray.Append(new Vertex(new Vector2f(borderSize, size.Y - borderSize), new Vector2f(oX + iX, oY + 2 * iY)));
            vertexArray.Append(new Vertex(new Vector2f(size.X - borderSize, borderSize), new Vector2f(oX + 2 * iX, oY + iY)));
            vertexArray.Append(new Vertex(new Vector2f(size.X - borderSize, size.Y - borderSize), new Vector2f(oX + 2 * iX, oY + 2 * iY)));
            vertexArray.Append(new Vertex(new Vector2f(borderSize, size.Y - borderSize), new Vector2f(oX + iX, oY + 2 * iY)));
            //Left
            vertexArray.Append(new Vertex(new Vector2f(0, borderSize), new Vector2f(oX, oY + iY)));
            vertexArray.Append(new Vertex(new Vector2f(borderSize, borderSize), new Vector2f(oX + iX, oY + iY)));
            vertexArray.Append(new Vertex(new Vector2f(0, size.Y - borderSize), new Vector2f(oX, oY + 2 * iY)));
            vertexArray.Append(new Vertex(new Vector2f(borderSize, borderSize), new Vector2f(oX + iX, oY + iY)));
            vertexArray.Append(new Vertex(new Vector2f(borderSize, size.Y - borderSize), new Vector2f(oX + iX, oY + 2 * iY)));
            vertexArray.Append(new Vertex(new Vector2f(0, size.Y - borderSize), new Vector2f(oX, oY + 2 * iY)));
            //Right
            vertexArray.Append(new Vertex(new Vector2f(size.X - borderSize, borderSize), new Vector2f(oX + 2 * iX, oY + iY)));
            vertexArray.Append(new Vertex(new Vector2f(size.X, borderSize), new Vector2f(oX + 3 * iX, oY + iY)));
            vertexArray.Append(new Vertex(new Vector2f(size.X - borderSize, size.Y - borderSize), new Vector2f(oX + 2 * iX, oY + 2 * iY)));
            vertexArray.Append(new Vertex(new Vector2f(size.X, borderSize), new Vector2f(oX + 3 * iX, oY + iY)));
            vertexArray.Append(new Vertex(new Vector2f(size.X, size.Y - borderSize), new Vector2f(oX + 3 * iX, oY + 2 * iY)));
            vertexArray.Append(new Vertex(new Vector2f(size.X - borderSize, size.Y - borderSize), new Vector2f(oX + 2 * iX, oY + 2 * iY)));
            //bottomleft
            vertexArray.Append(new Vertex(new Vector2f(0, size.Y - borderSize), new Vector2f(oX, oY + 2 * iY)));
            vertexArray.Append(new Vertex(new Vector2f(borderSize, size.Y - borderSize), new Vector2f(oX + iX, oY + 2 * iY)));
            vertexArray.Append(new Vertex(new Vector2f(0, size.Y), new Vector2f(oX, oY + 3 * iY)));
            vertexArray.Append(new Vertex(new Vector2f(borderSize, size.Y - borderSize), new Vector2f(oX + iX, oY + 2 * iY)));
            vertexArray.Append(new Vertex(new Vector2f(borderSize, size.Y), new Vector2f(oX + iX, oY + 3 * iY)));
            vertexArray.Append(new Vertex(new Vector2f(0, size.Y), new Vector2f(oX, oY + 3 * iY)));
            //bottomright
            vertexArray.Append(new Vertex(new Vector2f(size.X - borderSize, size.Y - borderSize), new Vector2f(oX + 2 * iX, oY + 2 * iY)));
            vertexArray.Append(new Vertex(new Vector2f(size.X, size.Y - borderSize), new Vector2f(oX + 3 * iX, oY + 2 * iY)));
            vertexArray.Append(new Vertex(new Vector2f(size.X - borderSize, size.Y), new Vector2f(oX + 2 * iX, oY + 3 * iY)));
            vertexArray.Append(new Vertex(new Vector2f(size.X, size.Y - borderSize), new Vector2f(oX + 3 * iX, oY + 2 * iY)));
            vertexArray.Append(new Vertex(new Vector2f(size.X, size.Y), new Vector2f(oX + 3 * iX, oY + 3 * iY)));
            vertexArray.Append(new Vertex(new Vector2f(size.X - borderSize, size.Y), new Vector2f(oX + 2 * iX, oY + 3 * iY)));
            //bottom
            vertexArray.Append(new Vertex(new Vector2f(borderSize, size.Y - borderSize), new Vector2f(oX + iX, oY + 2 * iY)));
            vertexArray.Append(new Vertex(new Vector2f(size.X - borderSize, size.Y - borderSize), new Vector2f(oX + 2 * iX, oY + 2 * iY)));
            vertexArray.Append(new Vertex(new Vector2f(borderSize, size.Y), new Vector2f(oX + iX, oY + 3 * iY)));
            vertexArray.Append(new Vertex(new Vector2f(size.X - borderSize, size.Y - borderSize), new Vector2f(oX + 2 * iX, oY + 2 * iY)));
            vertexArray.Append(new Vertex(new Vector2f(size.X - borderSize, size.Y), new Vector2f(oX + 2 * iX, oY + 3 * iY)));
            vertexArray.Append(new Vertex(new Vector2f(0, size.Y), new Vector2f(oX + iX, oY + 3 * iY)));

            return vertexArray;
        }

        public VertexArray CreateMenuGraphic(FloatRect bounds, Vector2f size)
        {
            VertexArray vertexArray = new VertexArray(PrimitiveType.Triangles);
            vertexArray.Append(new Vertex(new Vector2f(0, 0), new Vector2f(bounds.Left, bounds.Top)));
            vertexArray.Append(new Vertex(new Vector2f(size.X, 0), new Vector2f(bounds.Left + bounds.Width, bounds.Top)));
            vertexArray.Append(new Vertex(new Vector2f(0, size.Y), new Vector2f(bounds.Left, bounds.Top + bounds.Height)));
            vertexArray.Append(new Vertex(new Vector2f(size.X, 0), new Vector2f(bounds.Left + bounds.Width, bounds.Top)));
            vertexArray.Append(new Vertex(new Vector2f(size.X, size.Y), new Vector2f(bounds.Left + bounds.Width, bounds.Top + bounds.Height)));
            vertexArray.Append(new Vertex(new Vector2f(0, size.Y), new Vector2f(bounds.Left, bounds.Top + bounds.Height)));
            return vertexArray;
        }
        public VertexArray CreateMenuGraphic(FloatRect bounds, Vector2f size, Color color)
        {
            VertexArray vertexArray = new VertexArray(PrimitiveType.Triangles);
            vertexArray.Append(new Vertex(new Vector2f(0, 0), color, new Vector2f(bounds.Left, bounds.Top)));
            vertexArray.Append(new Vertex(new Vector2f(size.X, 0), color, new Vector2f(bounds.Left + bounds.Width, bounds.Top)));
            vertexArray.Append(new Vertex(new Vector2f(0, size.Y), color, new Vector2f(bounds.Left, bounds.Top + bounds.Height)));
            vertexArray.Append(new Vertex(new Vector2f(size.X, 0), color, new Vector2f(bounds.Left + bounds.Width, bounds.Top)));
            vertexArray.Append(new Vertex(new Vector2f(size.X, size.Y), color, new Vector2f(bounds.Left + bounds.Width, bounds.Top + bounds.Height)));
            vertexArray.Append(new Vertex(new Vector2f(0, size.Y), color, new Vector2f(bounds.Left, bounds.Top + bounds.Height)));
            return vertexArray;
        }
        public void Translate(Vector2f translation)
        {
            if (!lockedPosition)
            {
                position += translation;
            }
        }

        public void SetRelativePosition(Vector2f position)
        {
            this.position = position;
        }

        public void SetPivots(string pivot1, string pivot2, string inside, float margin)
        {
            this.pivot1 = pivot1;
            this.pivot2 = pivot2;
            this.pivot3 = inside;
            this.margin = margin;
        }

        virtual public void SetInitialPosition()
        {
            if(parent == null)
            {
                return;
            }
            if (pivot3 == "inside")
            {
                position = new Vector2f(parent.size.X / 2 - size.X / 2, parent.size.Y / 2 - size.Y / 2);
                if (pivot1 == "top" || pivot2 == "top")
                {
                    position += new Vector2f(0, size.Y / 2 + margin - parent.size.Y / 2);
                }
                if (pivot1 == "bottom" || pivot2 == "bottom")
                {
                    position += new Vector2f(0, parent.size.Y / 2 - size.Y / 2 - margin);
                }
                if (pivot1 == "left" || pivot2 == "left")
                {
                    position += new Vector2f(margin + size.X / 2 - parent.size.X / 2, 0);
                }
                if (pivot1 == "right" || pivot2 == "right")
                {
                    position += new Vector2f(parent.size.X / 2 - size.X / 2 - margin, 0);
                }
            }
            else
            {
                position = new Vector2f(parent.size.X / 2 - size.X / 2, parent.size.Y / 2 - size.Y / 2);
                if (pivot1 == "top" || pivot2 == "top")
                {
                    position += new Vector2f(0, -size.Y / 2 - margin - parent.size.Y / 2);
                }
                if (pivot1 == "bottom" || pivot2 == "bottom")
                {
                    position += new Vector2f(0, parent.size.Y / 2 + margin + size.Y / 2);
                }
                if (pivot1 == "left" || pivot2 == "left")
                {
                    position += new Vector2f(-size.X - parent.size.X / 2 - margin, 0);
                }
                if (pivot1 == "right" || pivot2 == "right")
                {
                    position += new Vector2f(parent.size.X / 2 + margin, 0);
                }
            }
        }

        virtual public void SetInitialPosition(View GUIView)
        {
            if(parent == null) {
                position = new Vector2f(GUIView.Size.X / 2 - size.X / 2, GUIView.Size.Y / 2 - size.Y / 2);
                if (pivot1 == "top" || pivot2 == "top")
                {
                    position += new Vector2f(0, -GUIView.Size.Y / 2 + size.Y / 2 + margin);
                }
                if (pivot1 == "bottom" || pivot2 == "bottom")
                {
                    position += new Vector2f(0, GUIView.Size.Y / 2 - size.Y / 2 - margin);
                }
                if (pivot1 == "left" || pivot2 == "left")
                {
                    position += new Vector2f(-GUIView.Size.X / 2 + size.X / 2 + margin, 0);
                }
                if (pivot1 == "right" || pivot2 == "right")
                {
                    position += new Vector2f(GUIView.Size.X / 2 - size.X / 2 - margin, 0);
                }
            }
            else if(pivot3 == "inside")
            {
                position = new Vector2f(parent.size.X / 2 - size.X / 2, parent.size.Y / 2 - size.Y / 2);
                if (pivot1 == "top" || pivot2 == "top")
                {
                    position += new Vector2f(0, size.Y/2 + margin - parent.size.Y/2);
                }
                if (pivot1 == "bottom" || pivot2 == "bottom")
                {
                    position += new Vector2f(0, parent.size.Y/2 - size.Y/2 - margin);
                }
                if (pivot1 == "left" || pivot2 == "left")
                {
                    position += new Vector2f(margin + size.X/2 - parent.size.X/2, 0);
                }
                if (pivot1 == "right" || pivot2 == "right")
                {
                    position += new Vector2f(parent.size.X/2 - size.X/2 - margin, 0);
                }
            }
            else
            {
                position = new Vector2f(parent.size.X / 2 - size.X / 2, parent.size.Y / 2 - size.Y / 2);
                if (pivot1 == "top" || pivot2 == "top")
                {
                    position += new Vector2f(0, -size.Y/2 - margin - parent.size.Y/2);
                }
                if (pivot1 == "bottom" || pivot2 == "bottom")
                {
                    position += new Vector2f(0, parent.size.Y/2 + margin + size.Y/2);
                }
                if (pivot1 == "left" || pivot2 == "left")
                {
                    position += new Vector2f(-size.X - parent.size.X/2 - margin, 0);
                }
                if (pivot1 == "right" || pivot2 == "right")
                {
                    position += new Vector2f(parent.size.X/2 + margin, 0);
                }
            }
        }

        /// <summary>
        /// Position the component relative to another component
        /// </summary>
        /// <param name="component"></param>
        public void SetInitialPosition(MenuComponent component)
        {
            Vector2f origin = new Vector2f(0, 0);
            MenuComponent bubble = component;
            while (bubble != null)
            {
                origin += bubble.position;
                bubble = bubble.parent;
            }
            Vector2f thisOrigin = new Vector2f(0, 0);
            bubble = this.parent;
            while (bubble != null)
            {
                thisOrigin += bubble.position;
                bubble = bubble.parent;
            }
            Vector2f relativeToThis = origin - thisOrigin;
            if (pivot3 == "inside")
            {
                position = new Vector2f(relativeToThis.X + component.size.X / 2 - size.X / 2, relativeToThis.Y + component.size.Y / 2 - size.Y / 2);
                if (pivot1 == "top" || pivot2 == "top")
                {
                    position += new Vector2f(0, size.Y + margin - component.size.Y / 2);
                }
                if (pivot1 == "bottom" || pivot2 == "bottom")
                {
                    position += new Vector2f(0, component.size.Y / 2 - size.Y - margin);
                }
                if (pivot1 == "left" || pivot2 == "left")
                {
                    position += new Vector2f(margin + size.X - component.size.X / 2, 0);
                }
                if (pivot1 == "right" || pivot2 == "right")
                {
                    position += new Vector2f(component.size.X / 2 - size.X - margin, 0);
                }
            }
            else
            {
                position = new Vector2f(relativeToThis.X + component.size.X / 2 - size.X / 2, relativeToThis.Y + component.size.Y / 2 - size.Y / 2);
                if (pivot1 == "top" || pivot2 == "top")
                {
                    position += new Vector2f(0, -size.Y - margin - component.size.Y / 2);
                }
                if (pivot1 == "bottom" || pivot2 == "bottom")
                {
                    position += new Vector2f(0, component.size.Y / 2 + margin + size.Y /2);
                }
                if (pivot1 == "left" || pivot2 == "left")
                {
                    position += new Vector2f(-size.X - component.size.X / 2 - margin, 0);
                }
                if (pivot1 == "right" || pivot2 == "right")
                {
                    position += new Vector2f(component.size.X / 2 + margin + size.X/2, 0);
                }
            }
        }

        public void AttachComponent(MenuComponent component)
        {
            component.parent = this;
            attachedComponents.Add(component);
        }
        /// <summary>
        /// Input starts at the top level menues and bubbles down to smaller components.  Origin represents the origin of the parent in screen coordinates,
        /// When combined with the components own relative positions gives and absolute position.
        /// </summary>
        /// <param name="input"></param>
        /// <param name="origin"></param>
        virtual public void HandleInput(InputManager input)
        {
            for (int i = attachedComponents.Count - 1; i >= 0; i--)
            {
                attachedComponents[i].HandleInput(input);
            }
        }

        public void SubscribeToInput(InputManager input)
        {
            input.AddInputSubscriber(this, true);
        }

        public void UnsubscribeToInput(InputManager input)
        {
            input.RemoveInputSubscriber(this, true);
        }
    }
}
