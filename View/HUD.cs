using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace holmgang.Desktop
{
    public class HUD : IDrawable, IUpdatable
    {
        Player player;

        float pulseSpeed = 0f;
        float pulsetimer = 0f;

        public HUD(Player p)
        {
            player = p;
        }

        public void draw(GameTime gameTime, SpriteBatch spriteBatch)
        {

            spriteBatch.DrawString(ContentSupplier.Instance.fonts["testfont"], "HP: " + player.HP, Vector2.Zero, Color.White); // debug
            spriteBatch.Draw(ContentSupplier.Instance.textures["dot"], Mouse.GetState().Position.ToVector2(), Color.White);

            if(player.HP <= 80)
            {
                spriteBatch.Draw(ContentSupplier.Instance.textures["hplow"], Vector2.Zero, 
                                 new Color(player.HP == 0 ? Color.Black : Color.DarkRed, player.HP == 0 ? 1f : getPulse()));
            }
        }

        public void update(GameTime gameTime)
        {
            if(player.HP <= 30)
                pulseSpeed = 1f/120f; // 120Hz
            else if(player.HP <= 50)
                pulseSpeed = 1f / 80f; // 40Hz
            else if(player.HP <= 80)
                pulseSpeed = 1f/40f; // 40Hz
            else
                pulseSpeed = 0;

            float deltaM = (float)gameTime.ElapsedGameTime.TotalMinutes;
            pulsetimer += deltaM;
            if(pulsetimer >= pulseSpeed)
                pulsetimer -= pulseSpeed;
        }

        private float getPulse()
        {
            float cur = pulsetimer / pulseSpeed;
            if(cur > 0.5f)
                return 2f*(1f - cur);
            return 2f*cur;
        }
    }
}

