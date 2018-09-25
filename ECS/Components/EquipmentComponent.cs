using System;
namespace holmgang.Desktop
{
    public class EquipmentComponent : Component
    {
        public string type;
        public string name;
        public int effect;
        public int amount = 1;
        public bool stackable = true;

        public EquipmentComponent() :base(){}

        public EquipmentComponent(ItemComponent item) :base()
        {
            this.type = item.type;
            this.name = item.name;
            this.effect = item.effect;
            this.amount = item.amount;
        }
    }
}
