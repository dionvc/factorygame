using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFML.Graphics;
using SFML.System;
using SFML.Window;

namespace EngineeringCorpsCS
{
    class MenuContainer
    {
        private List<MenuComponent> menus = new List<MenuComponent>();
        InputManager input;
        public MenuContainer(InputManager input)
        {
            this.input = input;
        }
        public void RenderMenus(RenderTexture GUI)
        {
            for (int i = menus.Count - 1; i >= 0; i--)
            {
                menus[i].Draw(GUI, new Vector2f(0, 0));
            }
        }
        public void AttachMenu(MenuComponent menu)
        {
            menu.container = this;
            menus.Insert(0,menu);
            menu.SubscribeToInput(input);
        }

        public void RemoveMenu(MenuComponent menu)
        {
            menus.Remove(menu);
            menu.UnsubscribeToInput(input);
        }

        public void RemoveAllMenus()
        {
            foreach(MenuComponent menu in menus)
            {
                menu.UnsubscribeToInput(input);
            }
            menus.Clear();
        }

        public void RepositionMenus(Object s, SizeEventArgs e)
        {
            foreach(MenuComponent menu in menus)
            {
                menu.SetInitialPosition(input.GetCamera().GetGUIView());
            }
        }

        public void PushMenuToFront(MenuComponent menuComponent)
        {
            menus.Remove(menuComponent);
            menus.Insert(0, menuComponent);
            input.PushMenuToFront(menuComponent);
        }
    }
}
