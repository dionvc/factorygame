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
        byte tilePlace = 0;
        private TextureAtlases textureAtlases;
        /// <summary>
        /// This is a test entity!!!
        /// </summary>
        public TestTilePlacer(SurfaceContainer surface, Renderer renderer, TextureAtlases textureAtlases)
        {
            this.surface = surface;
            this.renderer = renderer;
            this.textureAtlases = textureAtlases;
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
            if(input.GetKeyPressed(InputBindings.increasePlacementSize, false))
            {
                tilePlace += 1;
            }
            if(input.GetKeyPressed(InputBindings.decreasePlacementSize, false))
            {
                tilePlace -= 1;
            }
            if(input.GetMouseHeld(InputBindings.primary, false))
            {
                float[] mousePos;
                bool mouse = input.GetMousePositionAsFloat(out mousePos);
                BoundingBox tilePlacementBox = new BoundingBox(1 + placementSize * 32, 1 + placementSize * 32);
                Vector2 pos = new Vector2(mousePos[0], mousePos[1]);
                int[] chunkBounds = BoundingBox.GetChunkBounds(tilePlacementBox, pos, surface);
                int[][] tileBounds = BoundingBox.GetTileBounds(tilePlacementBox, pos);
                for(int i = 0; i < chunkBounds.Length; i++)
                {
                    Chunk chunk = surface.GetChunk(chunkBounds[i], false);
                    if (chunk != null)
                    {
                        for (int j = 0; j < tileBounds[i].Length; j++)
                        {
                            chunk.SetTile(tileBounds[i][j], tilePlace);
                        }
                        renderer.RemoveCachedVertexArray(chunkBounds[i]);
                    }
                }
                    
            }

            if (input.GetMouseHeld(InputBindings.secondary, false))
            {
                float[] mousePos;
                bool mouse = input.GetMousePositionAsFloat(out mousePos);
                int[] tileAligned = new int[] {(int) (mousePos[0] - mousePos[0] % Props.tileSize + 16),(int) ( mousePos[1] - mousePos[1] % Props.tileSize + 16) };
                BoundingBox box = new BoundingBox(-15, -15, 15, 15);
                EntityGhost entityGhost = new EntityGhost(box, new Vector2(tileAligned[0], tileAligned[1]), surface);
                if (!BoundingBox.CheckForCollision(entityGhost))
                {
                    //new Tree(new Vector2(tileAligned[0], tileAligned[1]), surface, textureAtlases);
                    //new Player(new Vector2(tileAligned[0], tileAligned[1]), surface, textureContainer);
                }
            }
        }
    }
}
