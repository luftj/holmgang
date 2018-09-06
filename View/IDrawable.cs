using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace holmgang.Desktop
{
    public interface IDrawable
    {
        void draw(GameTime gameTime, SpriteBatch spriteBatch);
    }
}
