using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace holmgang.Desktop
{
    public class GameSettings
    {
        public int MasterVolume = 100;

        public void changeSetting(string key, string value)
        {

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
        public GameSettings gameSettings;

        public World world;


        //private TiledMap map;
        //private TiledMapRenderer maprenderer;
        //CollisionSystem collision;

        static GameSingleton()
        {
        }
        private GameSingleton()
        {
            drawables = new List<IDrawable>();
            actions = new List<CharAction>();
            updatables = new List<IUpdatable>();
            changelist = new List<ListAction>();
            gameSettings = new GameSettings();

            world = new World();
            updatables.Add(world);
        }

        public void init(GraphicsDevice gd)
        {
            graphics = gd;
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

            //map = ContentSupplier.Instance.maps["map"];
            //collision = new CollisionSystem(GameSingleton.Instance.graphics.Viewport.Width,
                                            //GameSingleton.Instance.graphics.Viewport.Height,
                                            //map, maprenderer, cam); //todo this allows collisin only in visible area
        }

        public void update(GameTime gameTime)
        {
            //foreach(ListAction la in changelist)
            //    performListAction(la);
            //changelist.Clear();

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

