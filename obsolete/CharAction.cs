using System;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;

namespace holmgang.Desktop
{
    public class CharAction : IDrawable
    {
        public enum ActionType
        {
            Attack,
            Blood,

            NONE,
        }

        //Texture2D tex;

        public Vector2 pos;
        double time = 1.0;
        ActionType type;
        Character owner;

        public CharAction(ActionType type, Vector2 pos, Character owner)
        {
            this.pos = pos;
            this.type = type;
            this.owner = owner;

            GameSingleton.Instance.drawables.Add(this);
        }

        public bool update(GameTime gametime )
        {
            switch(type)
            {
            case ActionType.Attack:
                if(time < 1.0f) // only do once
                    break;
                List<Character> chars = GameSingleton.Instance.world.getCloseChars(pos, 30.0f);
                foreach(Character c in chars)
                {
                    if(c == owner)
                        continue;
                    if((c.pos - pos).Length() < 30.0f)
                        c.hit();
                    ContentSupplier.Instance.sounds["sound"].Play();
                }
                break;
            case ActionType.Blood:

                break;
            case ActionType.NONE:
            //case default:
                break;
            }

            time -= gametime.ElapsedGameTime.TotalSeconds;
            if(time < 0)
                GameSingleton.Instance.drawables.Remove(this);
            return time > 0.0;
        }

        public void draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(ContentSupplier.Instance.textures["x"], pos, Color.White);
        }
    }
}
