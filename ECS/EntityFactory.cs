﻿using System;
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
            ret.attach(new CharacterComponent() { movementSpeed = 100f });
            ret.attach(new PlayerControlComponent());
            ret.attach(new WieldingComponent());
            return ret;
        }

        public static Entity createPlayerWithCam(Vector2 pos,Camera2D camera)
        {
            Entity ret = createPlayer(pos);
            ret.attach(new CameraComponent(camera));
            return ret;
        }

        public static Entity createItem(Vector2 pos, ItemType type, string name, int effect)
        {
            Entity ret = new Entity();
            ret.attach(new TransformComponent(pos, 0f));
            ret.attach(new SpriteComponent(name));      // todo think of something clever to query sprite name from itemcomponent
            ret.attach(new ItemComponent(type,name,effect));
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
            ret.attach(new CharacterComponent());
            ret.attach(new WieldingComponent() { primary = new ItemComponent(ItemType.MELEE, "sword", 20) });
            return ret;
        }

        public static Entity createCiv(Vector2 pos)
        {
            Entity ret = new Entity();
            ret.attach(new TransformComponent(pos, 0f));
            ret.attach(new SpriteComponent("char"));
            ret.attach(new HealthComponent(100));
            //ret.attach(new AIMoveToComponent(new Vector2(-300,200)));
            ret.attach(new AISimpleDialogueComponent("Hey.", "Fuck off."));
            ret.attach(new CharacterComponent());
            return ret;
        }

        public static Entity createAttack(Vector2 pos, Entity owner, int damage)
        {
            Entity ret = new Entity();
            ret.attach(new TransformComponent(pos, 0f));
            ret.attach(new SpriteComponent("x"));
            ret.attach(new ExpirationComponent(0.7));
            ret.attach(new DamagingOnceComponent(damage, owner));
            ret.attach(new SoundComponent("sound"));
            return ret;
        }

        public static Entity createSpeech(string text, Entity owner)
        {
            Entity ret = new Entity();
            ret.attach(new TransformComponent(owner.get<TransformComponent>().position+Vector2.UnitX*20f, 0f));
            ret.attach(new TextComponent(text, "testfont"));
            ret.attach(new ExpirationComponent(3.0));
            ret.attach(new AIFollowComponent(owner));
            return ret;
        }

        public static Entity createJavelin(Entity owner)
        {
            Entity ret = new Entity();
            ret.attach(new TransformComponent(owner.get<TransformComponent>().position, owner.get<TransformComponent>().orientation + 0.7f));
            ret.attach(new SpriteComponent("javelin"));
            ret.attach(new DamagingOnceComponent(120, owner));
            ret.attach(new SoundComponent("sound"));
            ret.attach(new ExpirationComponent(10.0));
            Vector2 velocity;
            velocity.X = (float)Math.Cos(owner.get<TransformComponent>().orientation);
            velocity.Y = (float)Math.Sin(owner.get<TransformComponent>().orientation);
            velocity *= 20f;
            ret.attach(new ProjectileComponent(velocity));
            return ret;
        }
    }
}
