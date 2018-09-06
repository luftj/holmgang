using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.Screens;

namespace holmgang.Desktop
{
    public class MenuItem
    {
        protected string text;
        public Action action;
        public bool selected = false;

        public MenuItem(string text, Action action)
        {
            this.text = text;
            this.action = action;
        }

        public virtual void draw(GameTime gameTime, SpriteBatch spriteBatch, int pos)
        {
            spriteBatch.DrawString(ContentSupplier.Instance.fonts["testfont"], text, Vector2.UnitY * 20 * pos, selected ? Color.Yellow : Color.White);
        }
    }

    public abstract class MenuScreen : Screen
    {
        protected List<MenuItem> items;
        protected SpriteBatch spriteBatch;

        protected KeyboardState prevKB;

        protected int currentSelection = 0;

        public MenuScreen()
        {
            items = new List<MenuItem>();
        }

        public override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GameSingleton.Instance.graphics);
            base.Initialize();
        }

        public override void Update(GameTime gameTime)
        {
            if(items.Count > 0)
            {

                if(Keyboard.GetState().IsKeyDown(Keys.Enter))
                {
                    items[currentSelection].action.Invoke();
                }

                if(Keyboard.GetState().IsKeyDown(Keys.Down) && prevKB.IsKeyUp(Keys.Down))
                {
                    items[currentSelection].selected = false;
                    currentSelection++;
                    if(currentSelection >= items.Count)
                        currentSelection = items.Count -1 ;
                    items[currentSelection].selected = true;
                }
                if(Keyboard.GetState().IsKeyDown(Keys.Up) && prevKB.IsKeyUp(Keys.Up))
                {
                    items[currentSelection].selected = false;
                    currentSelection--;
                    if(currentSelection < 0)
                        currentSelection = 0;
                    items[currentSelection].selected = true;
                }
            }

            prevKB = Keyboard.GetState();
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            GameSingleton.Instance.graphics.Clear(Color.Black);

            spriteBatch.Begin();
            int it = 0;
            foreach(MenuItem item in items)
            {
                item.draw(gameTime,spriteBatch,it);
                ++it;
            }
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
