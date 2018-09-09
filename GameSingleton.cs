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
            List<string> possibleValues;
            int curitem;

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

        List<GameSetting> settingsList;

        public GameSettings()
        {
            settingsList = new List<GameSetting>();
            settingsList.Add(new GameSetting("mastervol","100"));
        }
    }

    public sealed class GameSingleton : IUpdatable, IDrawable
    {
        public struct ListAction
        {
            public enum ListActionType
            {
                ADD,
                REMOVE,
                CLEAR,
                NONE,
            }
            public object item;
            public ListActionType action;

            public ListAction(object item, ListActionType action)
            {
                if(item is IDrawable || item is IUpdatable || item is CharAction)
                    this.item = item;
                else
                    throw new ArgumentException("wrong item type");
                this.action = action;
            }
        }

        private static readonly GameSingleton instance = new GameSingleton();
        public static GameSingleton Instance { get { return instance; } }


        public List<IDrawable> drawables;
        public List<CharAction> actions;
        public List<IUpdatable> updatables;
        List<ListAction> changelist;

        public GraphicsDevice graphics { private set; get; }

        public List<GameSettings.GameSetting> settingsList;

        public World world;

        static GameSingleton()
        {
        }
        private GameSingleton()
        {
            drawables = new List<IDrawable>();
            actions = new List<CharAction>();
            updatables = new List<IUpdatable>();
            changelist = new List<ListAction>();
            settingsList = new List<GameSettings.GameSetting>();

            world = new World();
            updatables.Add(world);
        }

        public void init(GraphicsDevice gd)
        {
            graphics = gd;

            settingsList.Add(new GameSettings.GameSettingPercent("mastervol", "100"));
        }

        #region changelist
        public void removeUpdate(IUpdatable comp)
        {
            changelist.Add(new ListAction(comp, ListAction.ListActionType.REMOVE));
        }
        public void addUpdate(IUpdatable comp)
        {
            changelist.Add(new ListAction(comp, ListAction.ListActionType.ADD));
        }
        public void removeDraw(IDrawable comp)
        {
            changelist.Add(new ListAction(comp, ListAction.ListActionType.REMOVE));
        }
        public void addDraw(IDrawable comp)
        {
            changelist.Add(new ListAction(comp, ListAction.ListActionType.ADD));
        }

        void performListAction(ListAction la)
        {
            switch(la.action)
            {
            case ListAction.ListActionType.REMOVE:
                if(la.item is IDrawable)
                    drawables.Remove(la.item as IDrawable);
                if(la.item is IUpdatable)
                    updatables.Remove(la.item as IUpdatable);
                else if(la.item is CharAction)
                    actions.Remove(la.item as CharAction);
                break;
            case ListAction.ListActionType.ADD:
                if(la.item is IDrawable)
                    drawables.Add(la.item as IDrawable);
                if(la.item is IUpdatable)
                    updatables.Add(la.item as IUpdatable);
                else if(la.item is CharAction)
                    actions.Add(la.item as CharAction);
                break;
            case ListAction.ListActionType.CLEAR:
                if(la.item is IDrawable)
                    drawables.Clear();
                if(la.item is IUpdatable)
                    updatables.Clear();
                if(la.item is CharAction)
                    actions.Clear();
                break;
            case ListAction.ListActionType.NONE:
                break;
            }

        }
        #endregion

        public void startGame()
        {
        }

        public void changeSetting(string key, string value)
        {
            settingsList.Find(x => x.key == key).curval = value;
        }

        public void update(GameTime gameTime)
        {
            //foreach(ListAction la in changelist)
            //    performListAction(la);
            //changelist.Clear();
            
            foreach(CharAction a in actions)
                a.update(gameTime);
            List<IUpdatable> updates = new List<IUpdatable>();
            updates.AddRange(Instance.updatables);
            foreach(IUpdatable u in updates)
                u.update(gameTime);
        }

        public void draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            world.draw(gameTime, spriteBatch);

            foreach(IDrawable d in Instance.drawables)
                d.draw(gameTime, spriteBatch);
        }
    }
}

