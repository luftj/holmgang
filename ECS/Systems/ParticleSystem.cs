using System;
using Microsoft.Xna.Framework;

namespace holmgang.Desktop
{
    public class ParticleSystem : System
    {
        public ParticleSystem(EntityManager entityManager) : base(entityManager)
        {
        }

        public void Update(GameTime gameTime)
        {
            foreach(var particleEmitter in entityManager.GetEntities<ParticleEmitterComponent>())
            {
                // update cooldowns
                var pec = particleEmitter.get<ParticleEmitterComponent>();
                pec.particleSpawnCooldown -= (float)gameTime.ElapsedGameTime.TotalSeconds;
                if(pec.particleSpawnCooldown <= 0f)
                {
                    pec.particleSpawnCooldown = pec.particleSpawnFreq; // reset timer
                    --pec.totalNumParticles;
                    //spawn new particle
                    entityManager.attachEntity(EntityFactory.createParticle(particleEmitter.get<TransformComponent>().position, pec.particleProperties));

                    if(pec.totalNumParticles == 0)
                    {
                        // no more particles, emitter not needed anymore
                        particleEmitter.detach(pec);
                    }
                }
            }
            foreach(var particle in entityManager.GetEntities<ParticleComponent>())
            {
                particle.get<TransformComponent>().position += particle.get<ParticleComponent>().velocity * (float)gameTime.ElapsedGameTime.TotalSeconds;
            }
        }
    }
}
