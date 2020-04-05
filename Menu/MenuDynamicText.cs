using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFML.System;
using SFML.Graphics;

namespace EngineeringCorpsCS
{
    class MenuDynamicText : MenuComponent
    {
        public delegate string DynamicString();
        DynamicString[] methodList;
        string[] stringArray;
        Text textComponent;
        char lineSplit = ' ';
        string textString;
        string unformattedString;

        /// <summary>
        /// Useful for displaying values that change frame to frame.
        /// </summary>
        /// <param name="relativePosition"></param>
        /// <param name="font"></param>
        /// <param name="text"></param>
        /// <param name="charSize"></param>
        /// <param name="methodList"></param>
        public MenuDynamicText(Vector2f componentSize, Font font, string text, uint charSize, DynamicString[] methodList)
        {
            Initialize(componentSize);
            this.methodList = methodList;
            textComponent = new Text("", font, charSize);
            textComponent.LineSpacing = 0.6f;
            unformattedString = text;
            stringArray = new string[methodList.Length];
        }

        override public void Draw(RenderTexture gui, Vector2f origin, RenderStates guiState)
        {
            EvaluateMethods();
            textString = string.Format(unformattedString, stringArray);
            ComputeSize();
            SetText(textString);
            textComponent.Position = position + origin;
            gui.Draw(textComponent);
            base.Draw(gui, origin, guiState);
        }

        public void SetText(string text)
        {
            textComponent.Position = new Vector2f(0, 0);
            textComponent.DisplayedString = text;
            uint lastlineSplit = 0;
            for (uint i = 0; i < textString.Length; i++)
            {
                if (textComponent.FindCharacterPos(i).X > size.X)
                {
                    for (uint j = i; j > lastlineSplit; j--)
                    {
                        if (textString[(int)j] == lineSplit)
                        {
                            textString = textString.Insert((int)(j) + 1, "\n");
                            Console.WriteLine(j + " : " + textComponent.FindCharacterPos(j).X + " : " + textString);
                            textComponent.DisplayedString = textString;
                            lastlineSplit = j;
                            break;
                        }
                        else if (j == lastlineSplit + 1)
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

        private void EvaluateMethods()
        {
            for(int i = 0; i < methodList.Length; i++)
            {
                stringArray[i] = methodList[i]();
            }
        }
    }
}
