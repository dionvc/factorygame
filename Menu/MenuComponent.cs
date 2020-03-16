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
        public bool fixedWidth = true;
        public bool fixedHeight = false;
        protected Vector2f scale;
        public BoundingBox collisionBox { get; protected set; }
        public List<MenuComponent> attachedComponents { get; protected set; } = new List<MenuComponent>();

        virtual public void Draw(RenderTexture gui, Vector2f origin)
        {
            for (int i = 0; i < attachedComponents.Count; i++)
            {
                attachedComponents[i].Draw(gui, origin + position);
            }
        }
        public void Translate(Vector2f translation)
        {
            position += translation;
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
            input.AddInputSubscriber(this, false);
        }

        public void UnsubscribeToInput(InputManager input)
        {
            input.RemoveInputSubscriber(this, false);
        }
    }
}
