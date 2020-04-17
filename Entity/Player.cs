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
        ItemStack[] inventory;
        public ItemStack heldItem { get; set; } = null;
        EntityGhost heldItemGhost;
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

            inventory = new ItemStack[10];
            for (int i = 0; i < inventory.Length - 2; i++)
            {
                inventory[i] = new ItemStack();
            }
            
        }
        /// <summary>
        /// TODO: Add inheritance structure
        /// </summary>
        public override void Update()
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

            if(heldItem != null && input.GetMouseHeld(InputBindings.primary, true))
            {
                float[] mousePos = input.GetMousePositionAsFloat();
                int[] tileAligned = new int[] { (int)(mousePos[0] - mousePos[0] % Props.tileSize + 16), (int)(mousePos[1] - mousePos[1] % Props.tileSize + 16) };
                BoundingBox box = new BoundingBox(-15, -15, 15, 15);
                EntityGhost entityGhost = new EntityGhost(box, new Vector2(mousePos[0], mousePos[1]), surface);
                if (!BoundingBox.CheckForCollision(entityGhost))
                {
                    Entity tree = input.entityCollection.InstantiatePrototype("pineTree1", new Vector2(mousePos[0], mousePos[1]), surface);
                }
            }

            if(input.GetMouseHeld(InputBindings.secondary, true))
            {
                float[] mousePos = input.GetMousePositionAsFloat();
                //int[] tileAligned = new int[] { (int)(mousePos[0] - mousePos[0] % Props.tileSize + 16), (int)(mousePos[1] - mousePos[1] % Props.tileSize + 16) };
                BoundingBox box = new BoundingBox(-15, -15, 15, 15);
                EntityGhost entityGhost = new EntityGhost(box, new Vector2(mousePos[0], mousePos[1]), surface);
                List<Entity> list = BoundingBox.GetCollisionListOfType<Entity>(entityGhost);
                if (list.Count > 0 && list[0].minable == true)
                {
                    surface.RemoveEntity(list[0]);
                }
            }
        }

        public override Entity Clone()
        {
            return new Player(this.textureAtlases, this.name);
        }

        public override void InitializeEntity(Vector2 position, SurfaceContainer surface)
        {
            base.InitializeEntity(position, surface);
            IntRect bounds;
            radialLight = new LightSourceRadial(new Vector2(1024, 1024), surface, 256.0f, textureAtlases.GetTexture("lightsource", out bounds), bounds);
            radialLight.on = true;
            radialLight.attachedEntity = this;
            directionalLight = new LightSourceDirectional(new Vector2(1024, 1024), surface, 2000.0f, 1024, textureAtlases.GetTexture("directionallight", out bounds), bounds);
            directionalLight.on = true;
            directionalLight.attachedEntity = this;
        }
    }
}
