using System;
using MonoGame.Extended;

namespace holmgang.Desktop
{
    public class CameraComponent : Component
    {
        public Camera2D camera;

        public CameraComponent(Camera2D camera)
        {
            this.camera = camera;
        }
    }
}
