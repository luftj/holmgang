using System;
namespace holmgang.Desktop
{
    public class WieldingComponent : Component
    {
        public EquipmentComponent primary;
        public EquipmentComponent secondary;

        public WieldingComponent() :base()
        {
        }

        public EquipmentComponent wielding(string type)
        {
            if(primary?.type == type)
                return primary;
            if(secondary?.type == type)
                return secondary;
            return null;
        }
    }
}
