using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;

namespace holmgang.Desktop
{
    public class ControlSystem : System
    {
        KeyboardState prevKB;
        MouseState prevMS;

        public ControlSystem(EntityManager entityManager) : base(entityManager) { }

        public void update(GameTime gameTime)
        {
            if(entityManager.GetEntities<PlayerControlComponent>().Count != 1)
                throw new NotSupportedException("Uh-oh.. shouldn't be more or less than one player component");
            if(entityManager.GetEntities<CameraComponent>().Count != 1)
                throw new NotSupportedException("Uh-oh.. shouldn't be more or less than one camera component");

            Entity e = entityManager.GetEntities<PlayerControlComponent>()[0];

            Camera2D cam = entityManager.GetEntities<CameraComponent>()[0].get<CameraComponent>().camera;

            #region movementinput
            Vector2 move = Vector2.Zero;
            if(Keyboard.GetState().IsKeyDown(Keys.D))
                move.X = 1;
            if(Keyboard.GetState().IsKeyDown(Keys.A))
                move.X = -1;
            if(Keyboard.GetState().IsKeyDown(Keys.S))
                move.Y = 1;
            if(Keyboard.GetState().IsKeyDown(Keys.W))
                move.Y = -1;

            playerMove(e, cam, move);
            #endregion

            cam.Position = e.get<TransformComponent>().position - cam.Origin; // center camera on player

            if(Keyboard.GetState().IsKeyDown(Keys.E) && prevKB.IsKeyUp(Keys.E))
            {
                var item = entityManager.getClosest<ItemComponent>(e.get<TransformComponent>().position);
                if(e != item)
                {
                    float dist = (item.get<TransformComponent>().position - e.get<TransformComponent>().position).Length();
                    if(dist < 40f) //todo magic number
                    {
                        // pick up item
                        var itemcomp = item.get<ItemComponent>();
                        e.attach(new EquipmentComponent(itemcomp));
                        entityManager.destroyEntity(item);
                    }
                }
            }

            #region mouseinput
            Vector2 mousePos = Mouse.GetState().Position.ToVector2();
            e.get<TransformComponent>().lookAt(cam.ScreenToWorld(mousePos)); // player orientation

            if(Mouse.GetState().LeftButton == ButtonState.Pressed && prevMS.LeftButton == ButtonState.Released)
                playerAttack(e);
            #endregion

            prevKB = Keyboard.GetState();
            prevMS = Mouse.GetState();
        }

        private void playerAttack(Entity player)
        {
            var trans = player.get<TransformComponent>();
            Vector2 atpos = new Vector2((float)Math.Cos(trans.orientation), (float)Math.Sin(trans.orientation));
            atpos *= 30f;
            atpos += trans.position;
            bool hasSword = player.getAll<EquipmentComponent>().Exists(x => x.type == "sword");
            entityManager.attachEntity(EntityFactory.createAttack(atpos, player, hasSword?100:10));
        }

        private void playerMove(Entity player, Camera2D cam, Vector2 move)
        {
            var pos = player.get<TransformComponent>().position;
            var dir = move;
            float speed = 10f; //todo: put this in a component

            if(dir != Vector2.Zero)
            {
                dir.Normalize(); //zero vector -> NaN
                dir *= (speed);// * (float)gameTime.ElapsedGameTime.TotalSeconds); // todo: delta time

                if(entityManager.collisionSystem.getCollisionKey(cam.WorldToScreen(pos + dir)) == CollisionType.NONE)
                    player.get<TransformComponent>().position += dir;
            }
        }
    }
}
