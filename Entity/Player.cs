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
        public enum PlayerState
        {
            Idle,
            Moving,
            Mining
        }
        PlayerState playerState = PlayerState.Idle;
        Vector2 velocity;
        float rotation = 0;
        int playerVelocity = 12;
        List<ItemStack> inventory;
        List<ItemStack> dropItems = new List<ItemStack>();
        public ItemStack heldItem { get; set; } = null;
        EntityGhost heldItemGhost;
        public Entity selectedEntity { get; protected set; } = null;
        public int miningProgress;
        public int miningSoundFrequency = 30;
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
            for (int i = 0; i < dropItems.Count; i++)
            {
                for (int j = 0; j < dropItems[i].count; j++) {
                    entityCollection.InstantiatePrototype(dropItems[i].item.name, position.Copy(), surface);
                }
            }
            dropItems.Clear();
            if (playerState == PlayerState.Mining)
            {
                EntityPhysical entity = selectedEntity as EntityPhysical;
                if (entity != null)
                {
                    if (miningProgress % miningSoundFrequency == 0)
                    {
                        StaticSoundManager.PlaySound(entity.position, entity.miningSounds);
                    }
                    miningProgress += 1;
                    if (miningProgress > entity.miningProps.miningTime)
                    {
                        StaticSoundManager.PlaySound(entity.position, new string[] { "Pickup" });
                        entity.OnMined(this, itemCollection, entityCollection);
                        selectedEntity = null;
                        miningProgress = 0;
                        playerState = PlayerState.Idle;
                    }
                }
                else
                {
                    playerState = PlayerState.Idle;
                }
            }
            if (playerState == PlayerState.Moving)
            {
                ///TODO: Move this
                BoundingBox.ApplyPhysicalCollision(this, velocity);
                if (velocity.x != 0 || velocity.y != 0)
                {
                    rotation = velocity.GetRotation() + 180.0f;
                }
                walking.SetRotation(rotation);
                walking.Update();
                walking.animationSpeed = 60 / velocity.GetMagnitude();
                velocity.Set(0, 0);
                radialLight.Update();
                directionalLight.Update();
                directionalLight.SetDirection(270 + rotation);
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
        public void HandleInput(InputManager input)
        {
            //TODO: Debug code (move dis)
            if(inventory == null)
            {
                inventory = new List<ItemStack>();
                for (int i = 0; i < 10; i++)
                {
                    inventory.Add(new ItemStack(input.itemCollection.GetItem("Pine Sapling"), 100));
                }
                inventory[8] = new ItemStack(input.itemCollection.GetItem("Greenhouse"), 10);
            }

            //Movement
            if (playerState == PlayerState.Idle || playerState == PlayerState.Moving)
            {
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
                if (input.GetKeyHeld(InputBindings.moveRight, false))
                {
                    velocity.Add(playerVelocity, 0);
                }
                playerState = PlayerState.Moving;
            }

            //Determining Selected Entity
            Vector2f mousePosf;
            BoundingBox box = new BoundingBox(-2, -2, 2, 2);
            bool mouse = input.GetMousePosition(out mousePosf);
            if (mouse) {
                List<Entity> list = BoundingBox.CheckSelectionOfType<Entity>(new Vector2(mousePosf), box, surface);
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

            float[] mousePos;
            bool mousefloat = input.GetMousePositionAsFloat(out mousePos);
            EntityPhysical entity = selectedEntity as EntityPhysical;
            //Switch to mining state if suitable
            if (entity != null && input.GetMousePositionAsFloat(out mousePos) && input.GetMouseHeld(InputBindings.secondary, true) && entity.minable == true)
            {
                playerState = PlayerState.Mining;
            }
            EntityItem itemCheck = selectedEntity as EntityItem;
            if (itemCheck != null && input.GetMousePositionAsFloat(out mousePos) && input.GetMouseHeld(InputBindings.secondary, true))
            {
                ItemStack leftover = InsertIntoInventory(new ItemStack(input.itemCollection.GetItem(itemCheck.name), 1), false);
                if (leftover == null)
                {
                    input.entityCollection.DestroyInstance(itemCheck);
                }
            }

            //Check for onClick
            if (entity != null && selectedEntity is EntityPhysical && input.GetMouseClicked(InputBindings.primary, true))
            {
                entity.OnClick(this, input.menuFactory, input.recipeCollection);
                input.ConsumeMousePosition();
            }

            //Check for placement ability
            if (mousefloat && heldItem != null)
            {
                if (heldItem.item.placeResult != null && input.GetMouseHeld(InputBindings.primary, true))
                {
                    Entity prototype = input.entityCollection.GetPrototype(heldItem.item.placeResult);
                    float[] tileAligned;
                    if (prototype.tileAligned == true)
                    {
                        tileAligned = new float[] { (int)(mousePos[0] - mousePos[0] % Props.tileSize + 16), (int)(mousePos[1] - mousePos[1] % Props.tileSize + 16) };
                    }
                    else
                    {
                        tileAligned = new float[] { mousePos[0], mousePos[1] };
                    }
                    BoundingBox placeBox = new BoundingBox(prototype.collisionBox);
                    //Use prototype animation to construct entityghost
                    if (!BoundingBox.CheckForPlacementCollision(placeBox, new Vector2(tileAligned[0], tileAligned[1]), surface, prototype.collisionMask))
                    {
                        Entity placeItem = input.entityCollection.InstantiatePrototype(heldItem.item.placeResult, new Vector2(tileAligned[0], tileAligned[1]), surface);
                        heldItem = heldItem.Subtract(1);
                    }
                }
                //Check for dropping item
                if (input.GetKeyPressed(InputBindings.dropItem, true))
                {
                    input.entityCollection.InstantiatePrototype(heldItem.item.name, new Vector2(mousePos[0], mousePos[1]), surface);
                    heldItem = heldItem.Subtract(1);
                }
            }

            //Open Inventory
            if (input.GetKeyPressed(InputBindings.showInventory, true))
            {
                input.menuFactory.CreatePlayerInventory(this, inventory);
            }

            //Return item to inventory
            if (input.GetKeyPressed(InputBindings.returnItem, true) && heldItem != null)
            {
                ItemStack leftover = InsertIntoInventory(heldItem, false);
                if(leftover == null)
                {
                    heldItem = null;
                }
            }
        }

        public float GetProgress(string tag)
        {
            if(tag.Equals("mining"))
            {
                EntityPhysical entity = selectedEntity as EntityPhysical;
                if (entity != null && entity.minable == true) {
                    return miningProgress * 1.0f / entity.miningProps.miningTime;
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

        /// <summary>
        /// Inserts into inventory. Returns leftovers if unsuccessful.
        /// </summary>
        /// <param name="itemStack"></param>
        /// <returns></returns>
        public ItemStack InsertIntoInventory(ItemStack itemStack, bool dropIfFull)
        {
            for (int i = 0; i < inventory.Count; i++)
            {
                if (inventory[i] == null)
                {
                    inventory[i] = itemStack;
                    return null;
                }
                else if (ReferenceEquals(inventory[i].item, itemStack.item) && inventory[i].count <= inventory[i].item.maxStack)
                {
                    int leftover = inventory[i].Add(itemStack.count);
                    if (leftover > 0)
                    {
                        itemStack.SetCount(leftover);
                    }
                    else
                    {
                        return null;
                    }
                }
            }
            if(dropIfFull)
            {
                dropItems.Add(itemStack);
                return null;
            }
            return itemStack;
        }
    }
}
