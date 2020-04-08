﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFML.Graphics; //remove this later
using SFML.System;

namespace EngineeringCorpsCS
{
    class Player: EntityPhysical, IInputSubscriber
    {
        Vector2 velocity;
        float rotation = 0;
        ItemStack[] inventory;
        public ItemStack heldItem { get; set; } = null;
        EntityGhost heldItemGhost;
        TextureContainer textureContainer;
        public Player(Vector2 pos, SurfaceContainer surface, TextureContainer textureContainer)
        {
            this.textureContainer = textureContainer;
            position = pos;
            collisionBox = new BoundingBox(16, 16);
            surface.InitiateEntityInChunks(this);
            velocity = new Vector2(0, 0);
            Texture[] playerTextures = new Texture[] { textureContainer.GetTexture("orcrunning") };
            AnimationRotated walking = new AnimationRotated(playerTextures, new Vector2i(128, 128), new Vector2f(0, 0), new Vector2f(0, 0), new Vector2f(1, 1), 8, 8, "Forward", 0.0f);
            walking.behavior = AnimationRotated.AnimationBehavior.Forward;
            drawArray = new Drawable[] { walking };
            collisionMask = CollisionLayer.EntityPhysical | CollisionLayer.TerrainSolid;
            mapColor = Color.Magenta;

            inventory = new ItemStack[10];
            for (int i = 0; i < inventory.Length - 2; i++)
            {
                inventory[i] = new ItemStack();
            }
        }
        /// <summary>
        /// TODO: Add inheritance structure
        /// </summary>
        public void Update()
        {
            ///TODO: Move this
            BoundingBox.ApplyPhysicalCollision(this, velocity);
            if (velocity.x != 0 || velocity.y != 0)
            {
                rotation = velocity.GetRotation() + 180.0f;
            }
            drawArray[0].SetRotation(rotation);
            drawArray[0].Update();
            drawArray[0].SetAnimationSpeed(60/velocity.GetMagnitude());
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
            if (input.GetKeyHeld(InputBindings.moveUp, false))
            {
                velocity.Add(0, -8);
            }
            if (input.GetKeyHeld(InputBindings.moveDown, false))
            {
                velocity.Add(0, 8);
            }
            if (input.GetKeyHeld(InputBindings.moveLeft, false))
            {
                velocity.Add(-8, 0);
            }
            if(input.GetKeyHeld(InputBindings.moveRight, false))
            {
                velocity.Add(8, 0);
            }

            if(input.GetKeyPressed(InputBindings.showInventory, true))
            {
                input.menuFactory.CreateTestInventory(this, inventory);
            }

            if(heldItem != null && input.GetMouseClicked(InputBindings.primary, true))
            {
                float[] mousePos = input.GetMousePositionAsFloat();
                int[] tileAligned = new int[] { (int)(mousePos[0] - mousePos[0] % Props.tileSize + 16), (int)(mousePos[1] - mousePos[1] % Props.tileSize + 16) };
                BoundingBox box = new BoundingBox(-15, -15, 15, 15);
                EntityGhost entityGhost = new EntityGhost(box, new Vector2(tileAligned[0], tileAligned[1]), surface);
                if (!BoundingBox.CheckForCollision(entityGhost))
                {
                    new Tree(new Vector2(tileAligned[0], tileAligned[1]), surface, textureContainer);
                    //new Player(new Vector2(tileAligned[0], tileAligned[1]), surface, textureContainer);
                }
            }

            if(input.GetMouseClicked(InputBindings.secondary, true))
            {
                float[] mousePos = input.GetMousePositionAsFloat();
                int[] tileAligned = new int[] { (int)(mousePos[0] - mousePos[0] % Props.tileSize + 16), (int)(mousePos[1] - mousePos[1] % Props.tileSize + 16) };
                BoundingBox box = new BoundingBox(-15, -15, 15, 15);
                EntityGhost entityGhost = new EntityGhost(box, new Vector2(tileAligned[0], tileAligned[1]), surface);
                List<EntityPhysical> list = BoundingBox.GetCollisionListOfType<EntityPhysical>(entityGhost);
                if (list.Count > 0)
                {
                    surface.RemoveEntity(list[0]);
                }
            }
        }
    }
}
