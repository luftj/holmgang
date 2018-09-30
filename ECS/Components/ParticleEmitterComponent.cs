using System;
using Microsoft.Xna.Framework;

namespace holmgang.Desktop
{
    public struct ParticleProperties
    {
        public Vector2 velocity; // in px/s
        public float lifeSpan; // in s
        public string type;
        public Color colour;

        public ParticleProperties(Vector2 velocity,float lifeSpan, string type, Color? colour = null)
        {
            this.velocity = velocity;
            this.lifeSpan = lifeSpan;
            this.type = type;
            this.colour = colour ?? Color.White;
        }
    }

    public class ParticleEmitterComponent : Component
    {
        public ParticleEmitterComponent(float cooldown, int numparticles) : base()
        {
            totalNumParticles = numparticles;
            particleSpawnFreq = cooldown;
        }

        public int totalNumParticles;

        public float particleSpawnFreq; // in seconds between particle spawns
        public float particleSpawnCooldown = 0f;

        public ParticleProperties particleProperties;
    }
}
