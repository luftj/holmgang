using System;
using Microsoft.Xna.Framework;

namespace holmgang.Desktop
{
    public struct ParticleProperties
    {
        //public Vector2 velocity; // in px/s
        public float lifeSpan; // in s
        public float lifeSpanDeviation;
        public string type;
        public Color colour;
        public float direction;
        public float directionDeviation;
        public float speed;
        public float speedDeviation;

        //public ParticleProperties(Vector2 velocity,float lifeSpan, string type, Color? colour = null)
        //{
        //    this.velocity = velocity;
        //    this.lifeSpan = lifeSpan;
        //    this.type = type;
        //    this.colour = colour ?? Color.White;
        //}

        public ParticleProperties(string type,
                                  float direction,
                                  float lifeSpan,
                                  float speed,
                                  float directionDeviation =0f,
                                  float lifeSpanDeviation =0f,
                                  float speedDeviation = 0f,
                                  Color? colour = null)
        {
            this.direction = direction;
            this.directionDeviation = directionDeviation;
            this.speed = speed;
            this.speedDeviation = speedDeviation;
            this.lifeSpan = lifeSpan;
            this.lifeSpanDeviation = lifeSpanDeviation;
            this.type = type;
            this.colour = colour ?? Color.White;
        }

        public ParticleProperties(float direction,
                                  float deviation,
                                  float speed,
                                  float lifeSpan,
                                  string type,
                                  Color? colour = null) :
        this(type, direction, lifeSpan, speed, colour: colour){}
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
