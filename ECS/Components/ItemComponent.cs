using System;
namespace holmgang.Desktop
{
    public class ItemComponent : Component
    {
        public string name;
        public string type;

        public ItemComponent(string type, string name)
        {
            this.type = type;
            this.name = name;
        }
    }
}
