using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace holmgang.Desktop
{
    public class AISay : AIComponent, IDrawable
    {
        double time;
        string text;
        Vector2 pos;
        bool active = false;

        public AISay(string text, float time)
        {
            GameSingleton.Instance.drawables.Add(this);
            this.text = text;
            this.time = time;
        }

        public void update(GameTime gametime, Character owner)
        {
            active = true;
            pos = owner.pos;

            if(time < 0)
                GameSingleton.Instance.drawables.Remove(this);

            time -= gametime.ElapsedGameTime.TotalSeconds;
        }

        public void draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            if(active && time > 0)
                spriteBatch.DrawString(ContentSupplier.Instance.fonts["testfont"], text, pos+ Vector2.UnitX*20, Color.White);
        }
    }
}
