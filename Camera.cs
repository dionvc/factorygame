using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFML.Window;
using SFML.Graphics;

namespace EngineeringCorpsCS
{
    class Camera
    {
        public Player focusedEntity;
        View view = new View(new FloatRect(0, 0, 2 * 1280, 2 * 720));

        public Camera()
        {
            focusedEntity = new Player();
        }
        public void Update()
        {
            
            if (Keyboard.IsKeyPressed(Keyboard.Key.W))
            {
                view.Move(new SFML.System.Vector2f(0, -10));
            }
            if (Keyboard.IsKeyPressed(Keyboard.Key.A))
            {
                view.Move(new SFML.System.Vector2f(-10, 0));
            }
            if (Keyboard.IsKeyPressed(Keyboard.Key.S))
            {
                view.Move(new SFML.System.Vector2f(0, 10));
            }
            if (Keyboard.IsKeyPressed(Keyboard.Key.D))
            {
                view.Move(new SFML.System.Vector2f(10, 0));
            }
        }
        public View GetView()
        {
            return view;
        }
    }
}
