using System;
namespace holmgang.Desktop
{
    public enum ItemType
    {
        NONE =0,
        MELEE,
        BLOCK,
        RANGED,
        CONSUMABLE,
        MISC,

        UNKNOWN,
    } // enums can be parsed from strings

    public class ItemComponent : Component
    {
        public string name;
        public ItemType type;
        public int effect;
        public int amount = 1;
        public bool stackable = true; // todo: max stack size?

        public int durability = 100;
        public int maxDurability = 100;

        public float reach = 40f;

        public ItemComponent():base(){}

        public ItemComponent(ItemType type, string name, int effect)
        {
            this.type = type;
            this.name = name;
            this.effect = effect;
        }


        public void use()
        {
            // use this for consumables or rune power
        }

        public void damage(int damage)
        {
            durability -= damage;
            if(durability <= 0)
            {
                durability = 0;
                owner.get<WieldingComponent>().unequip(this); // consider: can unequipped items get damaged?
                if(type == ItemType.BLOCK)
                {
                    owner.get<CharacterComponent>()?.unblock();
                }
            }
        }
    }
}
