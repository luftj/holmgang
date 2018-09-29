using System;
using Microsoft.Xna.Framework;
using MonoGame.Extended.Screens;

namespace holmgang.Desktop
{
    public class LoadingScreen : Screen
    {
        bool done = false;

        //public LoadingScreen()
        //{
        //}

        public override void LoadContent()
        {
            base.LoadContent();

            ContentSupplier.Instance.LoadContent(); // this goes into loadingscreen for working around the ctor->initialise->loadcontent hassle
            GameSingleton.Instance.LoadContent();
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
