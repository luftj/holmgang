using System;
namespace holmgang.Desktop
{
    [OnlyOne]
    public class PlayerControlComponent : Component
    {
        //public float movementSpeed = 100f; // px/s;
        public float interactionDistance = 40f;

        //public bool isBlocking = false;

        public PlayerControlComponent():base()
        {
        }

        //public void block()
        //{
        //    // only block if has shield equipped
        //    if(owner.get<WieldingComponent>()?.wielding("BLOCK") == null)
        //        return;
        //    owner.attach(new SpriteComponent("shield"));
        //    isBlocking = true;
        //}

        //public void unblock()
        //{
        //    if(isBlocking)
        //        owner.detach(owner.getAll<SpriteComponent>().Find(x => x.spriteName == "shield"));
        //    isBlocking = false;
        //}
    }
}
