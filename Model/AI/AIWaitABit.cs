using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace holmgang.Desktop
{
    public class AIWaitABit :AIComponent
    {
        double time;
        List<AIComponent> queue;

        public AIWaitABit(List<AIComponent> queue, float whatbit)
        {
            time = whatbit;
            this.queue = queue;
        }

        public void update(GameTime gametime, Character owner)
        {
            if(time > 0)
            {
                time -= gametime.ElapsedGameTime.TotalSeconds;
            } else
            {
                owner.aiaddlist.AddRange(queue);
                queue.Clear();
            }
        }
    }
}
