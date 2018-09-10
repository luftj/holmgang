using System;
namespace holmgang.Desktop
{
    public class HealthComponent : Component
    {
        public int HP;
        int maxHP;
        float regPerS;

        public HealthComponent(int maxHP)
        {
            this.maxHP = maxHP;
            HP = maxHP;
            regPerS = 0f;
        }

        public void doDamage(int amount)
        {
            if(amount <= 0)
                return;
            HP -= amount;
            if(HP < 0)
                HP = 0;
        }
    }
}
