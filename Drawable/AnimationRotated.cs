using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFML.Graphics;
using SFML.System;

namespace EngineeringCorpsCS
{
    class AnimationRotated : Drawable, IAnimation
    {
        Texture texture; //all the textures making up the animation (in order)
        Vector2f origin;
        IntRect textureBounds;
        IntRect textureFrame;
        int states = 1; //number of states (intended for rotation, eg for 2 states one north/south one east/west)
        int frames = 1; //Frames per state (assumption of static number of sprites across states (fair assumption)
        public float animationSpeed { get; set; } = 0; //measured in ticks per frame (eg, 1 is 1 tick per frame, 2 is 2 ticks per frame, 0.5 is 2 frames per tick)
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
        public AnimationRotated(Texture texture, int width, int height, IntRect bounds, Vector2f drawOffset, int rotationStates, int framesPerState)
        {
            this.texture = texture;
            this.textureBounds = bounds;
            this.frames = framesPerState;
            this.origin = new Vector2f(width / 2, height / 2);
            textureFrame = new IntRect(bounds.Left, bounds.Top, width, height);
            this.states = rotationStates;
            this.drawOffset = drawOffset;
        }

        /// <summary>
        /// Updates the animation's sprite based on its animation speed and behavior
        /// </summary>
        public void Update()
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
        /// Draws the drawable to a spritebatcher.
        /// </summary>
        /// <param name="spriteBatch"></param>
        /// <param name="position"></param>
        override public void Draw(SpriteBatch spriteBatch, Vector2f position)
        {
            //Calculate the texture box for drawing
            textureFrame.Left = textureBounds.Left + (textureFrame.Width * (currentFrame + (currentState * frames))) % (textureBounds.Width);
            textureFrame.Top = textureBounds.Top + (textureFrame.Height * (currentFrame + (currentState * frames))) / (textureBounds.Width) * textureFrame.Height;
            spriteBatch.Draw(texture, position + drawOffset, textureFrame, color, scale, origin, 0.0f);
        }

        /// <summary>
        /// Gets the current sprite of the animation
        /// </summary>
        /// <returns></returns>
        /*override public Sprite GetSprite()
        {
            //TODO: ?Idea?, pass in current frame of game and store that.  The delta between current and last can be used to calculate current frame
            //rather than constantly updating animation


            //Calculate the texture box when the sprite is needed
            textureFrame.Left = (textureFrame.Width * (currentFrame + (currentState * frames))) % (textureBounds.Width);
            textureFrame.Top = (textureFrame.Height * (currentFrame + (currentState * frames))) / (textureBounds.Width) * textureFrame.Height;
            //Console.WriteLine("TextureIndex: " + textureIndex + "\nFrame: " + currentFrame + "\nTexPos: " + texturePos.X +  ", " + texturePos.Y);
            animationSprite.TextureRect = textureFrame;
            return animationSprite;
        }*/

        /// <summary>
        /// Sets the rotation state of the rotated animation
        /// </summary>
        /// <param name="rotation"></param>
        override public void SetRotation(float rotation)
        {
            currentState = (int)Math.Round(rotation * states / 360.0f);
            currentState = currentState % states;
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
