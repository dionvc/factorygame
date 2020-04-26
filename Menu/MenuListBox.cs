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
        public delegate void SelectionMethod(string addtionalParam, int value);
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
        VertexArray dropdownMainBox;
        VertexArray dropdownArrow;
        /// <summary>
        /// Additional param that will be passed with the selection
        /// </summary>
        public string tag { get; set; } = "";

        /// <summary>
        /// A drop down menu implementation.  Very quick if you are selecting from a range of enums.
        /// </summary>
        /// <param name="relativePosition"></param>
        /// <param name="componentSize"></param>
        /// <param name="listHeaders"></param>
        /// <param name="listValues"></param>
        public MenuListBox(Vector2f componentSize, string[] listHeaders, int[] listValues, SelectionMethod applySelection, Font font, uint charSize, float lineSpacing, int initialValue)
        {
            Initialize(componentSize);
            this.listHeaders = listHeaders;
            this.listValues = listValues;
            this.applySelection = applySelection;
            this.lineSpacing = lineSpacing;
            texts = new MenuText[listHeaders.Length];
            selectedText = new MenuText(this.size, font, "", charSize, 0.6f);
            selectedText.SetRelativePosition(new Vector2f(0, -2));
            selectedValue = initialValue;
            selectedText.SetText(listHeaders[initialValue]);
            selectedText.SetTextPosition("left", "center");
            for (int i = 0; i < listHeaders.Length; i++)
            {
                texts[i] = new MenuText(this.size, font, listHeaders[i], charSize, 0.6f);
                AttachComponent(texts[i]);
                texts[i].SetText(listHeaders[i]);
                texts[i].SetTextPosition("left", "center");
                texts[i].SetRelativePosition(new Vector2f(0, (i + 1) * (lineSpacing + 1) - 2));
            }
            dropdownBox = new BoundingBox(new Vector2f(size.X, lineSpacing));
            collisionBox = new BoundingBox(size + new Vector2f(size.Y, 0));
            listboxState = ListBoxState.Normal;

            dropdownMainBox = CreateMenuGraphicArrayWithBorder(new FloatRect(96, 0, 96, 96), 10);
            dropdownArrow = CreateMenuGraphic(new FloatRect(416, 0, 96, 96), new Vector2f(size.Y, size.Y));
        }

        public override void Draw(RenderTexture gui, Vector2f origin, RenderStates guiState)
        {
            //Draw the top component field and text header
            Transform t = new Transform(1, 0, 0, 0, 1, 0, 0, 0, 1);
            t.Translate(origin + position);
            Transform original = guiState.Transform;
            guiState.Transform = t;
            gui.Draw(dropdownMainBox, guiState);
            t = new Transform(1, 0, 0, 0, 1, 0, 0, 0, 1);
            t.Translate(origin + position + new Vector2f(size.X,0));
            guiState.Transform = t;
            gui.Draw(dropdownArrow, guiState);
            selectedText.Draw(gui, origin + position, guiState);
            //Draw the dropdown button
            //If is focused:
            if (listboxState == ListBoxState.Focused)
            {
                for (int i = 0; i < listHeaders.Length; i++)
                {
                    RectangleShape otherbox = new RectangleShape(size);
                    otherbox.Position = origin + new Vector2f(position.X, position.Y + size.Y + i * (lineSpacing + 1));
                    otherbox.FillColor = new Color(64, 64, 64, 128);
                    if(i == hoveredValue)
                    {
                        otherbox.FillColor = new Color(32, 96, 32, 128);
                    }
                    gui.Draw(otherbox);
                    texts[i].Draw(gui, origin + position, guiState);
                }
            }
            //Draw the dropdown transparent selector
            //Draw the highlight over currently hovered value
            //Draw the text for the values
            guiState.Transform = original;
        }

        public override void HandleInput(InputManager input)
        {
            base.HandleInput(input);
            //Computing absolute position to check collision
            Vector2f mousePos;
            bool mouse = input.GetMousePosition(out mousePos);
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
            else if (listboxState == ListBoxState.Focused)
            {
                for (int i = 0; i < listHeaders.Length; i++)
                {
                    //check collision for each list header
                    if (BoundingBox.CheckPointMenuCollision(mousePos.X, mousePos.Y, dropdownBox, origin + position + new Vector2f(0, lineSpacing * (i + 1))))
                    {
                        hoveredValue = i;
                        collided = true;
                    }
                }
                if(input.GetMouseClickedIgnoreConsume(InputBindings.primary))
                {
                    //don't process a click if it was on the listbox
                    if(collided)
                    {
                        input.GetMouseClicked(InputBindings.primary, true);
                    }
                    selectedValue = hoveredValue;
                    applySelection?.Invoke(tag, listValues[selectedValue]);
                    selectedText.SetText(listHeaders[selectedValue]);
                    listboxState = ListBoxState.Normal;
                }
            }
        }
    }
}
