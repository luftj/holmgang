using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.Screens;

namespace holmgang.Desktop
{
    public class TradeScreen : Screen
    {
        SpriteBatch spriteBatch;
        KeyboardState prevKB;
        MouseState prevMS;

        int entriesWidth = 64;
        int entriesHeight = 64;
        int numEntriesX;
        int curSelection = 0;

        EntityManager entityManager;
        Entity player;
        Entity npc;
        List<ItemComponent> playerInventory;
        List<ItemComponent> npcInventory;

        List<int> playerItemsSelected;
        List<int> npcItemsSelected;

        bool firstframe = true;

        public TradeScreen()
        {
            playerInventory = new List<ItemComponent>();
            npcInventory = new List<ItemComponent>();

            playerItemsSelected = new List<int>();
            npcItemsSelected = new List<int>();
            //this.entityManager = entityManager;

        }

        public void init(EntityManager entityManager)
        {
            this.entityManager = entityManager;
            numEntriesX = (int)(GameSingleton.Instance.graphics.Viewport.Width * 1 / 3f) / entriesWidth;
        }

        public override void LoadContent()
        {
            //base.Initialize();
            base.LoadContent();

            spriteBatch = new SpriteBatch(GameSingleton.Instance.graphics);
        }

        public override void Update(GameTime gameTime)
        {
            if (!firstframe)
            {
                if (Keyboard.GetState().IsKeyDown(Keys.Escape) && prevKB.IsKeyUp(Keys.Escape))
                {
                    firstframe = true;
                    Show<GameScreen>();
                }
            }
            else
                firstframe = false;

            if (entityManager.GetEntities<PlayerControlComponent>().Count != 1)
                throw new NotSupportedException("Uh-oh.. shouldn't be more or less than one player component");
            player = entityManager.GetEntities<PlayerControlComponent>()[0];
            playerInventory = player.getAll<ItemComponent>();
            npc = entityManager.getClosest<AITraderComponent>(player);
            npcInventory = npc.getAll<ItemComponent>();

            #region navigateinventory
            if (Keyboard.GetState().IsKeyDown(Keys.Right) && prevKB.IsKeyUp(Keys.Right))
            {
                ++curSelection;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.Left) && prevKB.IsKeyUp(Keys.Left))
            {
                if (curSelection > 0)
                    --curSelection;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.Down) && prevKB.IsKeyUp(Keys.Down))
            {
                curSelection += numEntriesX; // row down
            }
            if (Keyboard.GetState().IsKeyDown(Keys.Up) && prevKB.IsKeyUp(Keys.Up))
            {
                if (curSelection > numEntriesX - 1)
                    curSelection -= numEntriesX; // row up
            }
            #endregion
            if (Keyboard.GetState().IsKeyDown(Keys.Enter) && prevKB.IsKeyUp(Keys.Enter))
            {
                // equip/unequip
                if (curSelection < playerInventory.Count)
                {
                    if (playerItemsSelected.Contains(curSelection))
                        playerItemsSelected.Remove(curSelection);
                    else
                        playerItemsSelected.Add(curSelection);
                }
            }

            // todo: make trade

            // todo: unequip, when selling equipped items

            //if (Keyboard.GetState().IsKeyDown(Keys.G) && prevKB.IsKeyUp(Keys.G))
            //{
            //    // dropping items
            //    if (curSelection < playerInventory.Count)
            //    {
            //        ItemComponent item = playerInventory[curSelection];

            //        player.get<WieldingComponent>().unequip(item);
            //        player.detach(item); // remove from player

            //        Entity droppedItem = new Entity();
            //        droppedItem.attach(new TransformComponent(player.get<TransformComponent>().position, player.get<TransformComponent>().orientation));
            //        droppedItem.attach(new SpriteComponent(item.name));
            //        droppedItem.attach(item);
            //        entityManager.attachEntity(droppedItem); // place in world
            //    }
            //}
            //if (Keyboard.GetState().IsKeyDown(Keys.E) && prevKB.IsKeyUp(Keys.E))
            //{
            //    // using items
            //    if (curSelection < playerInventory.Count)
            //        playerInventory[curSelection].use();
            //}


            prevKB = Keyboard.GetState();
            prevMS = Mouse.GetState();

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            GameSingleton.Instance.graphics.Clear(Color.Black);
            spriteBatch.Begin();
            //todo: draw grid?
            // draw player items
            for (int i = 0; i < playerInventory.Count; ++i)
            {
                // find position in grid
                int xpos = i % numEntriesX;
                int ypos = i / numEntriesX;

                Color col = Color.Lerp(Color.Red, Color.White, playerInventory[i].durability / (float)playerInventory[i].maxDurability); // color red when damaged
                spriteBatch.Draw(ContentSupplier.Instance.textures[playerInventory[i].name],
                                 new Rectangle(xpos * entriesWidth, 
                                               ypos * entriesHeight, 
                                               entriesWidth, entriesHeight),
                                 col);
                // print stack size
                if (playerInventory[i].stackable && playerInventory[i].amount > 1)
                {
                    string text = playerInventory[i].amount + "x";
                    Vector2 pos = new Vector2((xpos + 1) * entriesWidth, (ypos + 1) * entriesHeight)
                        - ContentSupplier.Instance.fonts["testfont"].MeasureString(text); // bottom right corner of cell
                    spriteBatch.DrawString(ContentSupplier.Instance.fonts["testfont"],
                                           text,
                                           pos,
                                           Color.White);
                }

                // draw selected marker
                if (playerItemsSelected.Contains(i))
                    spriteBatch.Draw(ContentSupplier.Instance.textures["hplow"],
                                 new Rectangle(xpos * entriesWidth,
                                                ypos * entriesHeight,
                                                entriesWidth, entriesHeight),
                                 Color.Green);

            }
            // draw npc items
            for (int i = 0; i < npcInventory.Count; ++i)
            {
                // find position in grid
                int xpos = i % numEntriesX + 2 * numEntriesX * entriesWidth;
                int ypos = i / numEntriesX;

                Color col = Color.Lerp(Color.Red, Color.White, npcInventory[i].durability / (float)npcInventory[i].maxDurability); // color red when damaged
                spriteBatch.Draw(ContentSupplier.Instance.textures[npcInventory[i].name],
                                 new Rectangle(xpos * entriesWidth,
                                               ypos * entriesHeight,
                                               entriesWidth, entriesHeight),
                                 col);
                // print stack size
                if (npcInventory[i].stackable && npcInventory[i].amount > 1)
                {
                    string text = npcInventory[i].amount + "x";
                    Vector2 pos = new Vector2((xpos + 1) * entriesWidth, (ypos + 1) * entriesHeight)
                        - ContentSupplier.Instance.fonts["testfont"].MeasureString(text); // bottom right corner of cell
                    spriteBatch.DrawString(ContentSupplier.Instance.fonts["testfont"],
                                           text,
                                           pos,
                                           Color.White);
                }

                // draw selected marker
                if (npcItemsSelected.Contains(i))
                    spriteBatch.Draw(ContentSupplier.Instance.textures["hplow"],
                                 new Rectangle(xpos * entriesWidth,
                                                ypos * entriesHeight,
                                                entriesWidth, entriesHeight),
                                 Color.Green);

            }

            // draw selector
            spriteBatch.Draw(ContentSupplier.Instance.textures["hplow"],
                             new Rectangle((curSelection % numEntriesX) * entriesWidth,
                                           (curSelection / numEntriesX) * entriesHeight,
                                           entriesWidth, entriesHeight),
                             Color.White);

            //todo: draw controls
            Vector2 drawpos = new Vector2(GameSingleton.Instance.graphics.Viewport.Width  / 3f, 0);
            string legendtext = "ARROWS = choose\nENTER = equip/unequip\ng = drop\ne = use\ni/ESC = back to game";
            spriteBatch.DrawString(ContentSupplier.Instance.fonts["testfont"], legendtext, drawpos, Color.White);
            spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}

