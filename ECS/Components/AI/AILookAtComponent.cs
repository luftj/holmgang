using System;
using Microsoft.Xna.Framework;

namespace holmgang.Desktop
{
    public class AILookAtComponent : BehaviourComponent
    {
        public Vector2 target;

        public AILookAtComponent(Vector2 target)
        {
            this.target = target;
        }
    }
}
