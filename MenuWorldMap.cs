using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFML.System;
using SFML.Graphics;

namespace EngineeringCorpsCS
{
    class MenuWorldMap : MenuComponent
    {
        //Two options: have the minimap itself fetch and cache the minimap drawing
        //or have the renderer take care of rendering the minimap to a view this component passes.
        List<Texture> textureList;
        View minimapView;
        Renderer renderer;
        Camera camera;
        int refreshCounter = 60;
        int refreshRate = 60;
        public MenuWorldMap(Camera camera, Renderer renderer, Vector2f relativePosition, Vector2f componentSize, bool[] sizeScaling)
        {
            this.position = relativePosition;
            this.size = componentSize;
            this.camera = camera;
            this.renderer = renderer;

            minimapView = new View(new FloatRect(relativePosition, componentSize));
            textureList = new List<Texture>();
        }

        public override void Draw(RenderTexture gui, Vector2f origin)
        {
            refreshCounter++;
            if(refreshCounter >= refreshRate)
            {
                renderer.GenerateMinimapTextures(camera.focusedEntity.surface, camera.focusedEntity.position, 2, 2, textureList);
            }
            for (int i = 0; i < textureList.Count; i++)
            {
                Sprite s = new Sprite(textureList[i]);
                s.Position = position + new Vector2f((i / 5) * 32, (i % 5) * 32);
                gui.Draw(s);
            }
        }
    }
}
