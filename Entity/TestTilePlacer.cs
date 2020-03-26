using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EngineeringCorpsCS
{
    class TestTilePlacer: IInputSubscriber
    {
        private SurfaceContainer surface;
        private Renderer renderer;
        int placementSize = 0;
        private TextureContainer textureContainer;
        /// <summary>
        /// This is a test entity!!!
        /// </summary>
        public TestTilePlacer(SurfaceContainer surface, Renderer renderer, TextureContainer textureContainer)
        {
            this.surface = surface;
            this.renderer = renderer;
            this.textureContainer = textureContainer;
        }

        public void SubscribeToInput(InputManager input)
        {
            input.AddInputSubscriber(this, false);
        }

        public void UnsubscribeToInput(InputManager input)
        {
            input.AddInputSubscriber(this, false);
        }

        public void HandleInput(InputManager input)
        {
            if(input.keyPressed[InputBindings.increasePlacementSize])
            {
                placementSize += 1;
            }
            if(input.keyPressed[InputBindings.decreasePlacementSize])
            {
                placementSize -= 1;
            }
            if(input.mouseButton[InputBindings.primary])
            {
                float[] mousePos = input.GetMousePositionAsFloat();
                //TODO: change to get chunkindex and tile index and accept a vector2f to avoid unnecessary operations
                BoundingBox tilePlacementBox = new BoundingBox(1 + placementSize * 32, 1 + placementSize * 32);
                Vector2 pos = new Vector2(mousePos[0], mousePos[1]);
                int[] chunkBounds = BoundingBox.GetChunkBounds(tilePlacementBox, pos);
                int[][] tileBounds = BoundingBox.GetTileBounds(tilePlacementBox, pos);
                for(int i = 0; i < chunkBounds.Length; i++)
                {
                    Chunk chunk = surface.GetChunk(chunkBounds[i], false);
                    if (chunk != null)
                    {
                        for (int j = 0; j < tileBounds[i].Length; j++)
                        {
                            if (chunk.GetTile(tileBounds[i][j]) <= 1)
                            {
                                chunk.SetTile(tileBounds[i][j], 2);
                            }
                        }
                        renderer.RemoveCachedVertexArray(chunkBounds[i]);
                    }
                }
                    
            }

            if(input.mouseButton[InputBindings.secondary])
            {
                float[] mousePos = input.GetMousePositionAsFloat();
                int[] tileAligned = new int[] {(int) (mousePos[0] - mousePos[0] % Props.tileSize),(int) ( mousePos[1] - mousePos[1] % Props.tileSize) };
                BoundingBox box = new BoundingBox(32, 32);
                EntityGhost entityGhost = new EntityGhost(box, new Vector2(tileAligned[0], tileAligned[1]), surface);
                if (!BoundingBox.CheckForCollision(entityGhost))
                {
                    new Player(new Vector2(tileAligned[0], tileAligned[1]), surface, textureContainer);
                }
            }
        }
    }
}
