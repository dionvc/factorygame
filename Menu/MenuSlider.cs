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
        delegate void ApplySlider(float value);
        ApplySlider sliderAction;
        float sliderValue = 0.0f; //value relative to componentSize (default I guess can be halfway)
        float sliderRange = 100.0f; //The underlying range.  This means that a slider at halfway visually would have half this value.
        SliderState sliderState;
        public MenuSlider(Vector2f relativePosition, Vector2f componentSize)
        {
            Initialize(relativePosition, componentSize);
            sliderState = SliderState.Normal;
            collisionBox = new BoundingBox(componentSize);
        }

        public override void Draw(RenderTexture gui, Vector2f origin)
        {
            //draw the slider (regular, hightlighted, and pressed)
            //draw the backdrop of slider (details?)
            RectangleShape back = new RectangleShape(size);
            back.Position = origin + position;
            if(sliderState == SliderState.Hover)
            {
                back.FillColor = Color.Blue;
            }
            RectangleShape slider = new RectangleShape(new Vector2f(32, 64));
            slider.Position = origin + position + new Vector2f(sliderValue, 0);
            gui.Draw(back);
            gui.Draw(slider);
            base.Draw(gui, origin);
        }

        public override void HandleInput(InputManager input)
        {
            base.HandleInput(input);
            Vector2f mousePos = input.GetMousePosition();
            Vector2f origin = new Vector2f(0, 0);
            MenuComponent bubble = parent;
            while (bubble != null)
            {
                origin += bubble.position;
                bubble = bubble.parent;
            }

            if (sliderState != SliderState.Sliding && BoundingBox.CheckPointMenuCollision(mousePos.X, mousePos.Y, collisionBox, (position + origin)))
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
                sliderAction?.Invoke(sliderRange * sliderValue/size.X);
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
