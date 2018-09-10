using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace holmgang.Desktop
{
    public class AIStandGuard : AIComponent
    {
        Player target;
        float distance;

        public AIStandGuard(Player target, float viewingrange)
        {
            this.target = target;
            distance = viewingrange;
        }

        public void update(GameTime gametime, Character owner)
        {
            //base.update(gametime, owner);

            if((target.pos - owner.pos).Length() > distance)
                return;

            owner.aiaddlist.Add(new AILookAt(target));
            owner.aiaddlist.Add(new AISay("HOLD IT!", 2));
            List<AIComponent> q = new List<AIComponent>();
            q.Add(new AIFollow(target));
            q.Add(new AILookAt(target));
            q.Add(new AISay("I'll get you!", 2));
            q.Add(new AIAttackIfPossible(target));
            owner.aiaddlist.Add(new AIWaitABit(q,2));
        }
    }
}
