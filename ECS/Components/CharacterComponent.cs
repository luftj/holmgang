using System;
namespace holmgang.Desktop
{
    public enum CharacterFaction
    {
        PLAYER,
        WILD,
        CIV,
        ANTI,
    }

    [OnlyOne]
    public class CharacterComponent : Component
    {
        CharacterFaction faction;
        public float movementSpeed = 64f; // in px per s
        public float maxSpeed = 64f; // todo needed?
        public int h2hdamage = 10;
        public float reach = 40f;

        public bool isBlocking = false;
        public bool isThrowing = false;

        public CharacterComponent() : base()
        {
        }

        public void block()
        {
            // only block if has shield equipped
            if(owner.get<WieldingComponent>()?.wielded(ItemType.BLOCK) == null)
                return;
            owner.attach(new SpriteComponent("shield"));
            isBlocking = true;
        }

        public void unblock()
        {
            if(isBlocking)
                owner.detach(owner.getAll<SpriteComponent>().Find(x => x.spriteName == "shield"));
            isBlocking = false;
        }

        public void aimThrow()
        {
            if(isBlocking)
                return;
            if(owner.get<WieldingComponent>()?.wielded(ItemType.RANGED) == null)
                return;
            isThrowing = true;
            owner.attach(new SpriteComponent("javelin"));
        }

        public void throwWeapon()
        {
            if(!isThrowing)
                return;
            var rangedweapon = owner.get<WieldingComponent>().wielded(ItemType.RANGED);
            owner.get<WieldingComponent>().unequip(rangedweapon);
            owner.detach(rangedweapon);
            owner.detach(owner.getAll<SpriteComponent>().Find(x => x.spriteName == "javelin"));
            isThrowing = false;
            // todo: create javelin entity
            
        }
    }
}
