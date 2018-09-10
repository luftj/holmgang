using System;
using Microsoft.Xna.Framework;

namespace holmgang.Desktop
{
    public interface AIComponent
    {


        void update(GameTime gametime, Character owner);
    }
}
