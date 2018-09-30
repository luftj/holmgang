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
            var hp = player.get<HealthComponent>().HP;
            spriteBatch.DrawString(ContentSupplier.Instance.fonts["testfont"], "HP: " + hp, Vector2.Zero, Color.White); // debug
            var dottex = ContentSupplier.Instance.textures["dot"];
            spriteBatch.Draw(dottex, Mouse.GetState().Position.ToVector2() - dottex.Bounds.Size.ToVector2() / 2f, Color.White);

            if(hp <= 80)
            {
                spriteBatch.Draw(ContentSupplier.Instance.textures["hplow"], Vector2.Zero, 
                                 new Color(hp == 0 ? Color.Black : Color.DarkRed, hp == 0 ? 1f : getPulse()));
            }

            // draw equipped items
            // todo: draw backdrop for equipment
            ItemComponent[] items = { player.get<WieldingComponent>().primary, player.get<WieldingComponent>().secondary, player.get<WieldingComponent>().ranged };
            Vector2 drawpos = new Vector2(GameSingleton.Instance.graphics.Viewport.Width-10-16, 10);
            foreach(var item in items)
            {
                if(item == null)
                    continue;
                Texture2D tex = null;

                tex = ContentSupplier.Instance.textures[item.name];
                if(tex != null)
                {
                    spriteBatch.Draw(tex, drawpos, Color.White);
                }
                drawpos.Y += 26;
            }

            //draw aiming line
            if(player.get<CharacterComponent>().isThrowing)
            {
                var cam = player.get<CameraComponent>().camera;
                GeometryDrawer.drawLineGradient(cam.WorldToScreen(player.get<TransformComponent>().position), 
                                                Mouse.GetState().Position.ToVector2(), 
                                                Color.White, new Color(0)); // line fades out
            }
        }

        public void update(GameTime gameTime)
        {
            if(GameSingleton.Instance.entityManager.GetEntities<PlayerControlComponent>().Count == 0)
                return;
            player = GameSingleton.Instance.entityManager.GetEntities<PlayerControlComponent>()[0];
            float hp = player.get<HealthComponent>().HP;
            if(hp <= 30)
                pulseSpeed = 1f/120f; // 120Hz
            else if(hp <= 50)
                pulseSpeed = 1f / 80f; // 80Hz
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

