using System;
using Microsoft.Xna.Framework;

namespace holmgang.Desktop
{
    public class AIMoveToComponent : BehaviourComponent
    {
        public Vector2 target;

        public AIMoveToComponent(Vector2 target)
        {
            this.target = target;
        }
    }
}
