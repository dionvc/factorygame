using SFML.Graphics;
using SFML.System;

namespace EngineeringCorpsCS
{
    class MenuDrawable:MenuComponent
    {
        Drawable drawable;
        Vector2f scaleFactor;
        Entity entity;
        int index;
        public MenuDrawable(Vector2i size, Entity entity, int index)
        {
            Initialize(size);
            this.entity = entity;
            this.index = index;
            Recalculate();
        }

        private void Recalculate()
        {
            drawable = entity.drawArray[index];
            Sprite sprite = drawable.GetSprite();
            if (sprite.TextureRect.Width >= sprite.TextureRect.Height) //scale if too big in X
            {
                scaleFactor = new Vector2f(size.X * 1.0f / sprite.TextureRect.Width, size.Y * 1.0f / sprite.TextureRect.Width);
            }
            else //scale if too big in Y
            {
                scaleFactor = new Vector2f(size.X * 1.0f / sprite.TextureRect.Height, size.Y * 1.0f / sprite.TextureRect.Height);
            }
        }

        public override void Draw(RenderTexture gui, Vector2i origin, RenderStates guiState)
        {
            if (!ReferenceEquals(drawable, entity.drawArray[0]))
            {
                Recalculate();
            }
            Sprite sprite = drawable.GetSprite();
            sprite.Position = new Vector2f(origin.X + position.X, origin.Y + position.Y);
            sprite.Scale = scaleFactor;
            gui.Draw(sprite);
            base.Draw(gui, origin, guiState);
        }
    }
}
