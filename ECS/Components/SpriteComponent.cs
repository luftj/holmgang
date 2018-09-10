using System;
namespace holmgang.Desktop
{
    public class SpriteComponent : Component
    {
        public string spriteName;

        public SpriteComponent(string spriteName)
        {
            this.spriteName = spriteName;
        }
    }
}
