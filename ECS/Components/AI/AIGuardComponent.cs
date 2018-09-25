using System;
namespace holmgang.Desktop
{
    public class AIGuardComponent : BehaviourComponent
    {
        public float triggerDistance;
        public bool handled = false;

        public AIGuardComponent():base(){}
        public AIGuardComponent(float triggerDistance)
        {
            this.triggerDistance = triggerDistance;
        }
    }
}
