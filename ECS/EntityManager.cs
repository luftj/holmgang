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
        SoundSystem soundSystem;
        HealthSystem healthSystem;
        ParticleSystem particleSystem;

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
            soundSystem = new SoundSystem(this);
            healthSystem = new HealthSystem(this);
            particleSystem = new ParticleSystem(this);
        }

        public void LoadContent()
        {
            collisionSystem.LoadContent();
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
            healthSystem.update(gameTime);
            particleSystem.Update(gameTime);
            soundSystem.update();
        }

        public List<Entity> GetEntities<T>() where T : Component
        {
            List<Entity> ret = entities.FindAll(x => x.has<T>());

            return ret;
        }

        public Entity getClosest<T>(Vector2 pos) where T : Component
        {
            var list = GetEntities<T>();
            list = list.FindAll(x => x.has<TransformComponent>());
            float leastDistance = float.MaxValue;
            Entity ret = null;
            foreach(var e in list)
            {
                float cur = (e.get<TransformComponent>().position - pos).Length();
                if(cur < leastDistance)
                {
                    leastDistance = cur;
                    ret = e;
                }
            }
            return ret;
        }

        public Entity getClosest<T>(Entity self) where T : Component
        {
            var list = GetEntities<T>();
            list = list.FindAll(x => x.has<TransformComponent>());
            float leastDistance = float.MaxValue;
            Entity ret = null;
            foreach(var e in list)
            {
                if(e == self)
                    continue;
                float cur = (e.get<TransformComponent>().position - self.get<TransformComponent>().position).Length();
                if(cur < leastDistance)
                {
                    leastDistance = cur;
                    ret = e;
                }
            }
            return ret;
        }

        /// <summary>
        /// Saves the entities as string for storing game state.
        /// </summary>
        public string saveEntities()
        {
            string ret = "";
            foreach(var e in entities)
            {
                ret += e.saveEntity();
            }
            return ret;
        }

        public void loadEntities(List<Entity> entities)
        {
            this.entities.Clear();
            this.entities = entities;
        }
    }
}
