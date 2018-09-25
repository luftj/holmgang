using System;
using Microsoft.Xna.Framework.Audio;

namespace holmgang.Desktop
{
    public class SoundComponent : Component
    {
        public string sound;
        public SoundEffectInstance instance;
        bool alreadyplayed = false;

        public SoundComponent() : base(){}
        public SoundComponent(string sound)
        {
            this.sound = sound;
            instance = ContentSupplier.Instance.sounds[sound].CreateInstance();
        }

        public void play()
        {
            if(alreadyplayed)
                return;
            instance.Volume = Int32.Parse(GameSingleton.Instance.getSetting("mastervol"))/100f;
            instance.Play();
            alreadyplayed = true;
        }

        public bool active()
        {
            return instance.State == SoundState.Playing;
        }
    }
}
