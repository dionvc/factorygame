using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFML.System;
using SFML.Graphics;

namespace EngineeringCorpsCS
{
    class MenuProgressBar : MenuComponent
    {
        /// <summary>
        /// Return a value between 0 and 1 representing progress
        /// </summary>
        /// <param name="tag"></param>
        /// <returns></returns>
        public delegate float GetProgress(string tag);
        VertexArray backdrop;
        VertexArray bar;
        public bool hideWhenEmpty { get; set; } = false;
        public bool hideWhenFull { get; set; } = false;
        public string tag { get; set; } = "";
        float progress;
        GetProgress barProgress;
        public MenuProgressBar(Vector2f componentSize, FloatRect backdropBounds, FloatRect barBounds, GetProgress barProgress, Color barColor)
        {
            Initialize(componentSize);

            backdrop = CreateMenuGraphic(backdropBounds, componentSize);
            bar = CreateMenuGraphic(barBounds, componentSize, barColor);
            this.barProgress = barProgress;
        }

        public override void Draw(RenderTexture gui, Vector2f origin, RenderStates guiState)
        {
            progress = barProgress.Invoke(tag);
            if (!(progress == 1.0f && hideWhenFull) && !(progress == 0.0f && hideWhenEmpty))
            {
                Transform t = new Transform(1, 0, 0, 0, 1, 0, 0, 0, 1);
                t.Translate(origin + position);
                Transform original = guiState.Transform;
                guiState.Transform = t;
                //draw backdrop
                gui.Draw(backdrop, guiState);
                //draw bar
                t = new Transform(1, 0, 0, 0, 1, 0, 0, 0, 1);
                t.Scale(progress, 1);
                t.Translate(new Vector2f((origin + position).X * (1/progress), (origin+position).Y));
                guiState.Transform = t;
                gui.Draw(bar, guiState);
                guiState.Transform = original;
            }
            base.Draw(gui, origin, guiState);
        }
    }
}
