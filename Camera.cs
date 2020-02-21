using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFML.Window;
using SFML.Graphics;
using SFML.System;

namespace EngineeringCorpsCS
{
    class Camera
    {
        public Player focusedEntity;
        View view = new View(new FloatRect(50000, 50000, 2 * 1280, 2 * 720));
        float viewScale = 1.0f;
        public Camera()
        {
            focusedEntity = new Player();
            focusedEntity.position = new Vector2(50000, 50000);
            focusedEntity.collisionBox.SetRotation(150);
        }
        public void Update()
        {
            Vector2f moveVector = new Vector2f(0,0);
            if (Keyboard.IsKeyPressed(Keyboard.Key.W))
            {
                moveVector = new Vector2f(0, -10);
            }
            if (Keyboard.IsKeyPressed(Keyboard.Key.A))
            {
                moveVector = new Vector2f(moveVector.X - 10, moveVector.Y + 0);
            }
            if (Keyboard.IsKeyPressed(Keyboard.Key.S))
            {
                moveVector = new Vector2f(moveVector.X + 0, moveVector.Y + 10);
            }
            if (Keyboard.IsKeyPressed(Keyboard.Key.D))
            {
                moveVector = new Vector2f(moveVector.X + 10, moveVector.Y - 0);
            }
            if (Keyboard.IsKeyPressed(Keyboard.Key.R))
            {
                focusedEntity.collisionBox.SetRotation(focusedEntity.collisionBox.GetRotation() + 5);
            }
            if (Mouse.IsButtonPressed(Mouse.Button.Left) || Mouse.IsButtonPressed(Mouse.Button.Right))
            {
                if(Mouse.IsButtonPressed(Mouse.Button.Left))
                {
                    viewScale -= 0.25f;
                }
                else
                {
                    viewScale += 0.25f;
                }
                if(viewScale < 0.5)
                {
                    viewScale = 0.5f;
                }
                else if (viewScale > 8)
                {
                    viewScale = 8.0f;
                }
                
                view.Size = new SFML.System.Vector2f(viewScale * 1280, viewScale * 720);
            }
            if (BoundingBox.CheckTileCollision(focusedEntity.collisionBox, new Vector2(view.Center.X + moveVector.X, view.Center.Y + moveVector.Y)))
            {
                moveVector = new Vector2f(0, 0);
            }
            else
            {
                focusedEntity.position = new Vector2(view.Center.X + moveVector.X, view.Center.Y + moveVector.Y);
            }
            view.Move(moveVector);
        }
        public View GetView()
        {
            return view;
        }
    }
}
