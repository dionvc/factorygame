using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EngineeringCorpsCS
{
    class Renderer
    {
        Renderer()
        {

        }
        /// <summary>
        /// Collects visible chunks into list
        /// </summary>
        /// <param name="camX"></param>
        /// <param name="camY"></param>
        /// <param name="camW"></param>
        /// <param name="camH"></param>
        /// <param name="chunkM"></param>
        void GetVisibleChunks(float camX, float camY, float camW, float camH, ChunkManager chunkM)
        {
            int[] top = ChunkManager.WorldToChunkCoords(camX, camY);
            int[] bot = ChunkManager.WorldToChunkCoords(camX + camW, camY + camH);
            List<Chunk> cList = new List<Chunk>();
            for(int i = top[0]; i < bot[0]; i++)
            {
                for(int j = top[1]; j < top[1]; j++)
                {
                    cList.Add(chunkM.GetChunk(i, j));
                }
            }
        }

        //Construct vertex arrays for each visible chunk and each visible tile (cull tiles)
        //Store these vertex arrays

        //Check every frame the camera location

        //Update visible chunks if camera location changes enough

        //Get entities every frame from visible chunks and cull, then sort
    }
}
