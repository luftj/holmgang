using System;
namespace holmgang.Desktop
{
    public class HealthComponent : Component
    {
        public float HP;
        int maxHP;
        public float regPerS = 1f;

        public HealthComponent() : base()
        {}

        public HealthComponent(int maxHP)
        {
            this.maxHP = maxHP;
            HP = maxHP;
            regPerS = 1f;
        }

        public void doDamage(int amount)
        {
            if(amount <= 0)
                return;
            HP -= amount;
            if(HP < 0)
                HP = 0;
        }

        //public override string saveComponent()
        //{
        //    string ret = "<HealthComponent>\n";
        //    ret += "HP:" + HP+"\n";
        //    ret += "</HealthComponent>\n";
        //    return ret;
        //}

    }
}
