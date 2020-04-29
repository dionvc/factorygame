using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFML.Graphics;
using SFML.System;

namespace EngineeringCorpsCS
{
    class MenuInventory : MenuComponent
    {
        List<ItemStack> inventory;
        VertexArray itemFrames;
        BoundingBox frameBox;
        Player accessingPlayer;
        Entity accessedEntity;
        MenuText itemCount;
        MenuText itemName;
        int hoveredIndex = -1;
        Vector2f hoveredPosition;
        public bool allowInsertion = true;
        public MenuInventory(Vector2i componentSize, List<ItemStack> inventory, Player accessingPlayer, Entity accessedEntity, Font font)// TextureContainer textureContainer)
        {
            this.inventory = inventory;
            this.size = componentSize;
            this.accessingPlayer = accessingPlayer;
            this.accessedEntity = accessedEntity;

            frameBox = new BoundingBox(0, 0, 32, 32);
            itemCount = new MenuText(new Vector2i(32, 32), font, "", 16, 0);
            itemName = new MenuText(new Vector2i(128, 32), font, "", 16, 0);
        }

        public override void Draw(RenderTexture gui, Vector2i origin, RenderStates guiState)
        {
            RectangleShape frame = new RectangleShape(new Vector2f(32, 32));
            frame.FillColor = Color.Red;
            Vector2f pos = new Vector2f((position + origin).X, (origin + position).Y);
            for (int i = 0; i < inventory.Count; i++)
            {
                frame.Position = pos + new Vector2f((i * 32) % (int)size.X, ((i * 32) / (int)size.X) * 32);
                gui.Draw(frame);
                if (inventory[i] != null)
                {
                    Sprite itemSprite = inventory[i].item.itemSprite.GetSprite();
                    itemSprite.Position = pos + new Vector2f((i * 32) % (int)size.X, (i * 32) / (int)size.X * 32);
                    itemCount.SetText(inventory[i].count.ToString());
                    itemCount.SetTextPosition("right", "bottom");
                    itemCount.SetRelativePosition(position + new Vector2i((i * 32) % (int)size.X - 16, (i * 32) / (int)size.X * 32));
                    gui.Draw(itemSprite);
                    itemCount.Draw(gui, origin, guiState);
                }
            }
            if(hoveredIndex != -1 && inventory[hoveredIndex] != null)
            {
                itemName.SetText(inventory[hoveredIndex].item.name);
                itemName.SetTextPosition("left", "center");
                RectangleShape backdrop = new RectangleShape(new Vector2f(itemName.size.X, itemName.size.Y));
                backdrop.FillColor = new Color(64, 64, 64, 208);
                backdrop.Position = hoveredPosition;
                gui.Draw(backdrop);
                itemName.SetRelativePosition(new Vector2i((int)hoveredPosition.X, (int)hoveredPosition.Y));
                itemName.Draw(gui, new Vector2i(0,0), guiState);
            }
            base.Draw(gui, origin, guiState);
        }

        public override void HandleInput(InputManager input)
        {
            if(accessedEntity.surface == null || accessingPlayer.surface == null)
            {
                MenuComponent bubbleTest = parent;
                while(bubbleTest.parent != null)
                {
                    bubbleTest = bubbleTest.parent;
                }
                input.menuContainer.RemoveMenu(bubbleTest);
            }
            base.HandleInput(input);
            Vector2f mousePos;
            bool mouse = input.GetMousePosition(out mousePos);
            Vector2i origin = new Vector2i(0, 0);
            MenuComponent bubble = parent;
            while (bubble != null)
            {
                origin += bubble.position;
                bubble = bubble.parent;
            }
            Vector2f pos = new Vector2f((position + origin).X, (origin + position).Y);
            if (mouse)
            {
                bool anyCollision = false;
                for (int i = 0; i < inventory.Count; i++)
                {
                    bool collided = BoundingBox.CheckPointMenuCollision(mousePos.X, mousePos.Y, frameBox, pos + new Vector2f((i * 32) % (int)size.X, ((i * 32) / (int)size.X) * 32));
                    if(collided)
                    {
                        anyCollision = true;
                        hoveredIndex = i;
                        hoveredPosition = mousePos;
                        if (input.GetMouseClicked(InputBindings.primary, false))
                        {
                            if (accessingPlayer.heldItem != null && inventory[i] != null && ReferenceEquals(accessingPlayer.heldItem.item, inventory[i].item)) //attempt to combine
                            {
                                input.GetMouseClicked(InputBindings.primary, true);
                                int total = inventory[i].count + accessingPlayer.heldItem.count;
                                if (total <= inventory[i].item.maxStack)
                                {
                                    inventory[i].Add(accessingPlayer.heldItem.count);
                                    accessingPlayer.heldItem = null;
                                }
                                else
                                {
                                    inventory[i].SetCount(inventory[i].item.maxStack);
                                    accessingPlayer.heldItem.SetCount(total - inventory[i].item.maxStack);
                                }
                            }
                            else if(accessingPlayer.heldItem != null && allowInsertion) //swap item
                            {
                                input.GetMouseClicked(InputBindings.primary, true);
                                ItemStack temp = inventory[i];
                                inventory[i] = accessingPlayer.heldItem;
                                accessingPlayer.heldItem = temp;
                            }
                            else if(accessingPlayer.heldItem == null)
                            {
                                input.GetMouseClicked(InputBindings.primary, true);
                                accessingPlayer.heldItem = inventory[i];
                                inventory[i] = null;

                            }
                        }
                    }
                }
                if(anyCollision == false)
                {
                    hoveredIndex = -1;
                }
            }
            if (mouse && input.GetMouseClicked(InputBindings.secondary, true))
            {
                for (int i = 0; i < inventory.Count; i++)
                {
                    bool collided = BoundingBox.CheckPointMenuCollision(mousePos.X, mousePos.Y, frameBox, pos + new Vector2f((i * 32) % (int)size.X, ((i * 32) / (int)size.X) * 32));
                    if (collided)
                    {
                        if(accessingPlayer.heldItem != null && inventory[i] == null)
                        {
                            int half = accessingPlayer.heldItem.count / 2;
                            inventory[i] = new ItemStack(accessingPlayer.heldItem.item, half);
                            accessingPlayer.heldItem.Subtract(half);
                        }
                        else if(inventory[i] != null && accessingPlayer.heldItem == null)
                        {
                            int half = inventory[i].count / 2;
                            inventory[i].Subtract(half);
                            accessingPlayer.heldItem = new ItemStack(inventory[i].item, half);
                        }
                    }
                }
            }
        }
    }
}
