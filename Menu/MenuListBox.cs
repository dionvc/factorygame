using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFML.System;
using SFML.Graphics;

namespace EngineeringCorpsCS
{
    class MenuListBox : MenuComponent
    {
        enum ListBoxState
        {
            Normal, //closed with selected value visible
            Focused //Open with selections visible
        }
        public delegate void SelectionMethod(string header, int value);
        string[] listHeaders;
        int[] listValues;
        MenuText[] texts;
        MenuText selectedText;
        int selectedValue = 0; //in terms of index of listHeaders/values
        int hoveredValue = 0; //in terms of index of listHeaders/values
        BoundingBox dropdownBox;
        float lineSpacing;
        ListBoxState listboxState;
        SelectionMethod applySelection;

        /// <summary>
        /// A drop down menu implementation.  Very quick if you are selecting from a range of enums.
        /// </summary>
        /// <param name="relativePosition"></param>
        /// <param name="componentSize"></param>
        /// <param name="listHeaders"></param>
        /// <param name="listValues"></param>
        public MenuListBox(Vector2f relativePosition, Vector2f componentSize, string[] listHeaders, int[] listValues, SelectionMethod applySelection, Font font, uint charSize, float lineSpacing, int initialValue)
        {
            Initialize(relativePosition, componentSize);
            this.listHeaders = listHeaders;
            this.listValues = listValues;
            this.applySelection = applySelection;
            this.lineSpacing = lineSpacing;
            texts = new MenuText[listHeaders.Length];
            selectedText = new MenuText(new Vector2f(0,-2), this.size, font, "", charSize, 0.6f);
            selectedValue = initialValue;
            selectedText.SetText(listHeaders[initialValue]);
            for (int i = 0; i < listHeaders.Length; i++)
            {
                texts[i] = new MenuText(new Vector2f(0, 0), this.size, font, listHeaders[i], charSize, 0.6f);
                AttachComponent(texts[i]);
                texts[i].SetText(listHeaders[i]);
                texts[i].SetRelativePosition(new Vector2f(0, (i + 1) * lineSpacing - 2));
            }
            dropdownBox = new BoundingBox(new Vector2f(size.X, lineSpacing));
            collisionBox = new BoundingBox(size);
            listboxState = ListBoxState.Normal;
        }

        public override void Draw(RenderTexture gui, Vector2f origin)
        {
            //Draw the top component field and text header
            RectangleShape box = new RectangleShape(size);
            box.Position = position;
            box.FillColor = Color.Red;
            gui.Draw(box);
            selectedText.Draw(gui, origin + position);
            //Draw the dropdown button
            //If is focused:
            if (listboxState == ListBoxState.Focused)
            {
                for (int i = 0; i < listHeaders.Length; i++)
                {
                    RectangleShape otherbox = new RectangleShape(size);
                    otherbox.Position = new Vector2f(position.X, position.Y + size.Y + i * lineSpacing);
                    otherbox.FillColor = Color.Yellow;
                    gui.Draw(otherbox);
                    texts[i].Draw(gui, origin + position);
                }
            }
            //Draw the dropdown transparent selector
            //Draw the highlight over currently hovered value
            //Draw the text for the values
        }

        public override void HandleInput(InputManager input)
        {
            base.HandleInput(input);
            //Computing absolute position to check collision
            Vector2f mousePos = input.GetMousePosition();
            Vector2f origin = new Vector2f(0, 0);
            MenuComponent bubble = parent;
            while (bubble != null)
            {
                origin += bubble.position;
                bubble = bubble.parent;
            }
            bool collided = BoundingBox.CheckPointMenuCollision(mousePos.X, mousePos.Y, collisionBox, position + origin);

            if (listboxState == ListBoxState.Normal && collided && input.GetMouseClicked(InputBindings.primary, true))
            {
                listboxState = ListBoxState.Focused;
                hoveredValue = selectedValue;
            }
            if (listboxState == ListBoxState.Focused)
            {
                for (int i = 0; i < listHeaders.Length; i++)
                {
                    //check collision for each list header
                    if (BoundingBox.CheckPointMenuCollision(mousePos.X, mousePos.Y, dropdownBox, position + new Vector2f(0, lineSpacing * (i + 1))))
                    {
                        hoveredValue = i;
                        collided = true;
                    }
                }
                if(input.GetMouseClicked(InputBindings.primary, false))
                {
                    //don't process a click if it was on the listbox
                    if(collided)
                    {
                        input.GetMouseClicked(InputBindings.primary, true);
                    }
                    selectedValue = hoveredValue;
                    applySelection?.Invoke(listHeaders[selectedValue], listValues[selectedValue]);
                    selectedText.SetText(listHeaders[selectedValue]);
                    listboxState = ListBoxState.Normal;
                }
            }
        }
    }
}
