using System;
using Microsoft.Xna.Framework.Media;

namespace holmgang.Desktop
{
    public class SoundSystem : System
    {
        public SoundSystem(EntityManager entityManager) : base(entityManager){}

        public void update()
        {
            string bla = GameSingleton.Instance.settingsList.Find(x => x.key == "mastervol").curval;
            MediaPlayer.Volume = (Int32.Parse(bla))/100f; // todo: necessary in every frame? maybe put this in options screen

            foreach(var e in entityManager.GetEntities<SoundComponent>())
            {
                var c = e.get<SoundComponent>();
                c.play(); // plays sound effect if it has not been played yet
                if(!c.active()) // delete component after it is done
                    e.detach(c);
            }
        }
    }
}
