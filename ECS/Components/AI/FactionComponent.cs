using System;
namespace holmgang.Desktop
{
    public enum Faction
    {
        PLAYER,
        ENEMY,
        NEUTRAL,
    }

    public class FactionComponent : Component
    {
        public Faction faction;

        public FactionComponent(Faction faction)
        {
            this.faction = faction;
        }
    }
}
