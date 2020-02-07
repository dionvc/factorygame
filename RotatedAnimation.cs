using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFML.Graphics;
using SFML.System;

namespace EngineeringCorpsCS
{
    class RotatedAnimation: IAnimation
    {
        //Assumption: textures will be loaded to GPU by separate texture loader
        //Assumption: animations can be created by animation factory, so texture and other variables will be set by factory
        enum AnimationBehavior{
            Forward = 1,
            Backward = 1,
            ForwardAndBackward = -1
        }
        Sprite animationFrame; //array of sprites, heart of animation
        Texture[] textureRefs;
        Vector2i size; //stores x (width), y (height) of sprite to capture from texture
        Vector2i texturePos; //position on texture
        Vector2i textureSize;
        Vector2i offset; //offset of sprite from center of entity in pixels 
        //(default draw will center sprite around entity position)
        int states = 1; //number of states (intended for rotation, eg for 2 states one north/south one east/west)
        int frames = 1; //Frames per state (assumption of static number of sprites across states (fair assumption)
        float animationSpeed = 0; //measured in ticks per frame (eg, 1 is 1 tick per frame, 2 is 2 ticks per frame, 0.5 is 2 frames per tick)
        int currentFrame = 0; //current frame of animation state
        int currentState = 0; //current state (< states)
        float tickAccumulator = 0; //accumulates frames to subtract from
        int incrementAmount = 1;
        AnimationBehavior behavior;
        
        public RotatedAnimation(Texture[] textureRefs, Vector2i frameSize, int rotationStates, int framesPerState, string behavior, float animationSpeed, Vector2i offset)
        {
            this.animationFrame = new Sprite();
            this.size = frameSize;
            this.textureRefs = textureRefs;
            this.textureSize = new Vector2i((int)this.textureRefs[0].Size.X, (int)this.textureRefs[0].Size.Y);
            this.states = rotationStates;
            this.frames = framesPerState;
            this.offset = offset;
            this.animationSpeed = animationSpeed;
            animationSpeed = 0;
            SetBehavior(behavior);
        }



        /// <summary>
        /// Updates the animation's sprite based on its animation speed and behavior
        /// </summary>
        public void Update()
        {
            if (animationSpeed != 0)
            {
                tickAccumulator += 1;
                while (tickAccumulator > animationSpeed)
                {
                    currentFrame += incrementAmount;
                    tickAccumulator -= animationSpeed;
                }
                if (currentFrame > frames)
                {
                    currentFrame = currentFrame % frames;
                    incrementAmount *= (int)behavior;
                }

                //TODO: test multiple texture sheet support + test texture frame size (is it off by ones)

                texturePos.X = (size.X * (currentFrame + (currentState * frames))) % (textureSize.X * textureRefs.Length);
                texturePos.Y = (size.X * (currentFrame + (currentState * frames))) / (textureSize.X * textureRefs.Length) * size.Y;
                int textureIndex = (texturePos.X / textureSize.X);
                animationFrame.Texture = textureRefs[textureIndex];
                animationFrame.TextureRect = new IntRect(texturePos, size);
            }
        }

        /// <summary>
        /// Gets the current sprite of the animation
        /// </summary>
        /// <returns></returns>
        public Sprite GetAnimationFrame()
        {
            //Idea, pass in current frame of game and store that.  The delta between current and last can be used to calculate current frame
            //rather than constantly updating animation
            return animationFrame;
        }

        public void SetAnimationSpeed(float animationSpeed)
        {
            this.animationSpeed = animationSpeed;
        }

        public void SetBehavior(string behavior)
        {
            switch(behavior)
            {
                case ("Forward"):
                    this.behavior = AnimationBehavior.Forward;
                    this.incrementAmount = 1;
                    break;
                case ("Backward"):
                    this.behavior = AnimationBehavior.Backward;
                    this.incrementAmount = -1;
                    break;
                case ("ForwardAndBackward"):
                    this.behavior = AnimationBehavior.ForwardAndBackward;
                    this.incrementAmount = 1;
                    break;
                default:
                    this.behavior = AnimationBehavior.Forward;
                    this.incrementAmount = 1;
                    break;
            }
        }
    }
}
