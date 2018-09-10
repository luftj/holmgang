using System;
using System.Collections.Generic;

namespace holmgang.Desktop
{
    public class DamagingOnceComponent : Component
    {
        public int damage;
        public List<Entity> alreadyDamaged;

        public DamagingOnceComponent(int damage, Entity owner)
        {
            this.damage = damage;
            alreadyDamaged = new List<Entity>();
            alreadyDamaged.Add(owner);
        }
    }
}
