using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace holmgang.Desktop
{
    public class HealthSystem : System
    {
        public HealthSystem(EntityManager entityManager) : base(entityManager)
        {
        }

        public void update(GameTime gameTime)
        {
            List<Entity> list = entityManager.GetEntities<HealthComponent>();
            foreach(Entity character in list)
            {
                var hc = character.get<HealthComponent>();
                hc.regen((float)gameTime.ElapsedGameTime.TotalSeconds);
                if(hc.HP <= 0)
                {
                    var camc = character.get<CameraComponent>();
                    if(camc != null)
                    {
                        character.detach(camc);  // stick player cam to dying position
                        var cam = EntityFactory.createCamera(camc.camera);
                        entityManager.attachEntity(cam);
                    }
                    entityManager.destroyEntity(character); // u ded :(
                }
            }
        }
    }
}
