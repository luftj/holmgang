using System;
using MonoGame.Extended;

namespace holmgang.Desktop
{
    [OnlyOne]
    public class CameraComponent : Component
    {
        public Camera2D camera;

        public CameraComponent():base()
        {
        }

        public CameraComponent(Camera2D camera) : base()
        {
            this.camera = camera;
        }
    }
}
