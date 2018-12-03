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
                // bleeding particle effect
                else if(hc.HP<=50)
                {
                    if(!character.has<ParticleEmitterComponent>())
                    {
                        ParticleEmitterComponent pec = new ParticleEmitterComponent(0.8f, 10) {
                            particleProperties = new ParticleProperties("dot",
                                                                        0f,
                                                                        10f,
                                                                        0f,
                                                                        lifeSpanDeviation: 2f,
                                                                        colour: Color.DarkRed)
                        };
                        character.attach(pec);
                    }

                } else if(hc.HP <= 80)
                {
                    if(!character.has<ParticleEmitterComponent>())
                    {
                        ParticleEmitterComponent pec = new ParticleEmitterComponent(2f, 3) {
                            particleProperties = new ParticleProperties("dot",
                                                                        0f,
                                                                        10f,
                                                                        0f,
                                                                        lifeSpanDeviation: 2f,
                                                                        colour: Color.DarkRed)
                        };
                        character.attach(pec);
                    }
                }
            }
        }
    }
}
