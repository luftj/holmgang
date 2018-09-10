using System;
using Microsoft.Xna.Framework;

namespace holmgang.Desktop
{
    public class AIMoveTo : AIComponent
    {
        Vector2 target;

        public AIMoveTo(Vector2 target)
        {
            this.target = target;
        }

        public  void update(GameTime gametime, Character owner)
        {
            owner.move((target - owner.pos));
        }
    }
}
