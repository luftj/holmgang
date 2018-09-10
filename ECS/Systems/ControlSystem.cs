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

        public ControlSystem(EntityManager entityManager) : base(entityManager)
        {
        }

        public void update(GameTime gameTime)
        {
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
            #endregion

            #region mouseinput
            Vector2 mousePos = Mouse.GetState().Position.ToVector2();
            #endregion

            foreach(Entity e in entityManager.GetEntities<PlayerControlComponent>())
            {
                Camera2D cam = entityManager.GetEntities<CameraComponent>()[0].get<CameraComponent>().camera;
                e.get<TransformComponent>().lookAt(cam.ScreenToWorld(mousePos));

                var pos = e.get<TransformComponent>().position;
                var dir = move;
                float speed = 10f; //todo: put this in a component

                if(dir != Vector2.Zero)
                {
                    dir.Normalize(); //zero vector -> NaN
                    dir *= (speed);// * (float)gameTime.ElapsedGameTime.TotalSeconds); // todo: delta time

                    if(entityManager.collisionSystem.getCollisionKey(cam.WorldToScreen(pos + dir)) == CollisionType.NONE)
                        e.get<TransformComponent>().position += dir; //todo: collision
                }

                if(Mouse.GetState().LeftButton == ButtonState.Pressed && prevMS.LeftButton == ButtonState.Released)
                {
                    var trans = e.get<TransformComponent>();
                    Vector2 atpos = new Vector2((float)Math.Cos(trans.orientation), (float)Math.Sin(trans.orientation));
                    atpos *= 30f;
                    atpos += trans.position;
                    entityManager.attachEntity(EntityFactory.createAttack(atpos, e));
                }


                cam.Position = e.get<TransformComponent>().position - cam.Origin;
            }

            prevKB = Keyboard.GetState();
            prevMS = Mouse.GetState();
        }
    }
}
