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
        public string textString { get; protected set; }
        List<Text> textComponents;
        Vector2f[] textPositions;
        public float lineSpacing { get; set; }
        char lineSplit = ' ';
        Font font;
        string textPivot1 = "center";
        string textPivot2 = "center";
        public uint charSize { get; protected set; }
        public MenuText(Vector2f componentSize, Font font, string text, uint charSize, float lineSpacing)
        {
            Initialize(componentSize);
            textComponents = new List<Text>();
            this.lineSpacing = lineSpacing;
            this.font = font;
            this.charSize = charSize;
            SetText(text);
        }

        override public void Draw(RenderTexture gui, Vector2f origin, RenderStates guiState)
        {
            for(int i = 0; i < textComponents.Count; i++)
            {
                if (textPositions == null)
                {
                    textComponents[i].Position = position + origin + new Vector2f(0, i * 12.0f);
                }
                else
                {
                    textComponents[i].Position = position + origin + textPositions[i];
                }
                gui.Draw(textComponents[i]);
            }
            base.Draw(gui, origin, guiState);
        }

        public void SetTextPosition(string textPivot, string textPivotOther)
        {
            textPivot1 = textPivot;
            textPivot2 = textPivotOther;
            //do not change the position of the text component, but instead the positions of the text objects contained
            float centerX = size.X / 2;
            float centerY = size.Y / 2;
            int components = textComponents.Count;
            textPositions = new Vector2f[components];
            for (int i = 0; i < components; i++)
            {
                textPositions[i] = new Vector2f(size.X / 2, size.Y / 2);
                textComponents[i].Origin = new Vector2f(textComponents[i].GetLocalBounds().Width / 2.0f, charSize / 2.0f);
                if (textPivot1 == "center" || textPivot2 == "center")
                {
                    textPositions[i] += new Vector2f(0, (i - (components - 1)/2.0f) * charSize - charSize/8);
                }
                if (textPivot1 == "top" || textPivot2 == "top")
                {
                    textComponents[i].Origin += new Vector2f(0, -textComponents[i].GetLocalBounds().Height / 2.0f);
                    textPositions[i] += new Vector2f(0, (i * charSize) - size.Y / 2 + margin);
                }
                if (textPivot1 == "bottom" || textPivot2 == "bottom") {
                    textComponents[i].Origin += new Vector2f(0, -textComponents[i].GetLocalBounds().Height / 2.0f);
                    textPositions[i] += new Vector2f(0, -((components - i) * charSize) + size.Y / 2 - margin);
                }
                if (textPivot1 == "left" || textPivot2 == "left") 
                {
                    textComponents[i].Origin += new Vector2f(-textComponents[i].GetLocalBounds().Width / 2.0f, 0);
                    textPositions[i] += new Vector2f(-size.X / 2 + margin, 0);
                }
                if (textPivot1 == "right" || textPivot2 == "right")
                {
                    textComponents[i].Origin += new Vector2f(textComponents[i].GetLocalBounds().Width / 2.0f, 0) ;
                    textPositions[i] += new Vector2f(size.X - margin, 0);
                }
            }
        }

        /// <summary>
        /// Change the text contained by the menu text component
        /// </summary>
        /// <param name="text"></param>
        public void SetText(string text)
        {
            textComponents.Clear();
            textString = text;
            textComponents.Add(new Text(textString, font, charSize));
            textComponents[0].Position = new Vector2f(0, 0);
            textComponents[0].DisplayedString = text;
            uint lastlineSplit = 0;
            int currentComponent = 0;

            for(uint i = 0; i < textString.Length; i++)
            {
                if(textComponents[currentComponent].FindCharacterPos(i - lastlineSplit).X + charSize/2 > size.X)
                {
                    for(uint j = i; j > lastlineSplit; j--)
                    {
                        if(textString[(int)j].Equals(lineSplit))
                        {
                            textComponents[currentComponent].DisplayedString = textString.Substring((int)lastlineSplit, (int)j - (int)lastlineSplit).Trim();
                            textComponents.Add(new Text(textString.Substring((int)j).Trim(), font, charSize));
                            currentComponent++;
                            lastlineSplit = j;
                            break;
                        }
                        if(j == lastlineSplit + 1)
                        {
                            string newstring = textString.Substring((int)lastlineSplit, (int)i - (int)lastlineSplit).Trim();
                            textComponents[currentComponent].DisplayedString = newstring.Insert(newstring.Length, "-");
                            textComponents.Add(new Text(textString.Substring((int)i).Trim(), font, charSize));
                            currentComponent++;
                            lastlineSplit = i;
                            break;
                        }
                    }
                }
            }
            SetTextPosition(textPivot1, textPivot2);
        }

        public void ComputeSize()
        {
            this.size = new Vector2f(parent.size.X - 2 * margin - position.X, parent.size.Y - 2 * margin - position.Y);
        }

        public Vector2f GetLastPosition()
        {
            return textPositions[textPositions.Length-1] + new Vector2f(textComponents[textPositions.Length -1].GetLocalBounds().Width, 0) - textComponents[textPositions.Length - 1].Origin;
        }
    }
}
