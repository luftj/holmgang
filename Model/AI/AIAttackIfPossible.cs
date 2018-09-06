using System;
using Microsoft.Xna.Framework;

namespace holmgang.Desktop
{
    public class AIAttackIfPossible : AIComponent
    {
        double cooldown = 0.5;
        double time;
        float distance = 40;
        Character target;

        public AIAttackIfPossible(Character target)
        {
            this.target = target;
        }

        public void update(GameTime gametime, Character owner)
        {
            if((owner.pos - target.pos).Length() < distance)
            {
                if(time > cooldown) //can attack
                {
                    owner.attack();
                    time = 0.0f;
                }
            }
            time += gametime.ElapsedGameTime.TotalSeconds;
        }
    }
}
