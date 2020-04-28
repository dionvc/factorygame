using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFML.Graphics;
using SFML.System;

namespace EngineeringCorpsCS
{
    class Animation : Drawable, IAnimation
    {
        Sprite internalSprite = null;
        Texture texture;
        Vector2f origin;
        IntRect textureBounds;
        IntRect textureFrame;
        int incrementAmount = 1;
        int frames = 1;
        public int currentFrame { get; set; } = 0;
        public float animationSpeed { get; set; } = 0; //ticks per frame of animation (eg. 1 is 1 tick per frame, 2 is 2 ticks per frame
        float tickAccumulator = 0;
        float rotation = 0.0f;
        int behaviorIncrement = 1;
        AnimationBehavior _behavior;
        public AnimationBehavior behavior
        {
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

        public Animation(Texture texture, int width, int height, int frames, IntRect bounds, Vector2f drawOffset)
        {
            this.texture = texture;
            this.textureBounds = bounds;
            this.frames = frames;
            this.origin = new Vector2f(width / 2, height / 2);
            textureFrame = new IntRect(bounds.Left, bounds.Top, width, height);
            rotation = 0.0f;
            this.drawOffset = drawOffset;
        }

        public new Animation Clone()
        {
            Animation cloned = new Animation(this.texture, this.textureFrame.Width, this.textureFrame.Height, this.frames, this.textureBounds, new Vector2f(this.drawOffset.X, this.drawOffset.Y));
            cloned.drawLayer = this.drawLayer;
            cloned.color = this.color;
            return cloned;
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
                }
            }
        }

        /// <summary>
        /// Draws the drawable to a spritebatcher
        /// </summary>
        /// <param name="spriteBatch"></param>
        /// <param name="position"></param>
        override public void Draw(SpriteBatch spriteBatch, Vector2f position)
        {
            textureFrame.Left = textureBounds.Left + (textureFrame.Width * currentFrame) % (textureBounds.Width);
            textureFrame.Top = textureBounds.Top + (textureFrame.Width * currentFrame) / (textureBounds.Width) * textureFrame.Height;
            spriteBatch.Draw(texture, position + drawOffset, textureFrame, color, scale, origin, rotation);
        }

        public override Sprite GetSprite()
        {
            if(internalSprite == null)
            {
                internalSprite = new Sprite(texture, textureFrame);
                internalSprite.Color = color;
            }
            return internalSprite;
        }

        /*override public Sprite GetSprite()
        {
            textureFrame.Left = (textureFrame.Width * currentFrame) % (textureBounds.Width);
            textureFrame.Top = (textureFrame.Width * currentFrame) / (textureBounds.Width) * textureFrame.Height;
            animationSprite.TextureRect = textureFrame;
            return animationSprite;
        }*/

        /// <summary>
        /// Does nothing as Animation does not need rotation
        /// </summary>
        /// <param name="rotation"></param>
        override public void SetRotation(float rotation)
        {
            this.rotation = rotation;
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
