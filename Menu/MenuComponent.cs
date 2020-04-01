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

        //unused for now
        public bool fixedWidth = true;
        public bool fixedHeight = false;
        public bool lockedPosition = false;
        public string pivot1 = "center";
        public string pivot2 = "center";
        public string pivot3 = "inside";
        public float margin = 0.0f; //specify margin of attachment.  If attaching to a component, then the sign matters.
        protected Vector2f scale;

        virtual public void Draw(RenderTexture gui, Vector2f origin)
        {
            for (int i = 0; i < attachedComponents.Count; i++)
            {
                attachedComponents[i].Draw(gui, origin + position);
            }
        }
        public void Translate(Vector2f translation)
        {
            if (!lockedPosition)
            {
                position += translation;
            }
        }

        public void SetInitialPosition(View GUIView)
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
                position = new Vector2f(parent.position.X + parent.size.X / 2 - size.X / 2, parent.position.Y + parent.size.Y / 2 - size.Y / 2);
                if (pivot1 == "top" || pivot2 == "top")
                {
                    position += new Vector2f(0, size.Y + margin - parent.size.Y/2);
                }
                if (pivot1 == "bottom" || pivot2 == "bottom")
                {
                    position += new Vector2f(0, parent.size.Y/2 - size.Y - margin);
                }
                if (pivot1 == "left" || pivot2 == "left")
                {
                    position += new Vector2f(margin + size.X - parent.size.X/2, 0);
                }
                if (pivot1 == "right" || pivot2 == "right")
                {
                    position += new Vector2f(parent.size.X/2 - size.X - margin, 0);
                }
            }
            else
            {
                position = new Vector2f(parent.position.X + parent.size.X / 2 - size.X / 2, parent.position.Y + parent.size.Y / 2 - size.Y / 2);
                if (pivot1 == "top" || pivot2 == "top")
                {
                    position += new Vector2f(0, -size.Y - margin - parent.size.Y/2);
                }
                if (pivot1 == "bottom" || pivot2 == "bottom")
                {
                    position += new Vector2f(0, parent.size.Y/2 + margin);
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
            for (int i = 0; i < attachedComponents.Count; i++)
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
