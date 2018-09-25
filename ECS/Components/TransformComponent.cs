using System;
using Microsoft.Xna.Framework;

namespace holmgang.Desktop
{
    public class TransformComponent : Component
    {
        public Vector2 position;
        public float orientation;

        public TransformComponent():base(){}
        public TransformComponent(Vector2 position, float orientation)
        {
            this.position = position;
            this.orientation = orientation;
        }

        public void lookAt(Vector2 target)
        {
            orientation = (float)Math.Atan2(target.Y - position.Y, target.X - position.X);
        }
    }
}
