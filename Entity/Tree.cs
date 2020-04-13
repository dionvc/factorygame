using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFML.Graphics;
using SFML.System;

namespace EngineeringCorpsCS
{
    class Tree : EntityPhysical
    {
        public enum TreeState
        {
            Chopped,    //If a tree exists in this state, it is just so it can leave a tree stump
            Dead,       //A tree in this state has no leaves
            Dying,      //A tree in this states only has some leaves
            Affected,   //A tree in this state has a moderate number of leaves
            Healthy     //A tree in this state has numerous leaves
        }
        StaticSprite trunk; //contains full tree trunk or stump
        Animation leaves; //A leaf animation that will change the leaf density based on the state of the animation
        Animation shadow; //A shadow animation containing the various shadows of the tree
        TreeState treeState;

        /// <summary>
        /// Creates a tree
        /// </summary>
        /// <param name="pos"></param>
        /// <param name="surface"></param>
        /// <param name="textureAtlases"></param>
        public Tree(Vector2 pos, SurfaceContainer surface, TextureAtlases textureAtlases)
        {
            position = pos;
            collisionBox = new BoundingBox(32, 32);
            surface.InitiateEntityInChunks(this);
            IntRect bounds;
            Animation trunk = new Animation(textureAtlases.GetTexture("tree", out bounds), 128, bounds.Height, 1, bounds, new Vector2f(0, -112));
            Animation leaves = new Animation(textureAtlases.GetTexture("tree", out bounds), 128, bounds.Height, 1, bounds, new Vector2f(0, -112));
            Random r = new Random();
            leaves.currentFrame = r.Next(0,4);
            if (leaves.currentFrame != 0)
            {
                drawArray = new Drawable[] { trunk, leaves };
            }
            else
            {
                drawArray = new Drawable[] { trunk };
            }
            collisionMask = CollisionLayer.EntityPhysical;
            mapColor = new Color(32, 160, 0);
        }

        public void Update()
        {
            if(surface.GetChunk(centeredChunk, false).pollutionValue > 50)
            {
                treeState = TreeState.Affected;
            }
            else if(surface.GetChunk(centeredChunk, false).pollutionValue > 100)
            {
                treeState = TreeState.Dying;
            }
            else if(surface.GetChunk(centeredChunk, false).pollutionValue > 150)
            {
                treeState = TreeState.Dead;
            }
        }

        public void EvaluateTree()
        {
            switch (treeState)
            {
                case (TreeState.Healthy):
                    break;
                case (TreeState.Affected):
                    break;
                case (TreeState.Dying):
                    break;
                case (TreeState.Dead):
                    break;
                case (TreeState.Chopped):
                    break;
            }
        }
    }
}
