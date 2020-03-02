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
        View view = new View(new FloatRect(0, 0, 2 * 1280, 2 * 720));
        float viewScale = 1.0f;
        public Vector2f moveVector;
        public Camera()
        {
            focusedEntity = new Player(new Vector2(256, 256));
            //focusedEntity.collisionBox.SetRotation(150);
        }
        public void Update()
        {
            moveVector = new Vector2f(0,0);
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
            if (false) //&& BoundingBox.CheckTerrainTileCollision(focusedEntity.collisionBox, new Vector2(view.Center.X + moveVector.X, view.Center.Y + moveVector.Y), Base.CollisionLayer.TerrainSolid))
            {
                //moveVector = new Vector2f(0, 0);
            }
            else
            {
                
            }
            view.Center = new Vector2f(focusedEntity.position.x, focusedEntity.position.y);
        }
        public View GetView()
        {
            return view;
        }
    }
}
