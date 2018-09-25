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

    public class CharacterComponent : Component
    {
        CharacterFaction faction;
        public float movementSpeed = 64f; // in px per s

        public CharacterComponent() : base()
        {
        }
    }
}
