using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace holmgang.Desktop
{
    public class World : IUpdatable, IDrawable
    {
        public List<Character> characters = new List<Character>();

        public World()
        {

        }


        public List<Character> getCloseChars(Vector2 pos, float dist)
        {
            List<Character> ret = new List<Character>();
            foreach(Character c in characters)
                if((pos - c.pos).Length() < dist)
                    ret.Add(c);
            return ret;
        }

        public void update(GameTime gameTime)
        {
            List<Character> updates = new List<Character>();
            updates.AddRange(characters);
            foreach(Character c in updates)
                c.update(gameTime);
        }

        public void draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
        }
    }
}
