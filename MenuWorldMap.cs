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
        List<VertexArray> vertexArrays;
        RenderTexture textureMinimap;
        RenderStates transformState;
        Transform transform;
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

            transform = new Transform(1, 0, 0, 0, 1, 0, 0, 0, 1);
            transformState = new RenderStates(transform);
            textureMinimap = new RenderTexture((uint)size.X, (uint)size.Y);
            vertexArrays = new List<VertexArray>();
        }

        public override void Draw(RenderTexture gui, Vector2f origin)
        {
            refreshCounter++;
            if(refreshCounter >= refreshRate)
            {
                renderer.GenerateMinimapTextures(camera.focusedEntity.surface, camera.focusedEntity.position, 4, 4, vertexArrays);
            }
            Transform transform = new Transform(1, 0, 0, 0, 1, 0, 0, 0, 1);
            Vector2f translation = new Vector2f(size.X/2 -(camera.focusedEntity.position.x / Props.tileSize), size.Y/2 -(camera.focusedEntity.position.y / Props.tileSize));
            transform.Translate(translation);
            transformState.Transform = transform;
            for (int i = 0; i < vertexArrays.Count; i++)
            {
                textureMinimap.Draw(vertexArrays[i], transformState);
            }
            textureMinimap.Display();
            Sprite minimap = new Sprite(textureMinimap.Texture);
            minimap.Position = position;
            gui.Draw(minimap);
            textureMinimap.Clear();
        }
    }
}
