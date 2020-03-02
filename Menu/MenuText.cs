using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFML.Graphics;
using SFML.System;

namespace EngineeringCorpsCS
{
    class MenuText : MenuComponent
    {
        string textString;
        Text textComponent;
        public MenuText(Vector2f relativePosition, Vector2f componentSize, bool[] sizeScaling, Font font, string text, uint charSize)
        {
            this.position = relativePosition;
            textComponent = new Text("", font, charSize);
            this.size = componentSize;
            this.SetText(text);

        }

        override public void Draw(RenderTexture gui, Vector2f origin)
        {
            textComponent.Position = position + origin;
            gui.Draw(textComponent);
        }

        override public void Translate(Vector2f translation)
        {
            position += translation;
        }

        /// <summary>
        /// Change the text contained by the menu text component
        /// </summary>
        /// <param name="text"></param>
        public void SetText(string text)
        {
            textComponent.Position = new Vector2f(0, 0);
            textString = text;
            textComponent.DisplayedString = text;
            if(fixedWidth == false)
            {
                //change the width of the text component to reflect its variable width
                size = new Vector2f(textComponent.FindCharacterPos(Convert.ToUInt32(textComponent.DisplayedString.Length)).X, size.Y);
                return;
            }
            for (uint i = 0; i < textString.Length; i++)
            {
                if (textComponent.FindCharacterPos(i).X > size.X)
                {
                    textString = textString.Insert((int)i, "\n");
                    Console.WriteLine(i + " : " + textComponent.FindCharacterPos(i).X + " : " + textString);
                    textComponent.DisplayedString = textString;
                }
            }
            size = new Vector2f(size.X, textComponent.FindCharacterPos(Convert.ToUInt32(textComponent.DisplayedString.Length)).Y);
        }
    }
}
