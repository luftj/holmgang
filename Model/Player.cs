using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace holmgang.Desktop
{
    public class Player : Character
    {

        public Player(Vector2 pos) : base(pos)
        {
            base.colour = Color.Blue;
            speed = 11f;
        }


        //public new void draw(GameTime gameTime, SpriteBatch spriteBatch, Camera camera)
        //{
        //    if(HP > 0)//new Vector2(400,240)
        //        spriteBatch.Draw(ContentSupplier.Instance.textures["char"], pos, null, colour, orient, size/2, 1.0f, SpriteEffects.None, 0);
        //}
    }
}
