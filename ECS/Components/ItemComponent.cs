using System;
namespace holmgang.Desktop
{
    public class ItemComponent : Component
    {
        public string name;
        public string type;
        public int effect;
        public int amount = 1;
        public bool stackable = true;

        public ItemComponent():base(){}

        public ItemComponent(string type, string name, int effect)
        {
            this.type = type;
            this.name = name;
            this.effect = effect;
        }
    }
}
