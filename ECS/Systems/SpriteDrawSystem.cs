using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;

namespace holmgang.Desktop
{
    public class SpriteDrawSystem : System
    {
        public SpriteDrawSystem(EntityManager entityManager) : base(entityManager)
        {
        }

        public void Update(GameTime gameTime)
        {
            // animate sprites here
        }

        public void draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            foreach(Entity e in entityManager.entities)
            {
                if(e.has<SpriteComponent>() && e.has<TransformComponent>())
                {
                    Color col = e.has<PlayerControlComponent>() ? Color.Blue : Color.White;
                    Texture2D tex = ContentSupplier.Instance.textures[e.get<SpriteComponent>().spriteName];
                    spriteBatch.Draw(texture: tex, 
                                     position: e.get<TransformComponent>().position,
                                     origin: tex.Bounds.Size.ToVector2()/2f,
                                     rotation: e.get<TransformComponent>().orientation, 
                                     color: col);
                }
                if(e.has<TextComponent>() && e.has<TransformComponent>())
                {
                    SpriteFont font = ContentSupplier.Instance.fonts[e.get<TextComponent>().font];
                    spriteBatch.DrawString(font,
                                           e.get<TextComponent>().text,
                                           e.get<TransformComponent>().position,
                                           Color.White);
                }
            }
        }
    }
}
