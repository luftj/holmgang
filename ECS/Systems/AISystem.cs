using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace holmgang.Desktop
{
    public class AISystem : System
    {
        public AISystem(EntityManager entityManager) : base(entityManager)
        {
        }

        public void update(GameTime gameTime)
        {
            foreach(Entity e in entityManager.entities)
            {
                var list = new List<BehaviourComponent>(e.getAll<BehaviourComponent>());
                if(list == null)
                    continue;
                foreach(BehaviourComponent c in list)
                {
                    if(c is AIMoveToComponent || c is AIFollowComponent)
                    {
                        var pos = e.get<TransformComponent>().position;
                        var dir = ((c is AIMoveToComponent) ?
                                   (c as AIMoveToComponent).target :
                                   (c as AIFollowComponent).target.get<TransformComponent>().position)
                                - pos;
                        float speed = 1f; //todo: put this in a component

                        if(dir == Vector2.Zero || dir.Length() < 30f)
                            continue;
                        dir.Normalize(); //zero vector -> NaN
                        dir *= (speed);// * (float)gameTime.ElapsedGameTime.TotalSeconds); // todo: delta time

                        e.get<TransformComponent>().position += dir;
                    } else if(c is AILookAtComponent)
                    {
                        e.get<TransformComponent>().lookAt((c as AILookAtComponent).target);
                    } else if(c is AIAttackComponent)
                    {
                        var cc = (c as AIAttackComponent);
                        var targetPos = cc.target.get<TransformComponent>().position;
                        var trans = e.get<TransformComponent>();
                        trans.lookAt(targetPos);
                        var len = (targetPos - trans.position).Length();
                        if(len < cc.distance)
                        {
                            if(cc.time > cc.cooldown) //can attack
                            {
                                Vector2 testpos = targetPos - trans.position;
                                testpos.Normalize();
                                testpos *= 30f;
                                testpos += trans.position;

                                entityManager.attachEntity(EntityFactory.createAttack(testpos, e));
                                cc.time = 0.0f;
                            }
                        }
                        cc.time += gameTime.ElapsedGameTime.TotalSeconds;
                    } else if(c is AIGuardComponent)
                    {
                        Entity target = getClosest<PlayerControlComponent>(e.get<TransformComponent>().position);
                        if((target.get<TransformComponent>().position - e.get<TransformComponent>().position).Length() > ((AIGuardComponent)c).triggerDistance)
                            return;
                        entityManager.attachEntity(EntityFactory.createSpeech("HOLD IT!", e));

                        e.attach(new AIFollowComponent(target));
                        e.attach(new AIAttackComponent(target));
                        e.detach(c);
                    }
                }
            }
        }

        public Entity getClosest<T>(Vector2 pos) where T : Component
        {
            var list = entityManager.GetEntities<T>();
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
    }
}
