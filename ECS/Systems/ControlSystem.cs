using System;
using System.Linq;
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
            if(entityManager.GetEntities<PlayerControlComponent>().Count == 0) // player dead
                return;
            if(entityManager.GetEntities<PlayerControlComponent>().Count > 1)
                throw new NotSupportedException("Uh-oh.. shouldn't be more than one player component");
            if(entityManager.GetEntities<CameraComponent>().Count != 1)
                throw new NotSupportedException("Uh-oh.. shouldn't be more or less than one camera component");

            Entity e = entityManager.GetEntities<PlayerControlComponent>()[0];
            var hc = e.get<HealthComponent>(); // todo: put hp reg somewhere als and for all characters
            if(hc.HP < 100f)
                hc.HP += (hc.regPerS * (float)gameTime.ElapsedGameTime.TotalSeconds);

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

            playerMove(e, move, (float)gameTime.ElapsedGameTime.TotalSeconds);
            #endregion

            cam.Position = e.get<TransformComponent>().position - cam.Origin; // center camera on player

            #region interact
            if(Keyboard.GetState().IsKeyDown(Keys.E) && prevKB.IsKeyUp(Keys.E))
            {
                // items
                var item = entityManager.getClosest<ItemComponent>(e);

                if(item != null)
                {
                    float dist = (item.get<TransformComponent>().position - e.get<TransformComponent>().position).Length();
                    if(dist < e.get<PlayerControlComponent>().interactionDistance) // if close enough to interact
                    {
                        // pick up item
                        var itemcomp = item.get<ItemComponent>();
                        var sametype = e.getAll<EquipmentComponent>().Find(x => x.type == itemcomp.type);
                        if(sametype != null && itemcomp.stackable)
                        {
                            ++sametype.amount; // stack item
                        } else
                            e.attach(new EquipmentComponent(itemcomp)); // put in inventory instead
                        entityManager.destroyEntity(item);
                    }
                }
                var speaker = entityManager.getClosest<AISimpleDialogueComponent>(e.get<TransformComponent>().position);
                if(speaker != null)
                {
                    float dist = (speaker.get<TransformComponent>().position - e.get<TransformComponent>().position).Length();
                    if(dist < e.get<PlayerControlComponent>().interactionDistance) // if close enough to interact
                    {
                        // print speech bubble
                        string speech = speaker.get<AISimpleDialogueComponent>().getNextSpeech();
                        if(speech != "")
                            entityManager.attachEntity(EntityFactory.createSpeech(speech, speaker));
                    }
                }
            }
            #endregion

            #region mouseinput
            Vector2 mousePos = Mouse.GetState().Position.ToVector2();
            e.get<TransformComponent>().lookAt(cam.ScreenToWorld(mousePos)); // player orientation

            if(Mouse.GetState().LeftButton == ButtonState.Pressed && prevMS.LeftButton == ButtonState.Released)
                playerAttack(e);
            else if(Mouse.GetState().RightButton == ButtonState.Pressed && prevMS.RightButton == ButtonState.Released)
                playerBlock(e);
            else if(Mouse.GetState().RightButton == ButtonState.Released && prevMS.RightButton == ButtonState.Pressed)
                playerUnblock(e);
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
            var sword = player.get<WieldingComponent>().wielding("sword");
            entityManager.attachEntity(EntityFactory.createAttack(atpos, player, sword != null ? sword.effect : 10));
        }

        private void playerBlock(Entity player)
        {
            player.attach(new SpriteComponent("shield"));
            //todo: save state somewhere, so damage handling can take this into account
        }
        private void playerUnblock(Entity player)
        {
            player.detach(player.getAll<SpriteComponent>().Find(x => x.spriteName == "shield"));
        }

        private void playerMove(Entity player, Vector2 move, float deltaS)
        {
            Camera2D cam = entityManager.GetEntities<CameraComponent>()[0].get<CameraComponent>().camera;
            var pos = player.get<TransformComponent>().position;
            var dir = move;
            float speed = player.get<PlayerControlComponent>().movementSpeed;

            if(dir != Vector2.Zero)
            {
                dir.Normalize(); //zero vector -> NaN
                dir *= (speed * deltaS);

                if(entityManager.collisionSystem.getCollisionKey(cam.WorldToScreen(pos + dir)) == CollisionType.NONE)
                    player.get<TransformComponent>().position += dir;
            }
        }
    }
}
