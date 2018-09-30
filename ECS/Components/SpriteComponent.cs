using System;
using Microsoft.Xna.Framework;

namespace holmgang.Desktop
{
    public class SpriteComponent : Component
    {
        public string spriteName;
        public Color colour;
        public int layer;

        public SpriteComponent() : base(){}
        public SpriteComponent(string spriteName, Color? colour = null, int layer=1)
        {
            this.spriteName = spriteName;
            this.layer = layer;
            this.colour = colour ?? Color.White;
        }
    }
}
