using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using MonoGame.Extended.Tiled;

namespace holmgang.Desktop
{
    public sealed class ContentSupplier
    {
        private static readonly ContentSupplier instance = new ContentSupplier();

        private ContentManager con;

        public Dictionary<string, Texture2D> textures;// todo use generic getter?
        public Dictionary<string, SpriteFont> fonts;
        public Dictionary<string, TiledMap> maps;
        public Dictionary<string, Song> music;
        public Dictionary<string, SoundEffect> sounds;

        static ContentSupplier(){}
        public ContentSupplier()
        {
            textures = new Dictionary<string, Texture2D>();
            fonts = new Dictionary<string, SpriteFont>();
            maps = new Dictionary<string, TiledMap>();
            music = new Dictionary<string, Song>();
            sounds = new Dictionary<string, SoundEffect>();
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
            //LoadContent();
        }

        public void LoadContent()
        {
            ContentSupplier.Instance.fonts.Add("testfont", con.Load<SpriteFont>("testfont"));
            ContentSupplier.Instance.textures.Add("char", con.Load<Texture2D>("char"));
            ContentSupplier.Instance.textures.Add("x", con.Load<Texture2D>("x"));
            ContentSupplier.Instance.textures.Add("dot", con.Load<Texture2D>("dot"));
            ContentSupplier.Instance.textures.Add("sword", con.Load<Texture2D>("sword"));
            ContentSupplier.Instance.textures.Add("shield", con.Load<Texture2D>("shield"));
            ContentSupplier.Instance.textures.Add("coin", con.Load<Texture2D>("coin"));
            ContentSupplier.Instance.maps.Add("map",con.Load<TiledMap>("test3"));
            ContentSupplier.Instance.textures.Add("hplow", con.Load<Texture2D>("vignette800x480"));
            ContentSupplier.Instance.music.Add("music", con.Load<Song>("valris"));
            ContentSupplier.Instance.sounds.Add("sound", con.Load<SoundEffect>("slashsound"));
        }
    }
}