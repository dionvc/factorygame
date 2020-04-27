using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFML.System;
using SFML.Graphics;

namespace EngineeringCorpsCS
{
    class MenuSlider: MenuComponent
    {
        //The slider's visual and underlying data value are coupled so the value of the slider should be calculated
        //by visual appearance.  This is contrary to the rest of the game's design, where visuals are a result of the data.
        enum SliderState
        {
            Normal,
            Hover,
            Sliding,
            Released
        }
        public delegate void ApplySlider(string tag, float value);
        ApplySlider sliderAction;
        float sliderValue = 0.0f; //value relative to componentSize (default I guess can be halfway)
        float sliderMax = 2.0f; //The underlying range.  This means that a slider at halfway visually would have half this value.
        float sliderMin = 0.5f;
        SliderState sliderState;
        VertexArray[] sliderArrays;
        VertexArray sliderArray;
        VertexArray sliderArrayHover;
        VertexArray sliderArraySliding;
        public string tag { get; set; }
        public MenuSlider(int segments, ApplySlider sliderAction, float sliderMin, float sliderMax, float sliderInitialValue)
        {
            Vector2f componentSize = new Vector2f(32 * segments, 32);
            Initialize(componentSize);
            this.sliderMin = sliderMin;
            this.sliderMax = sliderMax;
            sliderValue = ((sliderInitialValue)/(sliderMax - sliderMin) * size.X/2);
            sliderState = SliderState.Normal;
            collisionBox = new BoundingBox(componentSize);
            this.sliderAction = sliderAction;
            sliderArrays = new VertexArray[segments];
            for(int i = 0; i < segments; i ++)
            {
                if(i == 0)
                {
                    sliderArrays[i] = CreateMenuGraphic(new FloatRect(0, 96, 32, 32), new Vector2f(32, 32));
                }
                else if(i == segments - 1)
                {
                    sliderArrays[i] = CreateMenuGraphic(new FloatRect(64, 96, 32, 32), new Vector2f(32, 32));
                }
                else
                {
                    sliderArrays[i] = CreateMenuGraphic(new FloatRect(32, 96, 32, 32), new Vector2f(32, 32));
                }
            }
            sliderArray = CreateMenuGraphic(new FloatRect(96, 96, 32, 32), new Vector2f(32, 32));
            sliderArrayHover = CreateMenuGraphic(new FloatRect(96, 96, 32, 32), new Vector2f(32, 32), new Color(192, 255, 192, 255));
            sliderArraySliding = CreateMenuGraphic(new FloatRect(96, 96, 32, 32), new Vector2f(32, 32), new Color(128, 255, 128, 255));
        }

        public override void Draw(RenderTexture gui, Vector2f origin, RenderStates guiState)
        {
            //draw the slider (regular, hightlighted, and pressed)
            //draw the backdrop of slider (details?)
            Transform original = guiState.Transform;
            Transform t = new Transform(1, 0, 0, 0, 1, 0, 0, 0, 1);
            t.Translate(origin + position);
            for (int i = 0; i < sliderArrays.Length; i++)
            {
                guiState.Transform = t;
                gui.Draw(sliderArrays[i], guiState);
                t.Translate(new Vector2f(32, 0));
            }
            t = new Transform(1, 0, 0, 0, 1, 0, 0, 0, 1);
            t.Translate(origin + position + new Vector2f(sliderValue - 16, -4));
            guiState.Transform = t;
            if(sliderState == SliderState.Normal)
            {
                gui.Draw(sliderArray, guiState);
            }
            else if(sliderState == SliderState.Hover)
            {
                gui.Draw(sliderArrayHover, guiState);
            }
            else if(sliderState == SliderState.Sliding)
            {
                gui.Draw(sliderArraySliding, guiState);
            }
            guiState.Transform = original;
            base.Draw(gui, origin, guiState);
        }

        public override void HandleInput(InputManager input)
        {
            base.HandleInput(input);
            Vector2f mousePos;
            bool mouse = input.GetMousePosition(out mousePos);
            Vector2f origin = new Vector2f(0, 0);
            MenuComponent bubble = parent;
            while (bubble != null)
            {
                origin += bubble.position;
                bubble = bubble.parent;
            }

            if (mouse && sliderState != SliderState.Sliding && BoundingBox.CheckPointMenuCollision(mousePos.X, mousePos.Y, collisionBox, (position + origin)))
            {
                sliderState = SliderState.Hover;
            }
            else if(sliderState != SliderState.Sliding)
            {
                sliderState = SliderState.Normal;
            }
            if(sliderState == SliderState.Hover && input.GetMouseClicked(InputBindings.primary, true))
            {
                sliderState = SliderState.Sliding;
            }
            if(sliderState == SliderState.Sliding && input.GetMouseReleased(InputBindings.primary, true))
            {
                sliderState = SliderState.Released;
                sliderAction?.Invoke(tag, sliderMax * sliderValue/size.X + sliderMin);
            }

            if(sliderState == SliderState.Sliding)
            {
                sliderValue = mousePos.X - origin.X - position.X;
                sliderValue = sliderValue < 0 ? 0 : sliderValue;
                sliderValue = sliderValue > size.X ? size.X : sliderValue;
            }
        }
    }
}
