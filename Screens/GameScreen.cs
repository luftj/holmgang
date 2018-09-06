using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using MonoGame.Extended.Screens;
using MonoGame.Extended.Tiled;
using MonoGame.Extended.Tiled.Graphics;
using MonoGame.Extended.ViewportAdapters;

namespace holmgang.Desktop
{
    public class GameScreen : Screen
    {
        #region VIEW
        //int windowWidth = 800;
        //int windowHeight = 480;
        public SpriteBatch spriteBatch;
        Camera2D cam;
        HUD hud;
        #endregion

        #region CONTROL
        KeyboardState prevKB;
        MouseState prevMS;

        CollisionSystem collision;
        #endregion

        #region MODEL
        private TiledMap map;
        private TiledMapRenderer maprenderer;

        Player player = new Player(new Vector2(100, 100)); // todo: put this in a game init (gamesingleton?), load from map
        Character npc = new Character(new Vector2(400, 400));
        #endregion

        public GameScreen()
        {
            GameSingleton.Instance.drawables.Add(npc);
            GameSingleton.Instance.world.characters.Add(npc);
            npc.ai.Add(new AIStandGuard(player, 150));

            GameSingleton.Instance.drawables.Add(player);
            GameSingleton.Instance.world.characters.Add(player);
            hud = new HUD(player);
        }

        public override void Initialize()
        {
            base.Initialize();

            spriteBatch = new SpriteBatch(GameSingleton.Instance.graphics);
            maprenderer = new TiledMapRenderer(GameSingleton.Instance.graphics);

            //var viewportAdapter = new BoxingViewportAdapter(game.Window, game.GraphicsDevice, 
                                                            //game.GraphicsDevice.Viewport.Width, 
                                                            //game.GraphicsDevice.Viewport.Height);
            cam = new Camera2D(GameSingleton.Instance.graphics);
        }

        public override void LoadContent()
        {
            base.LoadContent();
            map = ContentSupplier.Instance.maps["map"];
            collision = new CollisionSystem(GameSingleton.Instance.graphics.Viewport.Width, 
                                            GameSingleton.Instance.graphics.Viewport.Height, 
                                            map, maprenderer, cam); //todo this allows collisin only in visible area
        }

        public override void Draw(GameTime gameTime)
        {
            #region collisiondetection
            // draws on separate render target, has to come first
            collision.Begin();
            collision.handleMap();
            collision.End();
            #endregion

            GameSingleton.Instance.graphics.Clear(Color.CornflowerBlue);

            // draw in world coords
            #region drawworld
            spriteBatch.Begin(transformMatrix: cam.GetViewMatrix(), samplerState: SamplerState.PointClamp);

            // map
            maprenderer.Draw(map.GetLayer("Tile Layer 1"), cam.GetViewMatrix());
            //maprenderer.Draw(map.GetLayer("collision"), cam.GetViewMatrix()); // debug

            // game objects
            GameSingleton.Instance.draw(gameTime, spriteBatch);

            spriteBatch.End();
            #endregion

            // draw in screen coords
            #region drawscreen
            spriteBatch.Begin();

            // interface
            hud.draw(gameTime, spriteBatch);
            // todo: draw other interface

            spriteBatch.End();
            #endregion

            base.Draw(gameTime);
        }

        public override void Update(GameTime gameTime)
        {
            #region uiinput
            if(GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            {
                //Hide();
                //this.IsVisible = false;
                Show<MainMenuScreen>();
            }
            #endregion

            hud.update(gameTime);

            maprenderer.Update(map, gameTime);

            foreach(CharAction a in GameSingleton.Instance.actions)
                a.update(gameTime);

            GameSingleton.Instance.update(gameTime);

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

            // check collision for player 
            //todo maybe put this somewhere else? e.g. character class
            if(collision.getCollisionKey(cam.WorldToScreen(player.pos + move * player.speed)) == CollisionType.NONE)
            {
                player.move(move);
                cam.Position = player.pos - cam.Origin;
            }
            #endregion

            #region mouseinput
            Vector2 mousePos = Mouse.GetState().Position.ToVector2();

            player.lookAt(cam.ScreenToWorld(mousePos));

            if(Mouse.GetState().LeftButton == ButtonState.Pressed && prevMS.LeftButton != ButtonState.Pressed)
            {
                player.attack();
                Console.WriteLine(collision.getCollisionKey(mousePos)); // debug
            }
            #endregion

            #region debug
            if(Keyboard.GetState().IsKeyDown(Keys.Add) && prevKB.IsKeyUp(Keys.Add))
                cam.Zoom = cam.Zoom * 2f;
            if(Keyboard.GetState().IsKeyDown(Keys.OemMinus) && prevKB.IsKeyUp(Keys.OemMinus))
                cam.Zoom = cam.Zoom / 2f;


            // debug output
            //Vector2 bla = Vector2.Zero;
            //foreach(var s in Keyboard.GetState().GetPressedKeys())
            //{
            //    Console.WriteLine(s.ToString());
            //    bla.Y -= 10;
            //    spriteBatch.DrawString(ContentSupplier.Instance.fonts["testfont"], s.ToString(), bla, Color.White);
            //}
            //Console.WriteLine(collision.getCollisionKey(cam.WorldToScreen(player.pos + move * player.speed)));
            #endregion

            prevKB = Keyboard.GetState();
            prevMS = Mouse.GetState();

            base.Update(gameTime);
        }
    }
}
