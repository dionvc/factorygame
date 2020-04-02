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
        public delegate bool ParseField(string tag, string value, out string updated);

        public string tag { get; set; }
        ParseField parseField;
        string fieldValue;
        MenuText textField;
        public MenuField(Font font, ParseField parseField)
        {
            textField = new MenuText(new Vector2f(0, 0), new Vector2f(100, 100), font, "", 24, 24);
            this.parseField = parseField;
        }

        public override void Draw(RenderTexture gui, Vector2f origin)
        {
            base.Draw(gui, origin);
            textField.Draw(gui, origin + position);
        }

        public override void HandleInput(InputManager input)
        {
            base.HandleInput(input);
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
}
