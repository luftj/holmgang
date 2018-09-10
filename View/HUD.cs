using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace holmgang.Desktop
{
    public class HUD //: IDrawable, IUpdatable
    {
        Entity player;

        float pulseSpeed = 0f;
        float pulsetimer = 0f;

        public HUD(Entity p)
        {
            player = p;
        }

        public void draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            float hp = player.get<HealthComponent>().HP;
            spriteBatch.DrawString(ContentSupplier.Instance.fonts["testfont"], "HP: " + hp, Vector2.Zero, Color.White); // debug
            spriteBatch.Draw(ContentSupplier.Instance.textures["dot"], Mouse.GetState().Position.ToVector2(), Color.White);

            if(hp <= 80)
            {
                spriteBatch.Draw(ContentSupplier.Instance.textures["hplow"], Vector2.Zero, 
                                 new Color(hp == 0 ? Color.Black : Color.DarkRed, hp == 0 ? 1f : getPulse()));
            }
        }

        public void update(GameTime gameTime)
        {
            float hp = player.get<HealthComponent>().HP;
            if(hp <= 30)
                pulseSpeed = 1f/120f; // 120Hz
            else if(hp <= 50)
                pulseSpeed = 1f / 80f; // 40Hz
            else if(hp <= 80)
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

