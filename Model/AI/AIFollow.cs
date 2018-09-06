using System;
using Microsoft.Xna.Framework;

namespace holmgang.Desktop
{
    public class AIFollow : AIComponent
    {
        Character target;

        public AIFollow(Character target)
        {
            this.target = target;
        }

        public void update(GameTime gametime, Character owner)
        {
            Vector2 delta = target.pos - owner.pos;
            if(delta.Length() > 30) // todo: magic number
                owner.move(delta);
        }
    }
}
