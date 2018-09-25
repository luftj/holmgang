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
using MonoGame.Extended.Graphics;

namespace holmgang.Desktop
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        #region VIEW
        int windowWidth = 800;
        int windowHeight = 480;
        GraphicsDeviceManager graphics;
        #endregion

        private static bool _EXIT = false;

        public static void EXIT()
        {
            _EXIT = true;
        }

        ScreenGameComponent sgc;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.IsFullScreen = false;
            graphics.PreferredBackBufferHeight = windowHeight;
            graphics.PreferredBackBufferWidth = windowWidth;
            Window.AllowAltF4 = true;
            Window.AllowUserResizing = false;
            Window.Title = "holmgang v0.1"; // todo: doesn't do anything

            ContentSupplier.Instance.init(Content);

            sgc = new ScreenGameComponent(this);
            Components.Add(sgc);
            sgc.Register(new LoadingScreen()); // has to go first, for content loading
            sgc.Register(new MainMenuScreen());
            sgc.Register(new OptionsScreen());
            sgc.Register(new GameScreen());
            sgc.Register(new GameMenuScreen());
            sgc.Register(new InventoryScreen());
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            GameSingleton.Instance.init(graphics.GraphicsDevice);
            sgc.FindScreen<GameScreen>().init(GameSingleton.Instance.entityManager);
            sgc.FindScreen<GameMenuScreen>().init(GameSingleton.Instance.entityManager);
            sgc.FindScreen<InventoryScreen>().init(GameSingleton.Instance.entityManager);

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            //ContentSupplier.Instance.LoadContent();
            GameSingleton.Instance.entityManager.LoadContent();
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if(_EXIT)
                Exit();

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
        }
    }
}
