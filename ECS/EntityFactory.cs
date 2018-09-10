using System;
using Microsoft.Xna.Framework;
using MonoGame.Extended;

namespace holmgang.Desktop
{
    public static class EntityFactory
    {
        public static Entity createCamera(Camera2D camera)
        {
            Entity ret = new Entity();
            ret.attach(new CameraComponent(camera));
            return ret;
        }

        public static Entity createPlayer(Vector2 pos)
        {
            Entity ret = new Entity();
            ret.attach(new TransformComponent(pos, 0f));
            ret.attach(new SpriteComponent("char"));
            ret.attach(new HealthComponent(100));
            ret.attach(new PlayerControlComponent());

            return ret;
        }

        public static Entity createNPC(Vector2 pos)
        {
            Entity ret = new Entity();
            ret.attach(new TransformComponent(pos, 0f));
            ret.attach(new SpriteComponent("char"));
            ret.attach(new HealthComponent(100));
            //ret.attach(new AIMoveToComponent(new Vector2(-300,200)));
            ret.attach(new AIGuardComponent(150f));

            return ret;
        }

        public static Entity createAttack(Vector2 pos, Entity owner)
        {
            Entity ret = new Entity();
            ret.attach(new TransformComponent(pos, 0f));
            ret.attach(new SpriteComponent("x"));
            ret.attach(new ExpirationComponent(0.7));
            ret.attach(new DamagingOnceComponent(10, owner));
            return ret;
        }

        public static Entity createSpeech(string text, Entity owner)
        {
            Entity ret = new Entity();
            ret.attach(new TransformComponent(owner.get<TransformComponent>().position, 0f));
            ret.attach(new TextComponent(text, "testfont"));
            ret.attach(new ExpirationComponent(3.0));
            ret.attach(new AIFollowComponent(owner));
            return ret;
        }
    }
}
