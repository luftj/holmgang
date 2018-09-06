using System;
using Microsoft.Xna.Framework;
using MonoGame.Extended.Screens;

namespace holmgang.Desktop
{
    public class LoadingScreen : Screen
    {
        bool done = false;

        public LoadingScreen()
        {
        }

        public override void LoadContent()
        {
            base.LoadContent();


            ContentSupplier.Instance.LoadContent(); // todo: this goes into loadingscreen
            done = true;
        }

        public override void Update(GameTime gameTime)
        {
            if(done)
                Show<MainMenuScreen>();

            base.Update(gameTime);
        }
    }
}
