using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFML.System;
using SFML.Graphics;

namespace EngineeringCorpsCS
{
    class MenuSlider: MenuComponent
    {
        float sliderValue = 0.0f; //value relative to componentSize
        float sliderRange = 100.0f;
        public MenuSlider(Vector2f relativePosition, Vector2f componentSize)
        {

        }
    }
}
