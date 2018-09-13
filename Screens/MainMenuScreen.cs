using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Media;
using MonoGame.Extended.Screens;

namespace holmgang.Desktop
{
    public class MainMenuScreen : MenuScreen
    {
        public bool initialised = false;

        //public MainMenuScreen()
        //{
        //}

        public override void LoadContent()
        {
            base.LoadContent();

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
            string bla = GameSingleton.Instance.settingsList.Find(x => x.key == "mastervol").curval;
            MediaPlayer.Volume = (Int32.Parse(bla))/100f; // todo: move this somewhere else -> separate soundhandler class?
            base.Update(gameTime);
        }
    }
}
