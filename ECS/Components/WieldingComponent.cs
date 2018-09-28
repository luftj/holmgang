using System;
namespace holmgang.Desktop
{
    [OnlyOne]
    public class WieldingComponent : Component
    {
        public ItemComponent primary;
        public ItemComponent secondary;
        public ItemComponent ranged;

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
            if(c.durability <= 0) // don't equip broken items
                return;

            // don't equip both

            // unequip primary
            if(primary == c)
                primary = null;
            // unequip secondary
            else if(secondary == c)
                secondary = null;
            // unequip ranged
            else if(ranged == c)
                ranged = null;
            // equip primary
            else if(primary == null && c.type == "MELEE")
                primary = c;
            // equip secondary
            else if(secondary == null && c.type == "BLOCK" || c.type == "MELEE")
                secondary = c;
            else if(ranged == null && c.type == "RANGED")
                ranged = c;
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
