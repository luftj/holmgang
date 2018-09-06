using System;
using Microsoft.Xna.Framework;

namespace holmgang.Desktop
{
    public class AILookAt : AIComponent
    {
        Character target;

        public AILookAt(Character target)
        {
            this.target = target;
        }


        public void update(GameTime gametime, Character owner)
        {
            //base.update(gametime, owner);
            owner.lookAt(target.pos);
        }
    }
}
