using System;
namespace holmgang.Desktop
{
    public enum CharacterFaction
    {
        PLAYER,
        WILD,
        CIV,
        ANTI,
    }

    [OnlyOne]
    public class CharacterComponent : Component
    {
        CharacterFaction faction;
        public float movementSpeed = 64f; // in px per s
        public const float maxSpeed = 64f; // todo needed?

        public bool isBlocking = false;

        public CharacterComponent() : base()
        {
        }
    }
}
