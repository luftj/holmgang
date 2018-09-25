using System;
namespace holmgang.Desktop
{
    public class PlayerControlComponent : Component
    {
        public float movementSpeed = 100f; // px/s;
        public float interactionDistance = 40f;

        public PlayerControlComponent():base()
        {
        }
    }
}
