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
        //need functionality for holding items
        //store held item with camera
        //versus
        //store held item with player <- makes most sense as then placement mechanics would be tied to player
        //edge cases would be what if player is in vehicle?
        //so this type of inventory would require a player then?
        //what about an entity?
        //what if instead we just attach the camera to every inventory, and if the item is clicked then it can be transferred
        //to the focused entity?
        //downside -- ties camera to game functionality

        //Idea: pass in accessing player
        //fixes tying camera to game functionality.
        //makes sense, as for an inventory to appear, a player must access it
        //clicking on the inventory slot transfers the item to the player's held item
        //can create an Ingame entity to represent the player's held item, which can switch to being drawn on the menu layer
        //versus the world with its attached drawable
        //But if the player has a held item, it needs to be able to be passed back to the inventory

        ItemStack[] inventory;
        VertexArray itemFrames;
        BoundingBox frameBox;
        Player accessingPlayer;
        Entity accessedEntity;
        MenuText itemCount;
        public MenuInventory(Vector2i componentSize, ItemStack[] inventory, Player accessingPlayer, Entity accessedEntity, Font font)// TextureContainer textureContainer)
        {
            this.inventory = inventory;
            this.size = componentSize;
            this.accessingPlayer = accessingPlayer;
            this.accessedEntity = accessedEntity;
            //Need textureContainer to draw item icons
            //Need
            for(int i = 0; i < inventory.Length; i++)
            {
                //Construct inventory frames
            }
            frameBox = new BoundingBox(0, 0, 32, 32);
            itemCount = new MenuText(new Vector2i(32, 32), font, "", 16, 0);
        }

        public override void Draw(RenderTexture gui, Vector2i origin, RenderStates guiState)
        {
            base.Draw(gui, origin, guiState);
            RectangleShape frame = new RectangleShape(new Vector2f(32, 32));
            frame.FillColor = Color.Red;
            Vector2f pos = new Vector2f((position + origin).X, (origin + position).Y);
            for (int i = 0; i < inventory.Length; i++)
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
            if (mouse && input.GetMouseClicked(InputBindings.primary, false))
            {
                for(int i = 0; i < inventory.Length; i++)
                {
                    bool collided = BoundingBox.CheckPointMenuCollision(mousePos.X, mousePos.Y, frameBox, pos + new Vector2f((i * 32) % (int)size.X, ((i * 32) / (int)size.X) * 32));
                    if(collided)
                    {
                        if (accessingPlayer.heldItem != null && inventory[i] != null && ReferenceEquals(accessingPlayer.heldItem.item, inventory[i].item)) //attempt to combine
                        {
                            int total = inventory[i].count + accessingPlayer.heldItem.count;
                            if(total <= inventory[i].item.maxStack)
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
                        else //swap item
                        {
                            input.GetMouseClicked(InputBindings.primary, true);
                            ItemStack temp = inventory[i];
                            inventory[i] = accessingPlayer.heldItem;
                            accessingPlayer.heldItem = temp;
                        }
                    }

                }
            }
            if (mouse && input.GetMouseClicked(InputBindings.secondary, true))
            {
                for (int i = 0; i < inventory.Length; i++)
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
