using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace holmgang.Desktop
{
    public class Particle : IDrawable, IUpdatable
    {
        Vector2 pos;
        double time;
        Vector2 direction;
        Color colour = Color.DarkRed;

        public Particle(Vector2 pos, double duration, Vector2 direction)
        {
            this.pos = pos;
            time = duration;

            this.direction = direction;
        }

        public void update(GameTime gametime)
        {
            if(time>1.5)
                pos += direction*2;

            time -= gametime.ElapsedGameTime.TotalSeconds;
            if(time < 0)
            {
                GameSingleton.Instance.removeDraw(this);
                GameSingleton.Instance.updatables.Remove(this);
            }
        }

        public void draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(ContentSupplier.Instance.textures["dot"], pos, colour);
        }
    }
}
