using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFML.Graphics;
using SFML.System;

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
            for (int i = 0; i < menus.Count; i++)
            {
                menus[i].Draw(GUI, new Vector2f(0, 0));
            }
        }
        public void AttachMenu(MenuComponent menu)
        {
            menu.container = this;
            menus.Add(menu);
            menu.SubscribeToInput(input);
        }

        public void RemoveMenu(MenuComponent menu)
        {
            menus.Remove(menu);
            menu.UnsubscribeToInput(input);
        }

        public void RemoveAllMenus()
        {
            menus.Clear();
        }
    }
}
