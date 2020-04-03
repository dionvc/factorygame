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
        VertexArray pollutionArray;
        RenderTexture textureMinimap;
        RenderStates transformState;
        Transform transform;
        Renderer renderer;
        Camera camera;
        int refreshCounter = 30;
        int refreshRate = 30;
        Vector2 controlTranslation;
        float mapScale = 1.0f;
        int minimapXRange = 5;
        int minimapYRange = 5;
        bool getPollution = true;
        public bool controllable { get; set; } = false;
        public int controlSpeed { get; set; } = 4;
        public MenuWorldMap(Camera camera, Renderer renderer, Vector2f relativePosition, Vector2f componentSize)
        {
            Initialize(relativePosition, componentSize);
            this.camera = camera;
            this.renderer = renderer;
            controlTranslation = new Vector2(0, 0);
            transform = new Transform(1, 0, 0, 0, 1, 0, 0, 0, 1);
            transformState = new RenderStates(transform);
            textureMinimap = new RenderTexture((uint)size.X, (uint)size.Y);
            vertexArrays = new List<VertexArray>();
            pollutionArray = new VertexArray(PrimitiveType.Triangles);
        }

        public override void Draw(RenderTexture gui, Vector2f origin, RenderStates guiState)
        {
            textureMinimap.Clear();
            refreshCounter++;
            if (refreshCounter >= refreshRate || renderer.modifiedVertexArrays == true)
            {
                minimapXRange = (int)Math.Ceiling(size.X/ Props.tileSize / 1.5 / mapScale);
                minimapYRange = (int)Math.Ceiling(size.Y/ Props.tileSize / 1.5 / mapScale);
                renderer.GenerateMinimapTextures(camera.focusedEntity.surface, camera.focusedEntity.position + new Vector2(-controlTranslation.x * Props.tileSize, -controlTranslation.y * Props.tileSize), minimapXRange, minimapYRange, vertexArrays);
                refreshCounter = 0;
                if(getPollution == true)
                {
                    renderer.GeneratePollutionVertexArray(camera.focusedEntity.surface, camera.focusedEntity.position, minimapXRange, minimapYRange, pollutionArray);
                }
            }
            Transform transform = new Transform(1, 0, 0, 0, 1, 0, 0, 0, 1);
            Vector2f translation = new Vector2f(size.X / 2 - ((camera.focusedEntity.position.x / Props.tileSize) - controlTranslation.x) * mapScale, size.Y / 2 - ((camera.focusedEntity.position.y / Props.tileSize) - controlTranslation.y) * mapScale);
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
            if (getPollution == true)
            {
                textureMinimap.Draw(pollutionArray, transformState);
            }
            textureMinimap.Display();
            Sprite minimap = new Sprite(textureMinimap.Texture);
            minimap.Position = origin + position;
            gui.Draw(minimap);
            base.Draw(gui, origin, guiState);
        }

        public override void HandleInput(InputManager input)
        {
            base.HandleInput(input);
            if (controllable)
            {
                if (input.GetKeyHeld(InputBindings.moveLeft, true))
                {
                    controlTranslation.x += controlSpeed * mapScale;
                }
                if (input.GetKeyHeld(InputBindings.moveRight, true))
                {
                    controlTranslation.x -= controlSpeed * mapScale;
                }
                if (input.GetKeyHeld(InputBindings.moveUp, true))
                {
                    controlTranslation.y += controlSpeed * mapScale;
                }
                if (input.GetKeyHeld(InputBindings.moveDown, true))
                {
                    controlTranslation.y -= controlSpeed * mapScale;
                }
                if (input.GetMouseScrollDelta(false) != 0)
                {
                    mapScale += (input.GetMouseScrollDelta(true) / InputBindings.scrollSensitivity);
                    mapScale = mapScale < 0.5f ? 0.5f : mapScale;
                    mapScale = mapScale > 8.0f ? 8.0f : mapScale;
                }
            }
        }

        public void TogglePollution()
        {
            getPollution = !getPollution;
            refreshCounter = refreshRate;
        }
    }
}
