using System;
using System.IO;

namespace holmgang.Desktop
{
    public class GameMenuScreen : MenuScreen
    {
        EntityManager entityManager;

        public void init(EntityManager entityManager)
        {
            this.entityManager = entityManager;
        }

        public override void LoadContent()
        {
            base.LoadContent();

            items.Add(new MenuItem("continue", Show<GameScreen>) { selected = true });
            items.Add(new MenuItem("save game", saveGame));
            items.Add(new MenuItem("load game", loadGame) { active = false });
            items.Add(new MenuItem("options", Show<OptionsScreen>));
            items.Add(new MenuItem("end game", abortGame));
        }

        public void saveGame()
        {
            Console.WriteLine(entityManager.saveEntities());

            string filepath = "save.txt";   //todo think of some clever way to input this
            var fh = new SavefileHandler(filepath);
            fh.saveMap();// store current map
            fh.saveEntities(entityManager);

            //File.WriteAllText(filepath, entityManager.saveEntities());
            //using(StreamWriter file =
            //      new StreamWriter(filepath, true))
            //{
            //    file.WriteLine("Fourth line");
            //}
        }

        public void loadGame()
        {
            string filepath = "save.txt";   //todo think of some clever way to input this
            var fh = new SavefileHandler(filepath);
            fh.readFile();
            GameSingleton.Instance.entityManager.loadEntities(fh.getResults());
        }

        public void abortGame()
        {
            GameSingleton.Instance.startGame();
            Show<MainMenuScreen>();
        }
    }
}
