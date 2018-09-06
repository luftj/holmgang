using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Media;
using MonoGame.Extended.Screens;

namespace holmgang.Desktop
{
    public class MainMenuScreen : MenuScreen
    {
        public bool initialised = false;

        public MainMenuScreen()
        {
            items.Add(new MenuItem("start", Show<GameScreen>) { selected = true });
            items.Add(new MenuItem("options", Show<OptionsScreen>));
            items.Add(new MenuItem("end", Game1.EXIT));
        }

        public override void Update(GameTime gameTime)
        {
            if(!initialised)
            {
                MediaPlayer.Play(ContentSupplier.Instance.music["music"]);
                initialised = true;
            }

            base.Update(gameTime);
        }
    }
}
