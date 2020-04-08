using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFML.Graphics;
using SFML.System;

namespace EngineeringCorpsCS
{
    class AnimationRotated : Drawable
    {
        public enum AnimationBehavior {
            Forward,
            Backward,
            ForwardAndBackward
        }
        Sprite animationFrame;
        Texture[] textureRefs; //all the textures making up the animation (in order)
        Vector2i size; //stores x (width), y (height) of sprite to capture from texture
        Vector2i texturePos; //position on texture
        Vector2i textureSize; //size of a texture in the texture array
        int states = 1; //number of states (intended for rotation, eg for 2 states one north/south one east/west)
        int frames = 1; //Frames per state (assumption of static number of sprites across states (fair assumption)
        float animationSpeed = 0; //measured in ticks per frame (eg, 1 is 1 tick per frame, 2 is 2 ticks per frame, 0.5 is 2 frames per tick)
        int currentFrame = 0; //current frame of animation state
        int currentState = 0; //current state (< states)
        float tickAccumulator = 0; //accumulates frames to subtract from
        int incrementAmount = 1;
        int behaviorIncrement = 1;
        AnimationBehavior _behavior;
        public AnimationBehavior behavior {
            get
            {
                return _behavior;
            }
            set
            {
                _behavior = value;
                SetBehavior(_behavior);
            }
        }

        //TODO: add color
        
        /// <summary>
        /// Creates a rotated animation
        /// </summary>
        /// <param name="textureRefs"></param>
        /// <param name="frameSize"></param>
        /// <param name="rotationStates"></param>
        /// <param name="framesPerState"></param>
        /// <param name="behavior"></param>
        /// <param name="animationSpeed"></param>
        /// <param name="textureOffset"></param>
        public AnimationRotated(Texture[] textureRefs, Vector2i frameSize, Vector2f textureOffset, Vector2f drawOffset, Vector2f scale, int rotationStates, int framesPerState, string behavior, float animationSpeed)
        {
            this.animationFrame = new Sprite();
            this.size = frameSize;
            this.textureRefs = textureRefs;
            this.textureSize = new Vector2i((int)this.textureRefs[0].Size.X, (int)this.textureRefs[0].Size.Y);
            this.states = rotationStates;
            this.frames = framesPerState;
            this.animationSpeed = animationSpeed;
            this.drawOffset = drawOffset;
            animationFrame.Origin = new Vector2f(frameSize.X/2 + textureOffset.X, frameSize.Y/2 + textureOffset.Y);
            animationFrame.Scale = scale;
        }

        /// <summary>
        /// Simpler initializer (should really only be used for testing purposes)
        /// </summary>
        /// <param name="textureRef"></param>
        /// <param name="frameSize"></param>
        /// <param name="rotationStates"></param>
        /// <param name="framesPerState"></param>
        /// <param name="behavior"></param>
        /// <param name="animationSpeed"></param>
        public AnimationRotated(Texture[] textureRef, Vector2i frameSize, int rotationStates, int framesPerState, string behavior, float animationSpeed)
        {
            this.animationFrame = new Sprite();
            this.size = frameSize;
            this.textureRefs = textureRef;
            this.textureSize = new Vector2i((int)this.textureRefs[0].Size.X, (int)this.textureRefs[0].Size.Y);
            this.states = rotationStates;
            this.frames = framesPerState;
            this.animationSpeed = animationSpeed;
            animationFrame.Origin = new Vector2f(frameSize.X / 2, frameSize.Y / 2);
        }



        /// <summary>
        /// Updates the animation's sprite based on its animation speed and behavior
        /// </summary>
        override public void Update()
        {
            if (animationSpeed != 0)
            {
                tickAccumulator += 1;
                //TODO: test multiple texture sheet support + test texture frame size (is it off by ones)
                if (tickAccumulator > animationSpeed)
                {
                    while (tickAccumulator > animationSpeed)
                    {
                        currentFrame += incrementAmount;
                        tickAccumulator -= animationSpeed;
                    }
                    if (currentFrame >= frames - 1 || currentFrame <= 0)
                    {
                        incrementAmount *= behaviorIncrement;
                        currentFrame = (currentFrame + frames) % (frames);
                        
                    }
                    
                }
            }
        }

        /// <summary>
        /// Gets the current sprite of the animation
        /// </summary>
        /// <returns></returns>
        override public Sprite GetSprite()
        {
            //TODO: ?Idea?, pass in current frame of game and store that.  The delta between current and last can be used to calculate current frame
            //rather than constantly updating animation


            //Calculate the texture box when the sprite is needed
            texturePos.X = (size.X * (currentFrame + (currentState * frames))) % (textureSize.X * textureRefs.Length);
            texturePos.Y = (size.X * (currentFrame + (currentState * frames))) / (textureSize.X * textureRefs.Length) * size.Y;
            int textureIndex = (texturePos.X / textureSize.X);
            texturePos.X = (texturePos.X % (textureSize.X));
            //Console.WriteLine("TextureIndex: " + textureIndex + "\nFrame: " + currentFrame + "\nTexPos: " + texturePos.X +  ", " + texturePos.Y);
            animationFrame.Texture = textureRefs[textureIndex];
            animationFrame.TextureRect = new IntRect(texturePos, size);
            return animationFrame;
        }

        override public void SetColor(byte r, byte g, byte b, byte a)
        {
            animationFrame.Color = new Color(r, g, b, a);
        }

        override public void SetScale(float x, float y)
        {
            animationFrame.Scale = new Vector2f(x, y);
        }

        /// <summary>
        /// Sets the rotation state of the rotated animation
        /// </summary>
        /// <param name="rotation"></param>
        override public void SetRotation(float rotation)
        {
            currentState = (int)Math.Round(rotation * states / 360.0f);
            currentState = currentState % states;
        }

        override public void SetAnimationSpeed(float animationSpeed)
        {
            this.animationSpeed = animationSpeed;
        }

        /// <summary>
        /// Sets the behavior of the animation (Forward, Backward, Forward and Backward)
        /// </summary>
        /// <param name="behavior"></param>
        private void SetBehavior(AnimationBehavior behavior)
        {
            switch (behavior)
            {
                case (AnimationBehavior.Forward):
                    this.incrementAmount = 1;
                    this.behaviorIncrement = 1;
                    break;
                case (AnimationBehavior.Backward):
                    this.incrementAmount = -1;
                    this.behaviorIncrement = 1;
                    break;
                case (AnimationBehavior.ForwardAndBackward):
                    this.incrementAmount = 1;
                    this.behaviorIncrement = -1;
                    break;
                default:
                    this.behavior = AnimationBehavior.Forward;
                    this.incrementAmount = 1;
                    break;
            }
        }
    }
}
