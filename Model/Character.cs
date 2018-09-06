using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace holmgang.Desktop
{
    public class Character : IDrawable
    {
        public Vector2 pos;
        public Vector2 size;
        public float orient;
        public Color colour;

        public float speed = 1.0f;
        public int HP = 100;

        public List<AIComponent> ai;
        public List<AIComponent> aiaddlist;

        public Character(Vector2 pos)
        {
            this.pos = pos;
            colour = Color.White;
            ai = new List<AIComponent>();
            aiaddlist = new List<AIComponent>();
            size = Vector2.One * 30;//size = ContentSupplier.Instance.textures["char"].Bounds.Size.ToVector2();
        }

        public void lookAt(Vector2 point)
        {
            orient = (float)Math.Atan2(point.Y - pos.Y, point.X - pos.X);
        }

        public void move(Vector2 dir)
        {
            if(dir == Vector2.Zero)
                return;
            dir.Normalize(); //zero vector -> NaN
            dir *= speed;
            pos += dir;
        }

        public void update(GameTime gametime)
        {
            if(HP <= 0)
            {
                GameSingleton.Instance.world.characters.Remove(this); // todo: remove all components as well
            }

            if(aiaddlist.Count > 0)
            {
                ai.Clear();
                ai.AddRange(aiaddlist);
                aiaddlist.Clear();
            }

            foreach(AIComponent c in ai)
                c.update(gametime, this);
        }

        public void draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            if(HP > 0)
                spriteBatch.Draw(ContentSupplier.Instance.textures["char"], pos, null, colour, orient, size/2, 1.0f, SpriteEffects.None, 0);

        }

        public void attack()
        {
            Vector2 pos;
            pos.X = (float)Math.Cos(orient) * 30;
            pos.Y = (float)Math.Sin(orient) * 30;
            pos -= size / 2;
            pos += this.pos;

            GameSingleton.Instance.actions.Add(new CharAction(CharAction.ActionType.Attack, pos, this));
        }

        public void hit()
        {
            HP -= 10;

            Random rnd = new Random();
            for(int i = 0; i < 10; ++i)
            {
                double dir = orient;
                dir += rnd.NextDouble() * Math.PI/2 - Math.PI/4;
                float x = (float)Math.Cos(dir) * 10;
                float y = (float)Math.Sin(dir) * 10;
                Vector2 rand = new Vector2(x, y);
                rand.Normalize();
                float time = (float)rnd.NextDouble() + 1.0f;
                Particle p = new Particle(pos, time, rand);

                GameSingleton.Instance.drawables.Add(p);
                GameSingleton.Instance.updatables.Add(p);
            }
        }
    }
}
