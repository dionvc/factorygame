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
        float viewScale = 1.0f;
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
            if (Mouse.IsButtonPressed(Mouse.Button.Left) || Mouse.IsButtonPressed(Mouse.Button.Right))
            {
                if(Mouse.IsButtonPressed(Mouse.Button.Left))
                {
                    viewScale -= 0.5f;
                }
                else
                {
                    viewScale += 0.5f;
                }
                if(viewScale < 0.5)
                {
                    viewScale = 0.5f;
                }
                else if (viewScale > 4)
                {
                    viewScale = 4.0f;
                }
                
                view.Size = new SFML.System.Vector2f(viewScale * 1280, viewScale * 720);
            }
            focusedEntity.position = new Vector2(view.Center.X, view.Center.Y);
        }
        public View GetView()
        {
            return view;
        }
    }
}
