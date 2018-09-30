using System;
using Microsoft.Xna.Framework;

namespace holmgang.Desktop
{
    public class ProjectileComponent : Component
    {
        public Vector2 velocity;    // in px/s
        public float drag = 80f;          // in px/s²?
        public Vector2 acceleration { get { 
                var ret = velocity;
                ret.Normalize();
                ret *= -drag;
                return ret;
            }}

        public ProjectileComponent() : base()
        {
        }

        public ProjectileComponent(Vector2 velocity) :this()
        {
            this.velocity = velocity;
        }
    }
}
