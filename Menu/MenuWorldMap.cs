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
        int refreshCounter = 0;
        int refreshRate = 30;
        int tX = 0;
        int tY = 0;
        float mapScale = 1.0f;
        int minimapXRange = 5;
        int minimapYRange = 5;
        bool getPollution = true;
        public bool controllable { get; set; } = false;
        public int controlSpeed { get; set; } = 4;
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
            if (refreshCounter >= refreshRate || renderer.modifiedVertexArrays == true)
            {
                renderer.GenerateMinimapTextures(camera.focusedEntity.surface, camera.focusedEntity.position, minimapXRange, minimapYRange, vertexArrays);
                refreshCounter = 0;
                if(getPollution == true)
                {
                    vertexArrays.Add(new VertexArray(PrimitiveType.Triangles));
                    renderer.GeneratePollutionVertexArray(camera.focusedEntity.surface, camera.focusedEntity.position, minimapXRange, minimapYRange, vertexArrays[vertexArrays.Count - 1]);
                }
            }
            Transform transform = new Transform(1, 0, 0, 0, 1, 0, 0, 0, 1);
            Vector2f translation = new Vector2f(size.X / 2 - (camera.focusedEntity.position.x * mapScale / Props.tileSize) + tX, size.Y / 2 - (camera.focusedEntity.position.y * mapScale / Props.tileSize) + tY);
            transform.Translate(translation);
            transform.Scale(mapScale, mapScale);
            transformState.Transform = transform;
            for (int i = 0; i < vertexArrays.Count; i++)
            {
                if (vertexArrays[i] != null)
                {
                    textureMinimap.Draw(vertexArrays[i], transformState);
                }
            }
            textureMinimap.Display();
            Sprite minimap = new Sprite(textureMinimap.Texture);
            minimap.Position = origin + position;
            gui.Draw(minimap);
            textureMinimap.Clear();
        }

        public override void HandleInput(InputManager input)
        {
            base.HandleInput(input);
            if (controllable)
            {
                if (input.keyHeld[InputBindings.moveLeft])
                {
                    tX += controlSpeed;
                }
                if (input.keyHeld[InputBindings.moveRight])
                {
                    tX -= controlSpeed;
                }
                if (input.keyHeld[InputBindings.moveUp])
                {
                    tY += controlSpeed;
                }
                if (input.keyHeld[InputBindings.moveDown])
                {
                    tY -= controlSpeed;
                }
                if (input.mouseScrollDelta != 0)
                {
                    mapScale += (input.mouseScrollDelta / InputBindings.scrollSensitivity);
                    mapScale = mapScale < 0.5f ? 0.5f : mapScale;
                    mapScale = mapScale > 8.0f ? 8.0f : mapScale;
                }
            }
        }

        public void TogglePollution()
        {
            getPollution = !getPollution;
        }
    }
}
