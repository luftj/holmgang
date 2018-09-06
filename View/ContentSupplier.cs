using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Tiled;

namespace holmgang.Desktop
{
    public sealed class ContentSupplier
    {
        private static readonly ContentSupplier instance = new ContentSupplier();

        private ContentManager con;

        public Dictionary<string, Texture2D> textures;
        public Dictionary<string, SpriteFont> fonts;
        public Dictionary<string, TiledMap> maps;

        static ContentSupplier(){}
        public ContentSupplier()
        {
            textures = new Dictionary<string, Texture2D>();
            fonts = new Dictionary<string, SpriteFont>();
            maps = new Dictionary<string, TiledMap>();
        }

        public static ContentSupplier Instance {
            get {
                return instance;
            }
        }

        public void init(ContentManager con)
        {
            this.con = con;
            con.RootDirectory = "Content";
        }

        public void LoadContent()
        {
            ContentSupplier.Instance.fonts.Add("testfont", con.Load<SpriteFont>("testfont"));
            ContentSupplier.Instance.textures.Add("char", con.Load<Texture2D>("char"));
            ContentSupplier.Instance.textures.Add("x", con.Load<Texture2D>("x"));
            ContentSupplier.Instance.textures.Add("dot", con.Load<Texture2D>("dot"));
            ContentSupplier.Instance.maps.Add("map",con.Load<TiledMap>("test2"));
            ContentSupplier.Instance.textures.Add("hplow", con.Load<Texture2D>("vignette800x480"));
        }
    }
}