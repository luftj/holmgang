using System;
namespace holmgang.Desktop
{
    public class EquipmentComponent : Component
    {
        public string type;
        public string name;

        public EquipmentComponent(ItemComponent item)
        {
            this.type = item.type;
            this.name = item.name;
        }
    }
}
