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

            Entity player = entityManager.GetEntities<PlayerControlComponent>()[0];

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

            playerMove(player, move, (float)gameTime.ElapsedGameTime.TotalSeconds);
            #endregion

            cam.Position = player.get<TransformComponent>().position - cam.Origin; // center camera on player

            #region interact
            if(Keyboard.GetState().IsKeyDown(Keys.E) && prevKB.IsKeyUp(Keys.E))
            {
                if(!playerPickup(player))
                    playerSpeak(player);
            }
            #endregion

            if(Keyboard.GetState().IsKeyDown(Keys.G) && prevKB.IsKeyUp(Keys.G))
            {
                player.get<CharacterComponent>().aimThrow();
            } else if(Keyboard.GetState().IsKeyUp(Keys.G) && prevKB.IsKeyDown(Keys.G))
            {
                if(player.get<CharacterComponent>().isThrowing)
                    entityManager.attachEntity(EntityFactory.createJavelin(player));
                player.get<CharacterComponent>().throwWeapon();
            }

            #region mouseinput
            Vector2 mousePos = Mouse.GetState().Position.ToVector2();
            player.get<TransformComponent>().lookAt(cam.ScreenToWorld(mousePos)); // player orientation

            if(Mouse.GetState().LeftButton == ButtonState.Pressed && prevMS.LeftButton == ButtonState.Released)
                playerAttack(player);
            else if(Mouse.GetState().RightButton == ButtonState.Pressed && prevMS.RightButton == ButtonState.Released)
                player.get<CharacterComponent>().block();
            else if(Mouse.GetState().RightButton == ButtonState.Released && prevMS.RightButton == ButtonState.Pressed)
                player.get<CharacterComponent>().unblock();
            #endregion

            prevKB = Keyboard.GetState();
            prevMS = Mouse.GetState();
        }

        private void playerAttack(Entity player)
        {
            if(player.get<CharacterComponent>().isBlocking) // can't attack while blocking
                return;

            var trans = player.get<TransformComponent>();
            Vector2 atpos = new Vector2((float)Math.Cos(trans.orientation), (float)Math.Sin(trans.orientation));
            atpos *= 30f; // todo magic number -> get reach from equipped item or if none, from playercomponent
            atpos += trans.position;
            var sword = player.get<WieldingComponent>().wielded(ItemType.MELEE);
            entityManager.attachEntity(EntityFactory.createAttack(atpos, player, sword != null ? sword.effect : 10));
        }


        private void playerMove(Entity player, Vector2 move, float deltaS)
        {
            Camera2D cam = entityManager.GetEntities<CameraComponent>()[0].get<CameraComponent>().camera;
            var playerpos = player.get<TransformComponent>().position;
            var dir = move;
            float speed = player.get<CharacterComponent>().movementSpeed;

            if(dir != Vector2.Zero)
            {
                dir.Normalize(); //zero vector -> NaN
                dir *= (speed * deltaS);

                var newpos = entityManager.collisionSystem.tryMove(playerpos, playerpos + dir);
                player.get<TransformComponent>().position = newpos;
            }
        }

        private bool playerPickup(Entity player)
        {
            var item = entityManager.getClosest<ItemComponent>(player);

            if(item == null)
                return false;
            float dist = (item.get<TransformComponent>().distance(player.get<TransformComponent>()));
            if(dist > player.get<PlayerControlComponent>().interactionDistance) // if close enough to interact
                return false;
            // pick up item
            var itemcomp = item.get<ItemComponent>();
            var sametype = player.getAll<ItemComponent>().Find(x => x.type == itemcomp.type);

            if(sametype != null && itemcomp.stackable)
            {
                ++sametype.amount; // stack item
            } else
            {
                player.attach(itemcomp); // put in inventory instead
                player.get<WieldingComponent>()?.equip(itemcomp);
            }
            entityManager.destroyEntity(item);

            return true;
        }

        private bool playerSpeak(Entity player)
        {
            var speaker = entityManager.getClosest<AISimpleDialogueComponent>(player.get<TransformComponent>().position);
            if(speaker == null)
                return false;

            float dist = (speaker.get<TransformComponent>().position - player.get<TransformComponent>().position).Length();
            if(dist > player.get<PlayerControlComponent>().interactionDistance) // not close enough to interact
                return false;
            // print speech bubble
            string speech = speaker.get<AISimpleDialogueComponent>().getNextSpeech();
            if(speech == "")
                return false;
            entityManager.attachEntity(EntityFactory.createSpeech(speech, speaker));
            return true;
        }
    }
}
