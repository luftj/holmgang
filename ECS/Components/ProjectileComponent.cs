using System;
using Microsoft.Xna.Framework;

namespace holmgang.Desktop
{
    public class ProjectileComponent : Component
    {
        public Vector2 velocity;    // in px/s
        public Vector2 acceleration;

        public ProjectileComponent()
        {
        }

        public ProjectileComponent(Vector2 velocity)
        {
            this.velocity = velocity;
        }
    }
}
