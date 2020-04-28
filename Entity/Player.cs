using System;
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
        int playerVelocity = 12;
        ItemStack[] inventory;
        public ItemStack heldItem { get; set; } = null;
        EntityGhost heldItemGhost;
        public EntityPhysical selectedEntity { get; protected set; } = null;
        public int miningProgress;
        public float selectionRange = 100.0f;
        TextureAtlases textureAtlases;
        AnimationRotated walking;
        LightSourceDirectional directionalLight;
        LightSourceRadial radialLight;
        public Player(TextureAtlases textureAtlases, string name)
        {
            this.name = name;
            this.textureAtlases = textureAtlases;
            collisionBox = new BoundingBox(16, 16);
            drawingBox = new BoundingBox(-64, -64, 64, 64);
            velocity = new Vector2(0, 0);
            IntRect bounds;
            Texture playerTexture = textureAtlases.GetTexture("orcrunning", out bounds);
            walking = new AnimationRotated(playerTexture, 128, 128, bounds, new Vector2f(0, -32), 8, 8);
            walking.drawLayer = Drawable.DrawLayer.EntitySorted;
            walking.behavior = AnimationRotated.AnimationBehavior.Forward;
            drawArray = new Drawable[] { walking };
            collisionMask = CollisionLayer.EntityPhysical | CollisionLayer.TerrainSolid;
            mapColor = Color.Magenta;
        }
        /// <summary>
        /// TODO: Add inheritance structure
        /// </summary>
        public override void Update(EntityCollection entityCollection, ItemCollection itemCollection)
        {
            ///TODO: Move this
            BoundingBox.ApplyPhysicalCollision(this, velocity);
            if (velocity.x != 0 || velocity.y != 0)
            {
                rotation = velocity.GetRotation() + 180.0f;
            }
            walking.SetRotation(rotation);
            walking.Update();
            walking.animationSpeed = 60/velocity.GetMagnitude();
            velocity.Set(0, 0);
            radialLight.Update();
            directionalLight.Update();
            directionalLight.SetDirection(270 + rotation);
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
            if(inventory == null)
            {
                inventory = new ItemStack[10];
                for (int i = 0; i < inventory.Length - 2; i++)
                {
                    inventory[i] = new ItemStack(input.itemCollection.GetItem("Pine Sapling"), 100);
                }
                inventory[8] = new ItemStack(input.itemCollection.GetItem("Greenhouse"), 10);
            }

            //Movement
            if (input.GetKeyHeld(InputBindings.moveUp, false))
            {
                velocity.Add(0, -playerVelocity);
            }
            if (input.GetKeyHeld(InputBindings.moveDown, false))
            {
                velocity.Add(0, playerVelocity);
            }
            if (input.GetKeyHeld(InputBindings.moveLeft, false))
            {
                velocity.Add(-playerVelocity, 0);
            }
            if(input.GetKeyHeld(InputBindings.moveRight, false))
            {
                velocity.Add(playerVelocity, 0);
            }

            //Open Inventory
            if(input.GetKeyPressed(InputBindings.showInventory, true))
            {
                input.menuFactory.CreatePlayerInventory(this, inventory);
            }
            Vector2f mousePosf;
            BoundingBox box = new BoundingBox(-2, -2, 2, 2);
            bool mouse = input.GetMousePosition(out mousePosf);
            if (mouse) {
                List<EntityPhysical> list = BoundingBox.CheckSelectionOfType<EntityPhysical>(new Vector2(mousePosf), box, surface);
                if (list.Count > 0)
                {
                    if (!ReferenceEquals(list[0], selectedEntity))
                    {
                        miningProgress = 0;
                        selectedEntity = list[0];
                    }
                }
                else
                {
                    miningProgress = 0;
                    selectedEntity = null;
                }
            }
            else
            {
                selectedEntity = null;
            }
            //Check for onClick
            if (selectedEntity != null && input.GetMouseClicked(InputBindings.primary, true))
            {
                selectedEntity.OnClick(this, input.menuFactory);
                input.ConsumeMousePosition();
            }
            float[] mousePos;
            bool mousefloat = input.GetMousePositionAsFloat(out mousePos);
            //Check for placement ability
            if (mousefloat && heldItem != null && heldItem.item.placeResult != null && input.GetMouseHeld(InputBindings.primary, true))
            {
                int[] tileAligned = new int[] { (int)(mousePos[0] - mousePos[0] % Props.tileSize + 16), (int)(mousePos[1] - mousePos[1] % Props.tileSize + 16) };
                BoundingBox placeBox = new BoundingBox(-15, -15, 15, 15);
                //Use prototype animation to construct entityghost
                EntityGhost entityGhost = new EntityGhost(placeBox, new Vector2(mousePos[0], mousePos[1]), surface);
                if (!BoundingBox.CheckForCollision(entityGhost))
                {
                    Entity placeItem = input.entityCollection.InstantiatePrototype(heldItem.item.placeResult, new Vector2(mousePos[0], mousePos[1]), surface);
                    heldItem = heldItem.Subtract(1);
                }
            }

            
            //Check for mining potential (Switch to using states and update?)
            if(input.GetMousePositionAsFloat(out mousePos) && input.GetMouseHeld(InputBindings.secondary, true))
            {
                if(selectedEntity != null)
                {
                    miningProgress += 1;
                    if(miningProgress > selectedEntity.miningProps.miningTime)
                    {
                        input.entityCollection.DestroyInstance(selectedEntity);
                        selectedEntity = null;
                        miningProgress = 0;
                        Item item = input.itemCollection.GetItem("Wood");
                        for(int i = 0; i < inventory.Length; i++)
                        {
                            if (inventory[i] == null)
                            {
                                inventory[i] = new ItemStack(item, 1);
                                break;
                            }
                            else if (ReferenceEquals(inventory[i].item, item) && inventory[i].count <= inventory[i].item.maxStack)
                            {
                                inventory[i].Add(1);
                                break;
                            }
                        }
                    }
                }
            }
        }

        public float GetProgress(string tag)
        {
            if(tag.Equals("mining"))
            {
                if (selectedEntity != null) {
                    return miningProgress * 1.0f / selectedEntity.miningProps.miningTime;
                }
                else
                {
                    return 0.0f;
                }
            }

            return 0.0f;
        }

        public override Entity Clone()
        {
            Player newPlayer = new Player(this.textureAtlases, this.name);
            newPlayer.selectionBox = new BoundingBox(this.selectionBox);
            return newPlayer;
        }

        public override void InitializeEntity(Vector2 position, SurfaceContainer surface)
        {
            base.InitializeEntity(position, surface);
            IntRect bounds;
            radialLight = new LightSourceRadial(surface, 256.0f, textureAtlases.GetTexture("lightsource", out bounds), bounds);
            radialLight.on = true;
            radialLight.attachedEntity = this;
            directionalLight = new LightSourceDirectional(surface, 2000.0f, 1024, textureAtlases.GetTexture("directionallight", out bounds), bounds);
            directionalLight.on = true;
            directionalLight.attachedEntity = this;
        }
    }
}
