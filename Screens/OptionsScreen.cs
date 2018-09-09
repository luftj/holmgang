using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.Screens;

namespace holmgang.Desktop
{
    public enum OptionType
    {
        PERCENT,
        LIST
    }

    public class OptionsItem : MenuItem
    {
        public List<string> values;
        public string curval;
        OptionType type;
        public GameSettings.GameSetting setting;

        //public OptionsItem(string text, OptionType type, params string[] values) : base(text, null)
        public OptionsItem(string text, GameSettings.GameSetting setting) : base(text, null)
        {
            //this.values = new List<string>(values);
            //this.type = type;
            //curval = values[0];
            action = doNothing; // todo: seriously?
            this.setting = setting;
        }

        public override void draw(GameTime gameTime, SpriteBatch spriteBatch, int pos)
        {
            base.draw(gameTime, spriteBatch, pos);
            //spriteBatch.DrawString(ContentSupplier.Instance.fonts["testfont"], curval, Vector2.UnitX * 200 + Vector2.UnitY * 20 * pos, selected ? Color.Yellow : Color.White);
            spriteBatch.DrawString(ContentSupplier.Instance.fonts["testfont"], setting.getCurVal(), Vector2.UnitX * 200 + Vector2.UnitY * 20 * pos, selected ? Color.Yellow : Color.White);
        }

        public void doNothing()
        {

        }

        public void save()
        {
            // todo: right now, change is immediately saved -> implement either temp values until save or revert funcitonality
        }

    }

    public class OptionsScreen : MenuScreen
    {
        public OptionsScreen() : base()
        {
        }

        public override void LoadContent()
        {
            base.LoadContent();

            //items.Add(new OptionsItem("Master Volume", OptionType.PERCENT, GameSingleton.Instance.settings["mastervol"]) { selected = true });
            //items.Add(new OptionsItem("Screen Resolution", OptionType.LIST, "800x480", "1920x1080") { curval = "800x480" });
            foreach(var item in GameSingleton.Instance.settingsList)
                items.Add(new OptionsItem("Master Volume", item));
            items[0].selected = true;

            //items.Add(new MenuItem("Save", saveOptions));
            items.Add(new MenuItem("Back", Show<MainMenuScreen>));
        }

        private void saveOptions()
        {
            foreach(MenuItem it in items)
            {
                OptionsItem opt = it as OptionsItem;
                if(opt != null)
                {
                    opt.save();
                }
            }
        }

        public override void Update(GameTime gameTime)
        {
            // todo: get current values from stored gamesettings when entering screen

            if(Keyboard.GetState().IsKeyDown(Keys.Escape) && prevKB.IsKeyUp(Keys.Escape))
            {
                Show<MainMenuScreen>();
            }

            if(items.Count > 0)
            {
                if(Keyboard.GetState().IsKeyDown(Keys.Right) && prevKB.IsKeyUp(Keys.Right))
                {
                    OptionsItem o = items[currentSelection] as OptionsItem;
                    if(o != null)
                        //o.right();
                        o.setting.incVal();
                }
                if(Keyboard.GetState().IsKeyDown(Keys.Left) && prevKB.IsKeyUp(Keys.Left))
                {
                    OptionsItem o = items[currentSelection] as OptionsItem;
                    if(o != null)
                        //o.left();
                        o.setting.decVal();
                }
            }
            
            base.Update(gameTime);
        }
    }
}
