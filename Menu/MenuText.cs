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
        bool textSet = false;
        char lineSplit = ' ';
        public MenuText(Vector2f relativePosition, Font font, string text, uint charSize)
        {
            this.position = relativePosition;
            textComponent = new Text("", font, charSize);
            textComponent.LineSpacing = 0.6f;
            textString = text;
        }

        override public void Draw(RenderTexture gui, Vector2f origin)
        {
            if(textSet == false)
            {
                ComputeSize();
                SetText(textString);
            }
            textComponent.Position = position + origin;
            gui.Draw(textComponent);
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
            uint lastlineSplit = 0;
            textSet = true;
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
                    for(uint j = i; j > lastlineSplit; j--)
                    {
                        if(textString[(int)j] == lineSplit)
                        {
                            textString = textString.Insert((int)(j) + 1, "\n");
                            Console.WriteLine(j + " : " + textComponent.FindCharacterPos(j).X + " : " + textString);
                            textComponent.DisplayedString = textString;
                            lastlineSplit = j;
                            break;
                        }
                        else if(j == lastlineSplit + 1)
                        {
                            textString = textString.Insert((int)(i - 2), "-\n");
                            Console.WriteLine(i + " : " + textComponent.FindCharacterPos(i).X + " : " + textString);
                            textComponent.DisplayedString = textString;
                            lastlineSplit = i + 1;
                            i++;
                            break;
                        }
                    }
                    
                }
            }
            size = new Vector2f(size.X, textComponent.FindCharacterPos(Convert.ToUInt32(textComponent.DisplayedString.Length)).Y);
        }

        private void ComputeSize()
        {
            this.size = new Vector2f(parent.size.X - 2 * margin - position.X, parent.size.Y - 2 * margin - position.Y);
        }
    }
}
