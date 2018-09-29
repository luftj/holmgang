using System;
using System.Collections.Generic;
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
        bool firstframe = true;
        #endregion

        #region MODEL
        private TiledMap map;
        private TiledMapRenderer maprenderer;
        #endregion

        //ECS
        EntityManager entityManager;

        //public GameScreen(EntityManager entityManager)
        //{
        //    this.entityManager = entityManager;
        //}

        public void init(EntityManager entityManager)
        {
            this.entityManager = entityManager;
        }

        public override void Initialize()
        {
            base.Initialize();

            spriteBatch = new SpriteBatch(GameSingleton.Instance.graphics);
            maprenderer = new TiledMapRenderer(GameSingleton.Instance.graphics);

            //var viewportAdapter = new BoxingViewportAdapter(game.Window, game.GraphicsDevice, 
                                                            //game.GraphicsDevice.Viewport.Width, 
                                                            //game.GraphicsDevice.Viewport.Height);
        }

        public override void LoadContent()
        {
            base.LoadContent();
            map = ContentSupplier.Instance.maps[GameSingleton.Instance.currentmap];

            GameSingleton.Instance.startGame();

            hud = new HUD(entityManager.GetEntities<PlayerControlComponent>()[0]);
        }

        public override void Update(GameTime gameTime)
        {
            #region uiinput
            if(!firstframe)
            {
                if(Keyboard.GetState().IsKeyDown(Keys.Escape) && prevKB.IsKeyUp(Keys.Escape))
                {
                    firstframe = true;
                    Show<GameMenuScreen>();
                }
                if(Keyboard.GetState().IsKeyDown(Keys.I) && prevKB.IsKeyUp(Keys.I))
                {
                    firstframe = true;
                    Show<InventoryScreen>();
                }
            } else
            {
                firstframe = false;
                cam = entityManager.GetEntities<CameraComponent>()[0].get<CameraComponent>().camera;
                map = GameSingleton.Instance.map;
            }
            #endregion

            hud.update(gameTime);

            //maprenderer.Update(map, gameTime); // todo: needed? animated tiles?

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
            //Vector2 m = cam.ScreenToWorld((float)Mouse.GetState().X, (float)Mouse.GetState().Y);
            //Console.WriteLine(m.ToString() + "---" + collision.getPassable(m)); //m.X, m.Y));
            //var dolay = map.GetLayer<TiledMapObjectLayer>("doors");
            //var ob = dolay.Objects[0];
            //Console.WriteLine(ob.Position + " --- " + ob.Size);
            //foreach(var i in ob.Properties)
                //Console.WriteLine(i.Key + i.Value);
            #endregion

            entityManager.update(gameTime); // all the magic happens here

            prevKB = Keyboard.GetState();
            prevMS = Mouse.GetState();
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            #region collisiondetection
            entityManager.collisionSystem.Begin();
            entityManager.collisionSystem.handleMap();
            entityManager.collisionSystem.End();
            #endregion

            GameSingleton.Instance.graphics.Clear(Color.CornflowerBlue);

            // draw in world coords
            #region drawworld
            spriteBatch.Begin(transformMatrix: cam.GetViewMatrix(), samplerState: SamplerState.PointClamp);

            // map
            maprenderer.Draw(map.GetLayer("Tile Layer 1"), cam.GetViewMatrix());
            //maprenderer.Draw(map.GetLayer("collision"), cam.GetViewMatrix()); // debug

            // game objects
            entityManager.spriteDrawService.draw(gameTime, spriteBatch);

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
    }
}
