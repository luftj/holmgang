using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace holmgang.Desktop
{
    public class EntityManager //: IUpdatable
    {
        public List<Entity> entities; // todo: write efficient getters -> bitmask of components
        private List<Entity> removeList;
        private List<Entity> addList;

        public SpriteDrawSystem spriteDrawService;
        ExpirationSystem expirationSystem;
        AISystem aIService;
        public CollisionSystem collisionSystem;
        ControlSystem controlSystem;

        public EntityManager()
        {
            entities = new List<Entity>();
            removeList = new List<Entity>();
            addList = new List<Entity>();
            spriteDrawService = new SpriteDrawSystem(this);
            expirationSystem = new ExpirationSystem(this);
            aIService = new AISystem(this);
            collisionSystem = new CollisionSystem(this);
            controlSystem = new ControlSystem(this);
        }

        public void destroyEntity(Entity e)
        {
            removeList.Add(e);
        }

        public void attachEntity(Entity e)
        {
            addList.Add(e);
        }

        public void update(GameTime gameTime)
        {
            entities.RemoveAll(x => removeList.Contains(x));
            removeList.Clear();
            entities.AddRange(addList);
            addList.Clear();

            aIService.update(gameTime);
            spriteDrawService.Update(gameTime);
            expirationSystem.update(gameTime);
            collisionSystem.update(gameTime);
            controlSystem.update(gameTime);
        }

        public List<Entity> GetEntities<T>() where T : Component
        {
            List<Entity> ret = entities.FindAll(x => x.has<T>());

            return ret;
        }
    }
}
