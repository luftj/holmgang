using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace holmgang.Desktop
{
    public class ExpirationSystem : System
    {
        public ExpirationSystem(EntityManager entityManager) : base(entityManager)
        {
        }

        public void update(GameTime gameTime)
        {
            foreach(Entity e in entityManager.entities)
            {
                if(e.has<ExpirationComponent>())
                {
                    var c = e.get<ExpirationComponent>();
                    double deltaS = gameTime.ElapsedGameTime.TotalSeconds;
                    c.timeLeft -= deltaS;
                    if(c.timeLeft <= 0.0)
                    {
                        entityManager.destroyEntity(e);
                    }
                }
            }
        }
    }
}
