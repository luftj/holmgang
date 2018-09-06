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
        string curval;
        OptionType type;

        public OptionsItem(string text, OptionType type, params string[] values) : base(text, null)
        {
            this.values = new List<string>(values);
            this.type = type;
            curval = values[0];
            action = doNothing; // todo: seriously?
        }

        public override void draw(GameTime gameTime, SpriteBatch spriteBatch, int pos)
        {
            base.draw(gameTime, spriteBatch, pos);
            spriteBatch.DrawString(ContentSupplier.Instance.fonts["testfont"], curval, Vector2.UnitX * 200 + Vector2.UnitY * 20 * pos, selected ? Color.Yellow : Color.White);
        }

        public void doNothing()
        {

        }

        public void save()
        {
            GameSingleton.Instance.gameSettings.changeSetting(text, curval);
        }

        public void right()
        {
            if(type == OptionType.PERCENT && curval != "100")
            {
                int t = (Int32.Parse(curval));
                curval = "" + ++t;
            } else if(type == OptionType.LIST)
            {
                int i = (values.IndexOf(curval));
                int idx = ++i;
                if(idx < values.Count)
                    curval = values[idx];
            }
        }

        public void left()
        {
            if(type == OptionType.PERCENT && curval != "0")
            {
                int t = (Int32.Parse(curval));
                curval = "" + --t;
            } else if(type == OptionType.LIST)
            {
                int i = (values.IndexOf(curval));
                int idx = --i; if(idx >= 0)
                    curval = values[idx];
            }
        }
    }

    public class OptionsScreen : MenuScreen
    {
        public OptionsScreen() : base()
        {
            items.Add(new OptionsItem("Master Volume", OptionType.PERCENT, "100") { selected = true });
            items.Add(new OptionsItem("Screen Resolution", OptionType.LIST, "800x480", "1920x1080"));
            items.Add(new MenuItem("Save", saveOptions));
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
            if(items.Count > 0)
            {
                if(Keyboard.GetState().IsKeyDown(Keys.Right) && prevKB.IsKeyUp(Keys.Right))
                {
                    OptionsItem o = items[currentSelection] as OptionsItem;
                    if(o != null)
                        o.right();
                }
                if(Keyboard.GetState().IsKeyDown(Keys.Left) && prevKB.IsKeyUp(Keys.Left))
                {
                    OptionsItem o = items[currentSelection] as OptionsItem;
                    if(o != null)
                        o.left();
                }
            }

            base.Update(gameTime);
        }
    }
}
