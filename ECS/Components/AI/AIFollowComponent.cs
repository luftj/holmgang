using System;
namespace holmgang.Desktop
{
    public class AIFollowComponent : BehaviourComponent
    {
        public Entity target;

        public AIFollowComponent():base(){}
        public AIFollowComponent(Entity target)
        {
            this.target = target;
        }
    }
}
