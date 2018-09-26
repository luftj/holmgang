using System;
namespace holmgang.Desktop
{
    public class WieldingComponent : Component
    {
        public ItemComponent primary;
        public ItemComponent secondary;

        public WieldingComponent() :base()
        {
        }

        public ItemComponent wielding(string type)
        {
            if(primary?.type == type)
                return primary;
            if(secondary?.type == type)
                return secondary;
            return null;
        }

        public void equip(ItemComponent c)
        {
            // don't equip both

            // unequip primary
            if(primary == c)
                primary = null;
            // unequip secondary
            else if(secondary == c)
                secondary = null;
            // equip primary
            else if(primary == null)
                primary = c;
            // equip secondary
            else if(secondary == null)
                secondary = c;
            //else: slots full, can't equip
        }

        public void unequip(ItemComponent c)
        {
            if(primary == c)
                primary = null;
            if(secondary == c)
                secondary = null;
        }
    }
}
