using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace holmgang.Desktop
{
    public class GameSettings
    {
        public class GameSetting
        {
            public string key;
            public string curval;

            public GameSetting(string key, string value)
            {
                this.key = key;
                this.curval = value;
            }

            public virtual string getCurVal()
            {
                return curval;
            }

            public virtual void incVal()
            {

            }

            public virtual void decVal()
            {

            }
        }

        public class GameSettingPercent : GameSetting
        {
            public GameSettingPercent(string key, string val) : base(key,val)
            {}

            public override void incVal()
            {
                int t = (Int32.Parse(curval));
                if( t < 100)
                    curval = "" + ++t;
            }

            public override void decVal()
            {
                int t = (Int32.Parse(curval));
                if(t > 0)
                    curval = "" + --t;
            }
        }

        public class GameSettingList : GameSetting
        {
            List<string> possibleValues { get; }
            //int curitem;

            public GameSettingList(string key, string value, params string[] possVals) : base(key, value)
            {
                possibleValues = new List<string>(possVals);
            }

            public override void incVal()
            {
                int i = (possibleValues.IndexOf(curval));
                int idx = ++i;
                if(idx < possibleValues.Count)
                    curval = possibleValues[idx];
            }

            public override void decVal()
            {
                int i = (possibleValues.IndexOf(curval));
                int idx = --i; 
                if(idx >= 0)
                    curval = possibleValues[idx];
            }
        }

        List<GameSetting> settingsList { get; }

        public GameSettings()
        {
            settingsList = new List<GameSetting>();
            settingsList.Add(new GameSetting("mastervol","100"));
        }
    }

    public sealed class GameSingleton //: IUpdatable, IDrawable
    {
        private static readonly GameSingleton instance = new GameSingleton();
        public static GameSingleton Instance { get { return instance; } }

        public GraphicsDevice graphics { private set; get; }

        public List<GameSettings.GameSetting> settingsList;

        public EntityManager entityManager;

        static GameSingleton()
        {
        }
        private GameSingleton()
        {
            settingsList = new List<GameSettings.GameSetting>();
        }

        public void init(GraphicsDevice gd)
        {
            graphics = gd;

            settingsList.Add(new GameSettings.GameSettingPercent("mastervol", "100"));

            entityManager = new EntityManager();
        }

        public void startGame()
        {
            entityManager.entities.Clear();

            MonoGame.Extended.Camera2D cam = new MonoGame.Extended.Camera2D(GameSingleton.Instance.graphics);
            entityManager.entities.Add(EntityFactory.createPlayerWithCam(new Vector2(300, 280), cam));
            //entityManager.entities.Add(EntityFactory.createPlayer(new Vector2(220, 220)));
            entityManager.entities.Add(EntityFactory.createNPC(new Vector2(50, 50)));
            entityManager.entities.Add(EntityFactory.createCiv(new Vector2(-150, 250)));
            //entityManager.entities.Add(EntityFactory.createCamera(cam));
            entityManager.entities.Add(EntityFactory.createItem(new Vector2(200, 200), "MELEE", "sword", 50));
            entityManager.entities.Add(EntityFactory.createItem(new Vector2(250, 200), "BLOCK", "shield", 5));

            entityManager.entities.Add(EntityFactory.createItem(new Vector2(250, 330), "MISC", "coin", 1));
            entityManager.entities.Add(EntityFactory.createItem(new Vector2(250, 360), "MISC", "coin", 1));
            entityManager.entities.Add(EntityFactory.createItem(new Vector2(250, 590), "MISC", "coin", 1));
        }

        public void changeSetting(string key, string value)
        {
            settingsList.Find(x => x.key == key).curval = value;
        }

        public string getSetting(string key)
        {
            return settingsList.Find(x => x.key == key)?.curval;
        }
    }
}

