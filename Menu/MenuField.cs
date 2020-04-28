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

        public string tag { get; set; } = "";
        ParseField parseField;
        string fieldValue;
        MenuText textField;
        FieldState fieldState;
        VertexArray fieldBackGround;
        RectangleShape cursor;
        int cursorCounter;
        int cursorRefresh;
        public MenuField(Vector2i componentSize, Font font, ParseField parseField)
        {
            Initialize(componentSize);
            textField = new MenuText(new Vector2i(size.X, size.Y), font, "", Convert.ToUInt32(size.Y - 4), size.Y);
            textField.SetTextPosition("left", "center");
            textField.SetPivots("center", "center", "inside", 2);
            this.parseField = parseField;
            fieldState = FieldState.Normal;
            collisionBox = new BoundingBox(size);
            fieldBackGround = CreateMenuGraphicArrayWithBorder(new FloatRect(96, 0, 96, 96), 10);
            cursor = new RectangleShape(new Vector2f(1, textField.charSize - 4));
            cursorCounter = 0;
            cursorRefresh = 30;
        }

        public override void Draw(RenderTexture gui, Vector2i origin, RenderStates guiState)
        {
            
            //Construct transform
            Transform t = new Transform(1, 0, 0, 0, 1, 0, 0, 0, 1);
            Vector2f pos = new Vector2f((position + origin).X, (origin + position).Y);
            t.Translate(pos);
            Transform original = guiState.Transform;
            guiState.Transform = t;
            gui.Draw(fieldBackGround, guiState);
            textField.Draw(gui, origin + position, guiState);
            if (cursorCounter > cursorRefresh)
            {
                if (fieldState == FieldState.Focused)
                {
                    cursor.Position = pos + textField.GetLastPosition() + new Vector2f(1, 4);
                    gui.Draw(cursor);
                }
                if(cursorCounter > 2 * cursorRefresh)
                {
                    cursorCounter = 0;
                }
            }
            base.Draw(gui, origin, guiState);
            guiState.Transform = original;
            cursorCounter++;
        }

        public override void HandleInput(InputManager input)
        {
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
            bool collided = BoundingBox.CheckPointMenuCollision(mousePos.X, mousePos.Y, collisionBox, new Vector2f((position + origin).X, (origin + position).Y));
            if (mouse && (fieldState == FieldState.Normal || fieldState == FieldState.Modified) && collided && input.GetMouseClicked(InputBindings.primary, true))
            {
                fieldState = FieldState.Focused;
            }
            if (fieldState == FieldState.Focused)
            {
                string updated;
                if (input.GetKeyPressed(InputBindings.backSpace, true))
                {
                    if (fieldValue.Length > 1)
                    {
                        fieldValue = fieldValue.Substring(0, fieldValue.Length - 1);
                        parseField(tag, fieldValue, out updated);
                        textField.SetText(updated);
                        fieldValue = updated;
                    }
                    else
                    {
                        fieldValue = "";
                        parseField(tag, fieldValue, out updated);
                        textField.SetText(fieldValue);
                        fieldValue = updated;
                    }
                }
                else
                {
                    string inputString = input.GetKeyString(true);
                    fieldValue = fieldValue + inputString;
                    if (inputString != "")
                    {
                        bool success = parseField.Invoke(tag, fieldValue, out updated);
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
