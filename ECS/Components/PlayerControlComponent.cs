using System;
namespace holmgang.Desktop
{
    [OnlyOne]
    public class PlayerControlComponent : Component
    {
        public float movementSpeed = 100f; // px/s;
        public float interactionDistance = 40f;

        public bool isBlocking = false;

        public PlayerControlComponent():base()
        {
        }
    }
}
