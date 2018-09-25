using System;
namespace holmgang.Desktop
{
    public class TextComponent : Component
    {
        public string text;
        public string font;

        public TextComponent():base(){}
        public TextComponent(string text, string font)
        {
            this.text = text;
            this.font = font;
        }
    }
}
