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

        #endregion

        #region MODEL
        private TiledMap map;
        private TiledMapRenderer maprenderer;
        #endregion

        //ECS
        EntityManager entityManager;

        public GameScreen()
        {
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
            entityManager = new EntityManager();
            entityManager.entities.Add(EntityFactory.createPlayer(new Vector2(150, 150)));
            entityManager.entities.Add(EntityFactory.createNPC(new Vector2(50, 50)));
            entityManager.entities.Add(EntityFactory.createCamera(cam));
            entityManager.entities.Add(EntityFactory.createItem(new Vector2(200,200),"sword","test"));
            entityManager.entities.Add(EntityFactory.createItem(new Vector2(250, 200),"shield","test"));
            hud = new HUD(entityManager.GetEntities<PlayerControlComponent>()[0]);
        }

        public override void Update(GameTime gameTime)
        {
            #region uiinput
            if(Keyboard.GetState().IsKeyDown(Keys.Escape))
                Show<MainMenuScreen>();
            #endregion

            hud.update(gameTime);

            maprenderer.Update(map, gameTime); // todo: needed? animated tiles?

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
            #endregion

            entityManager.update(gameTime);

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
            //maprenderer.Draw(map.GetLayer("collision")); // debug

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
