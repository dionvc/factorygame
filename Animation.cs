using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFML.Graphics;
using SFML.System;

namespace EngineeringCorpsCS
{
    class Animation: IAnimation
    {
        enum AnimationBehavior
        {
            Forward = 1,
            Backward = 1,
            ForwardAndBackward = -1
        }
        AnimationBehavior behavior;
        Sprite animationFrame;
        Texture[] textureRefs;
        Vector2i size; //stores x (width), y (height) of sprite to capture from texture
        Vector2i texturePos; //position on texture
        Vector2i textureSize;
        Vector2i offset; //offset of sprite from center of entity in pixels 
        int incrementAmount = 1;
        int frames = 1;
        int currentFrame = 0;
        float animationSpeed = 0; //ticks per frame of animation (eg. 1 is 1 tick per frame, 2 is 2 ticks per frame
        float tickAccumulator = 0;

        public Animation(Texture[] textureRefs, Vector2i frameSize, int frames, string behavior, float animationSpeed, Vector2i offset)
        {
            this.animationFrame = new Sprite();
            this.size = frameSize;
            this.textureRefs = textureRefs;
            this.textureSize = new Vector2i((int)this.textureRefs[0].Size.X, (int)this.textureRefs[0].Size.Y);
            this.frames = frames;
            this.offset = offset;
            this.animationSpeed = animationSpeed;
            SetBehavior(behavior);
        }

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
                        incrementAmount *= (int)behavior;
                        currentFrame = (currentFrame + frames) % (frames);

                    }
                    texturePos.X = (size.X * currentFrame) % (textureSize.X * textureRefs.Length);
                    texturePos.Y = (size.X * currentFrame) / (textureSize.X * textureRefs.Length) * size.Y;
                    int textureIndex = (texturePos.X / textureSize.X);
                    animationFrame.Texture = textureRefs[textureIndex];
                    animationFrame.TextureRect = new IntRect(texturePos, size);
                }
            }
        }

        public Sprite GetAnimationFrame()
        {
            return animationFrame;
        }

        /// <summary>
        /// Does nothing as Animation does not need rotation
        /// </summary>
        /// <param name="rotation"></param>
        public void SetRotation(float rotation)
        {
            //does nothing
        }

        public void SetColor(byte r, byte g, byte b, byte a)
        {
            animationFrame.Color = new Color(r, g, b, a);
        }

        public void SetScale(float x, float y)
        {
            animationFrame.Scale = new Vector2f(x, y);
        }

        /// <summary>
        /// Set the speed of the animation in ticks per frame
        /// </summary>
        /// <param name="animationSpeed"></param>
        public void SetAnimationSpeed(float animationSpeed)
        {
            this.animationSpeed = animationSpeed;
        }

        /// <summary>
        /// Sets the behavior of the animation (Forward, Backward, Forward and Backward)
        /// </summary>
        /// <param name="behavior"></param>
        public void SetBehavior(string behavior)
        {
            switch (behavior)
            {
                case ("f"):
                case ("Forward"):
                    this.behavior = AnimationBehavior.Forward;
                    this.incrementAmount = 1;
                    break;
                case ("b"):
                case ("Backward"):
                    this.behavior = AnimationBehavior.Backward;
                    this.incrementAmount = -1;
                    break;
                case ("fb"):
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
