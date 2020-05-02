using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFML.Graphics;

namespace EngineeringCorpsCS
{
    class Machine:EntityPhysical
    {
        public enum MachineState
        {
            Idle,
            Working
        }

        MachineState machineState;
        Animation working; //This animation plays when working and stops when idle
        StaticSprite idle; //If this animation is assigned, then this will display when idle
        Animation shadow; //Shadow of the machine
        Recipe activeRecipe;
        int recipeProgress = 0;
        float workingSpeed = 1;
        int bufferAmount = 10;
        public List<ItemStack> result { get; set; }
        public List<ItemStack> input { get; set; }
        public LightSourceFlicker lightSourceFlicker;

        public Machine(string name, Animation working, StaticSprite idle, Animation shadow)
        {
            this.name = name;
            this.working = working;
            this.shadow = shadow;
            this.idle = idle;
            if (idle != null)
            {
                drawArray = new Drawable[] { idle, shadow };
            }
            else
            {
                drawArray = new Drawable[] { working, shadow };
            }
            machineState = MachineState.Idle;
            input = new List<ItemStack>();
            result = new List<ItemStack>();
        }

        public override void Update(EntityCollection entityCollection, ItemCollection itemCollection)
        {
            if (machineState == MachineState.Working)
            {
                if (recipeProgress >= activeRecipe.recipeTime)
                {
                    //Output products
                    for (int i = 0; i < activeRecipe.itemsResults.Length; i++)
                    {
                        if (result[i] == null)
                        {
                            result[i] = new ItemStack(itemCollection.GetItem(activeRecipe.itemsResults[i]), 0);
                        }
                        result[i].Add(activeRecipe.countsResult[i]);
                    }
                    recipeProgress = 0;
                    machineState = MachineState.Idle;
                    if (idle != null)
                    {
                        drawArray = new Drawable[] { idle, shadow };
                    }
                    if (lightSourceFlicker != null)
                    {
                        lightSourceFlicker.on = false;
                    }
                }
                else
                {
                    recipeProgress += (int)Math.Ceiling(workingSpeed);
                    working.Update();
                    shadow.currentFrame = working.currentFrame;
                }
            }
            if (machineState == MachineState.Idle)
            {
                //Check that machine has recipe
                if (activeRecipe != null)
                {
                    //Check that machine is not full
                    bool full = false;
                    for (int j = 0; j < activeRecipe.itemsResults.Length; j++)
                    {
                        if (result[j] != null && result[j].count >= bufferAmount)
                        {
                            full = true;
                        }
                    }
                    if (full == false)
                    {
                        //Check if input is valid
                        bool valid = true;
                        for (int i = 0; i < activeRecipe.itemsRequired.Length; i++)
                        {
                            if (input[i] == null || input[i].item.name != activeRecipe.itemsRequired[i] || input[i].count < activeRecipe.counts[i])
                            {
                                valid = false;
                            }
                        }
                        if (valid == true)
                        {
                            //Switch state to working and consume inputs
                            machineState = MachineState.Working;
                            drawArray = new Drawable[] { working, shadow };
                            for (int i = 0; i < activeRecipe.itemsRequired.Length; i++)
                            {
                                input[i] = input[i].Subtract(activeRecipe.counts[i]);
                            }
                            if(lightSourceFlicker != null)
                            {
                                lightSourceFlicker.on = true;
                            }
                        }
                    }
                }
            }
            
        }
        public override Entity Clone()
        {
            Machine clone = new Machine(this.name, this.working.Clone(), this.idle.Clone(), this.shadow.Clone());
            clone.drawingBox = new BoundingBox(this.drawingBox);
            clone.collisionBox = new BoundingBox(this.collisionBox);
            clone.selectionBox = new BoundingBox(this.selectionBox);
            clone.collisionMask = this.collisionMask;
            clone.minable = this.minable;
            clone.miningProps = this.miningProps;
            clone.mapColor = new Color(this.mapColor);
            clone.emissionPerSecond = this.emissionPerSecond;
            //Test light
            clone.lightSourceFlicker = this.lightSourceFlicker.Clone();
            return clone;
        }

        public override void InitializeEntity(Vector2 position, SurfaceContainer surface)
        {
            base.InitializeEntity(position, surface);
            lightSourceFlicker.on = false;
            lightSourceFlicker.Initialize(this);
        }

        public override void OnClick(Entity entity, MenuFactory menuFactory, RecipeCollection recipeCollection)
        {
            //create menu for machine
            if (entity is Player)
            {
                menuFactory.CreateMachineInterface(this, (Player)entity, recipeCollection);
            }
        }

        public override void OnMined(Player player, ItemCollection itemCollection, EntityCollection entityCollection)
        {
            surface.RemoveLightSource(this.lightSourceFlicker);
            for (int i = 0; i < input.Count; i++) {
                player.InsertIntoInventory(input[i], true);
            }
            for (int i = 0; i < result.Count; i++)
            {
                player.InsertIntoInventory(result[i], true);
            }
            base.OnMined(player, itemCollection, entityCollection);
        }

        public float GetProgress(string tag)
        {
            if (activeRecipe != null)
            {
                return recipeProgress * 1.0f / activeRecipe.recipeTime;
            }
            else
            {
                return 0.0f;
            }
        }

        public void ApplyRecipe(Recipe recipe, Player player)
        {
            activeRecipe = recipe;
            for(int i = 0; i < input.Count; i++)
            {
                player.InsertIntoInventory(input[i], true);
            }
            for (int j = 0; j < input.Count; j++)
            {
                player.InsertIntoInventory(result[j], true);
            }
            input.Clear();
            result.Clear();
            for(int i = 0; i < activeRecipe.itemsRequired.Length; i++)
            {
                input.Add(null);
            }
            for(int i = 0; i < activeRecipe.itemsResults.Length; i++)
            {
                result.Add(null);
            }
        }
    }
}
