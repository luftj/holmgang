using System;
namespace holmgang.Desktop
{
    public class SpriteComponent : Component
    {
        public string spriteName;

        public int layer;

        public SpriteComponent() : base(){}
        public SpriteComponent(string spriteName, int layer=1)
        {
            this.spriteName = spriteName;
            this.layer = layer;
        }
    }
}
