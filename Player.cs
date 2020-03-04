using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFML.Graphics; //remove this later
using SFML.System;

namespace EngineeringCorpsCS
{
    class Player: Entity, IInputSubscriber
    {
        Vector2 velocity;
        public RotatedAnimation playerTest;
        float rotation = 0;
        public Player(Vector2 pos, SurfaceContainer surface, TextureContainer textureContainer)
        {
            position = pos;
            collisionBox = new BoundingBox(-64, -64, 64, 64);
            surface.InitiateEntityInChunks(this);
            velocity = new Vector2(0, 0);
            Texture[] playerTextures = new Texture[] { textureContainer.GetTexture("orcrunning") };
            playerTest = new RotatedAnimation(playerTextures, new Vector2i(128, 128), 8, 8, "Forward", 12.0f);
        }
        /// <summary>
        /// TODO: Add inheritance structure
        /// </summary>
        public void Update()
        {
            int[] chunkList = BoundingBox.GetChunkBounds(collisionBox, position);
            for (int i = 0; i < chunkList.Length; i++)
            {
                List<Entity> collisionList = surface.GetChunk(chunkList[i]).entityCollisionList;
                for (int j = 0; j < collisionList.Count; j++)
                {
                    //TODO: Add collision mask check
                    if (!collisionList[j].Equals(this))
                    {
                        Vector2 pushBack;
                        if (BoundingBox.CheckCollision(collisionBox, collisionList[j].collisionBox, position, velocity, collisionList[j].position, out pushBack))
                        {
                            velocity.Add(pushBack);
                        }
                    }
                }
            }
            surface.UpdateEntityInChunks(this, velocity);
            position.Add(velocity);
            if (velocity.x != 0 || velocity.y != 0)
            {
                if (velocity.GetRotation() != rotation)
                {
                    
                    rotation = velocity.GetRotation();
                    Console.WriteLine(rotation);
                    playerTest.SetRotation(rotation);

                }
            }
            
            playerTest.Update();
            playerTest.GetAnimationFrame().Position = new Vector2f(position.x, position.y);
            playerTest.SetAnimationSpeed(velocity.GetMagnitude()/2);
            velocity.Set(0, 0);
        }
        override public void OnClick()
        {

        }

        public void SubscribeToInput(InputManager input)
        {
            input.AddInputSubscriber(this, false);
        }
        public void UnsubscribeToInput(InputManager input)
        {
            input.RemoveInputSubscriber(this, false);
        }
        public void HandleInput(InputManager input)
        {
            if (input.keyHeld[InputBindings.moveUp])
            {
                velocity.Add(0, -10);
            }
            if (input.keyHeld[InputBindings.moveDown])
            {
                velocity.Add(0, 10);
            }
            if (input.keyHeld[InputBindings.moveLeft])
            {
                velocity.Add(-10, 0);
            }
            if(input.keyHeld[InputBindings.moveRight])
            {
                velocity.Add(10, 0);
            }
        }
    }
}
