using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace holmgang.Desktop
{
    public class AISystem : System
    {
        public AISystem(EntityManager entityManager) : base(entityManager) { }

        public void update(GameTime gameTime)
        {
            foreach(Entity e in entityManager.GetEntities<BehaviourComponent>())
            {
                var list = new List<BehaviourComponent>(e.getAll<BehaviourComponent>());

                foreach(BehaviourComponent c in list)
                {
                    if(c is AIMoveToComponent || c is AIFollowComponent)
                    {
                        var move = (c is AIMoveToComponent) ?
                               (c as AIMoveToComponent).target :
                                                       (c as AIFollowComponent).target.get<TransformComponent>().position;
                        var pos = e.get<TransformComponent>().position;
                        var dir = move - pos;
                        float speed = e.has<CharacterComponent>() ?
                            e.get<CharacterComponent>().movementSpeed :
                             (c as AIFollowComponent).target.get<CharacterComponent>().movementSpeed; // 

                        if(dir == Vector2.Zero || dir.Length() < 30f) // todo: handle min distance (e.g. to player for attacking) where target is set instead. text uses aifollow as well!
                            continue;
                        dir.Normalize(); //zero vector -> NaN
                        dir *= (speed * (float)gameTime.ElapsedGameTime.TotalSeconds); // todo: delta time

                        aiMove(e,
                               dir);

                    } else if(c is AILookAtComponent)
                    {
                        e.get<TransformComponent>().lookAt((c as AILookAtComponent).target);
                    } else if(c is AIAttackComponent)
                    {
                        (c as AIAttackComponent).time += gameTime.ElapsedGameTime.TotalSeconds;
                        aiAttack(e, c as AIAttackComponent);

                    } else if(c is AIGuardComponent)
                    {
                        Entity target = entityManager.getClosest<PlayerControlComponent>(e.get<TransformComponent>().position);
                        if(target == null || (target.get<TransformComponent>().position - e.get<TransformComponent>().position).Length() > ((AIGuardComponent)c).triggerDistance)
                            continue;
                        entityManager.attachEntity(EntityFactory.createSpeech("HOLD IT!", e));

                        e.attach(new AIFollowComponent(target));
                        e.attach(new AIAttackComponent(target));
                        e.detach(c);
                    } else if(c is AISimpleDialogueComponent)
                    {
                        (c as AISimpleDialogueComponent).update((float)gameTime.ElapsedGameTime.TotalSeconds);
                    }
                }
            }
        }

        private void aiMove(Entity e, Vector2 dir)
        {
            var pos = e.get<TransformComponent>().position;
            //var dir = move - pos;
            //float speed = e.get<CharacterComponent>().movementSpeed; // todo: text uses aifollow as well

            //if(dir == Vector2.Zero || dir.Length() < 30f) // todo: handle min distance (e.g. to player for attacking) where target is set instead
            //    return;
            //dir.Normalize(); //zero vector -> NaN
            //dir *= (speed * deltaS); // todo: delta time

            var newpos = entityManager.collisionSystem.tryMove(pos, pos + dir);
            e.get<TransformComponent>().position = newpos;

            //if(entityManager.collisionSystem.getPassable(pos + dir))
                //e.get<TransformComponent>().position += dir;
        }

        private void aiAttack(Entity attacker, AIAttackComponent attcomp)
        {
            if(!entityManager.entities.Contains(attcomp.target))
            {
                // target already dead
                attcomp.target = null;
                attacker.detach(attcomp);
                return;
            }

            var target = attcomp.target.get<TransformComponent>();
            var attackerTransform = attacker.get<TransformComponent>();
            attackerTransform.lookAt(target.position);
            var len = attackerTransform.distance(target);
            var reach = attacker.get<WieldingComponent>()?.weaponReach ?? attacker.get<CharacterComponent>().reach;  // reach not fixed: might have ranged weapon
            if(len < reach)
            {
                if(attcomp.time < attcomp.cooldown) //can't attack yet
                    return;

                // else can attack
                Vector2 atpos = target.position - attackerTransform.position;
                atpos.Normalize();
                atpos *= reach;
                atpos += attackerTransform.position;

                int damage = attacker.get<WieldingComponent>()?.primary.effect ?? attacker.get<CharacterComponent>().h2hdamage; // get damage from weapon or char component
                entityManager.attachEntity(EntityFactory.createAttack(atpos, attacker, damage)); 
                attcomp.time = 0.0f;
            }
        }
    }
}
