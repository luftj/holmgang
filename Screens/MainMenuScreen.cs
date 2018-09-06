using System;
using Microsoft.Xna.Framework;
using MonoGame.Extended.Screens;

namespace holmgang.Desktop
{
    public class MainMenuScreen : MenuScreen
    {
        public MainMenuScreen()
        {
            items.Add(new MenuItem("start", Show<GameScreen>) { selected = true
        });
            items.Add(new MenuItem("end", Game1.EXIT));
        }

        public override void LoadContent()
        {
            base.LoadContent();

            //ContentSupplier.Instance.LoadContent(game.Content); // todo: this goes into loadingscreen
        }

        public override void Draw(GameTime gameTime)
        {
            GameSingleton.Instance.graphics.Clear(Color.Black);

            base.Draw(gameTime);
        }
    }
}
