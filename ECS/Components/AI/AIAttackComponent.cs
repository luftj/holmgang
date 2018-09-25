using System;
namespace holmgang.Desktop
{
    public class AIAttackComponent : BehaviourComponent
    {
        public double cooldown = 0.5;
        public double time = 0.0;
        public float distance = 40;
        public Entity target;

        public AIAttackComponent():base(){}
        public AIAttackComponent(Entity target)
        {
            this.target = target;
        }
    }
}
