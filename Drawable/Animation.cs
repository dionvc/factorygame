using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFML.Graphics;
using SFML.System;

namespace EngineeringCorpsCS
{
    class Animation: Drawable
    {
        public enum AnimationBehavior
        {
            Forward,
            Backward,
            ForwardAndBackward
        }
        AnimationBehavior behavior
        {
            get
            {
                return behavior;
            }
            set
            {
                behavior = value;
                SetBehavior(behavior);
            }
        }
        Sprite animationSprite;
        DrawLayer drawLayer;
        Texture textureReference;
        Vector2f origin;
        IntRect textureBounds;
        IntRect textureFrame;
        int incrementAmount = 1;
        int frames = 1;
        int currentFrame = 0;
        float animationSpeed = 0; //ticks per frame of animation (eg. 1 is 1 tick per frame, 2 is 2 ticks per frame
        float tickAccumulator = 0;
        Color color;
        Vector2f scale;
        float rotation;

        public Animation(Texture textureReference, int width, int height, int frames, IntRect bounds, Vector2f drawOffset)
        {
            this.textureReference = textureReference;
            this.textureBounds = bounds;
            this.frames = frames;
            this.origin = new Vector2f(width / 2, height / 2);
            textureFrame = new IntRect(bounds.Left, bounds.Top, width, height);
            color = new Color(255, 255, 255, 255);
            scale = new Vector2f(1.0f, 1.0f);
            rotation = 0.0f;
            this.drawOffset = drawOffset;
            animationSprite = new Sprite(textureReference);
            animationSprite.Origin = origin;
        }

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
                        incrementAmount *= (int)behavior;
                        currentFrame = (currentFrame + frames) % (frames);

                    }
                }
            }
        }

        public void Draw(SpriteBatch[] spriteBatch, Vector2f position)
        {
            textureFrame.Left = (textureFrame.Width * currentFrame) % (textureBounds.Width);
            textureFrame.Top = (textureFrame.Width * currentFrame) / (textureBounds.Width) * textureFrame.Height;
            spriteBatch[(int)drawLayer].Draw(textureReference, position + drawOffset, textureFrame, color, scale, origin, rotation);
        }

        override public Sprite GetSprite()
        {
            textureFrame.Left = (textureFrame.Width * currentFrame) % (textureBounds.Width);
            textureFrame.Top = (textureFrame.Width * currentFrame) / (textureBounds.Width) * textureFrame.Height;
            animationSprite.TextureRect = textureFrame;
            return animationSprite;
        }

        /// <summary>
        /// Does nothing as Animation does not need rotation
        /// </summary>
        /// <param name="rotation"></param>
        override public void SetRotation(float rotation)
        {
            //does nothing
        }

        override public void SetColor(byte r, byte g, byte b, byte a)
        {
            this.color = new Color(r, g, b, a);
        }

        override public void SetScale(float x, float y)
        {
            this.scale = new Vector2f(x, y);
        }

        /// <summary>
        /// Set the speed of the animation in ticks per frame
        /// </summary>
        /// <param name="animationSpeed"></param>
        override public void SetAnimationSpeed(float animationSpeed)
        {
            this.animationSpeed = animationSpeed;
        }

        private void SetBehavior(AnimationBehavior behavior)
        {
            switch (behavior)
            {
                case (AnimationBehavior.Forward):
                    this.incrementAmount = 1;
                    break;
                case (AnimationBehavior.Backward):
                    this.incrementAmount = -1;
                    break;
                case (AnimationBehavior.ForwardAndBackward):
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
