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
        int refreshCounter = 60;
        int refreshRate = 60;
        Vector2 controlTranslation;
        /// <summary>
        /// Set the mapscale for non-controllable minimaps.
        /// </summary>
        public float mapScale { get; set; } = Props.worldMapZoomMin;
        int minimapXRange = 2;
        int minimapYRange = 2;
        bool getPollution = true;
        public bool controllable { get; set; } = false;
        public int controlSpeed { get; set; } = 4;
        public MenuWorldMap(Vector2i componentSize, Camera camera, Renderer renderer)
        {
            Initialize(componentSize);
            this.camera = camera;
            this.renderer = renderer;
            controlTranslation = new Vector2(0, 0);
            transform = new Transform(1, 0, 0, 0, 1, 0, 0, 0, 1);
            transformState = new RenderStates(transform);
            textureMinimap = new RenderTexture((uint)size.X, (uint)size.Y);
            vertexArrays = new List<VertexArray>();
            pollutionArray = new VertexArray(PrimitiveType.Triangles);
        }

        public override void Draw(RenderTexture gui, Vector2i origin, RenderStates guiState)
        {
            textureMinimap.Clear();
            refreshCounter++;
            if (refreshCounter >= refreshRate || renderer.modifiedVertexArrays == true)
            {
                minimapXRange = (int)Math.Ceiling(size.X/ Props.chunkSize / 2 / mapScale);
                minimapYRange = (int)Math.Ceiling(size.Y/ Props.chunkSize / 2 / mapScale);
                renderer.GenerateMinimapTextures(camera.focusedEntity.surface, camera.focusedEntity.position + new Vector2(-controlTranslation.x * Props.tileSize, -controlTranslation.y * Props.tileSize), minimapXRange, minimapYRange, vertexArrays);
                refreshCounter = 0;
                if(getPollution == true)
                {
                    renderer.GeneratePollutionVertexArray(camera.focusedEntity.surface, camera.focusedEntity.position + new Vector2(-controlTranslation.x * Props.tileSize, -controlTranslation.y * Props.tileSize), minimapXRange, minimapYRange, pollutionArray);
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
            minimap.Position = new Vector2f((position + origin).X, (origin + position).Y);
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
                    mapScale = mapScale < Props.worldMapZoomMin ? Props.worldMapZoomMin : mapScale;
                    mapScale = mapScale > Props.worldMapZoomMax ? Props.worldMapZoomMax : mapScale;
                }
            }
        }

        public void TogglePollution(string tag)
        {
            getPollution = !getPollution;
            refreshCounter = refreshRate;
        }
    }
}
