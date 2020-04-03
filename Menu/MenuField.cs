using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFML.System;
using SFML.Graphics;

namespace EngineeringCorpsCS
{
    class MenuField : MenuComponent
    {
        enum FieldState
        {
            Normal,
            Focused,
            Modified
        }
        public delegate bool ParseField(string tag, string value, out string updated);
        
        public string tag { get; set; }
        ParseField parseField;
        string fieldValue;
        MenuText textField;
        FieldState fieldState;
        public MenuField(Vector2f relativePosition, Vector2f componentSize, Font font, ParseField parseField)
        {
            Initialize(relativePosition, componentSize);
            textField = new MenuText(new Vector2f(0, 0), new Vector2f(100, 100), font, "", 24, 24);
            this.parseField = parseField;
            fieldState = FieldState.Normal;
            collisionBox = new BoundingBox(size);
        }

        public override void Draw(RenderTexture gui, Vector2f origin, RenderStates guiState)
        {
            RectangleShape testBack = new RectangleShape(size);
            testBack.Position = origin + position;
            if(fieldState == FieldState.Focused)
            {
                testBack.FillColor = Color.Cyan;
            }
            else if(fieldState == FieldState.Modified)
            {
                testBack.FillColor = Color.Blue;
            }
            gui.Draw(testBack);
            textField.Draw(gui, origin + position, guiState);
            base.Draw(gui, origin, guiState);
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
            bool collided = BoundingBox.CheckPointMenuCollision(mousePos.X, mousePos.Y, collisionBox, position + origin);
            if ((fieldState == FieldState.Normal || fieldState == FieldState.Modified) && collided && input.GetMouseClicked(InputBindings.primary, true))
            {
                fieldState = FieldState.Focused;
            }
            if (fieldState == FieldState.Focused)
            {
                if (input.GetKeyPressed(InputBindings.backSpace, true))
                {
                    if (fieldValue.Length > 1)
                    {
                        fieldValue = fieldValue.Substring(0, fieldValue.Length - 1);
                        textField.SetText(fieldValue);
                    }
                    else
                    {
                        fieldValue = "";
                        textField.SetText(fieldValue);
                    }
                }
                else
                {
                    string inputString = input.GetKeyString(true);
                    string updated;
                    fieldValue = fieldValue + inputString;
                    if (inputString != "" && parseField.Invoke(tag, fieldValue, out updated))
                    {
                        textField.SetText(updated);
                        fieldValue = updated;
                    }
                }
            }
            if(fieldState == FieldState.Focused && !collided && input.GetMouseClickedIgnoreConsume(InputBindings.primary))
            {
                fieldState = FieldState.Modified;
            }
        }
    }
}
