using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.Screens;

namespace holmgang.Desktop
{
    public class InventoryScreen : Screen
    {
        SpriteBatch spriteBatch;
        KeyboardState prevKB;
        MouseState prevMS;
        
        int entriesWidth = 64;
        int entriesHeight = 64;
        int numEntriesX;
        int curSelection = 0;

        Entity player;
        List<ItemComponent> inventoryItems;
        EntityManager entityManager;

        bool firstframe = true;

        public InventoryScreen()
        {
            inventoryItems = new List<ItemComponent>();
            //this.entityManager = entityManager;

        }

        public void init(EntityManager entityManager)
        {
            this.entityManager = entityManager;
            numEntriesX = (int)(GameSingleton.Instance.graphics.Viewport.Width * 2/3f) / entriesWidth;
        }

        public override void LoadContent()
        {
            //base.Initialize();
            base.LoadContent();

            spriteBatch = new SpriteBatch(GameSingleton.Instance.graphics);
        }
        
        public override void Update(GameTime gameTime)
        {
            if(!firstframe)
            {
                if(Keyboard.GetState().IsKeyDown(Keys.Escape) && prevKB.IsKeyUp(Keys.Escape) ||
                   Keyboard.GetState().IsKeyDown(Keys.I) && prevKB.IsKeyUp(Keys.I))
                {
                    firstframe = true;
                    Show<GameScreen>();
                }
            } else
                firstframe = false;

            if(entityManager.GetEntities<PlayerControlComponent>().Count != 1)
                throw new NotSupportedException("Uh-oh.. shouldn't be more or less than one player component");
            player = entityManager.GetEntities<PlayerControlComponent>()[0];
            inventoryItems = player.getAll<ItemComponent>();

            #region navigateinventory
            if(Keyboard.GetState().IsKeyDown(Keys.Right) && prevKB.IsKeyUp(Keys.Right))
            {
                ++curSelection;
            }
            if(Keyboard.GetState().IsKeyDown(Keys.Left) && prevKB.IsKeyUp(Keys.Left))
            {
                if(curSelection > 0)
                    --curSelection;
            }
            if(Keyboard.GetState().IsKeyDown(Keys.Down) && prevKB.IsKeyUp(Keys.Down))
            {
                curSelection += numEntriesX; // row down
            }
            if(Keyboard.GetState().IsKeyDown(Keys.Up) && prevKB.IsKeyUp(Keys.Up))
            {
                if(curSelection > numEntriesX -1)
                    curSelection -= numEntriesX; // row up
            }
#endregion
            if(Keyboard.GetState().IsKeyDown(Keys.Enter) && prevKB.IsKeyUp(Keys.Enter))
            {
                // equip/unequip
                if(curSelection < inventoryItems.Count)
                {
                    player.get<WieldingComponent>().equip(inventoryItems[curSelection]);
                }
            }
            if(Keyboard.GetState().IsKeyDown(Keys.G) && prevKB.IsKeyUp(Keys.G))
            {
                // dropping items
                if(curSelection < inventoryItems.Count)
                {
                    ItemComponent item = inventoryItems[curSelection];

                    player.get<WieldingComponent>().unequip(item);
                    player.detach(item); // remove from player

                    Entity droppedItem = new Entity();
                    droppedItem.attach(new TransformComponent(player.get<TransformComponent>().position, player.get<TransformComponent>().orientation));
                    droppedItem.attach(new SpriteComponent(item.name));
                    droppedItem.attach(item);
                    entityManager.attachEntity(droppedItem); // place in world
                }
            }
            if(Keyboard.GetState().IsKeyDown(Keys.E) && prevKB.IsKeyUp(Keys.E))
            {
                // using items
                if(curSelection < inventoryItems.Count)
                    inventoryItems[curSelection].use();
            }


            prevKB = Keyboard.GetState();
            prevMS = Mouse.GetState();

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            GameSingleton.Instance.graphics.Clear(Color.Black);
            spriteBatch.Begin();
            //todo: draw grid?
            // draw items
            for(int i = 0; i < inventoryItems.Count;++i)
            {
                // find position in grid
                int xpos = i % numEntriesX;
                int ypos = i / numEntriesX;

                Color col = Color.Lerp(Color.Red, Color.White, inventoryItems[i].durability / (float)inventoryItems[i].maxDurability); // color red when damaged
                spriteBatch.Draw(ContentSupplier.Instance.textures[inventoryItems[i].name],
                                 new Rectangle(xpos * entriesWidth, ypos * entriesHeight, entriesWidth, entriesHeight),
                                 col);
                // print stack size
                if(inventoryItems[i].stackable && inventoryItems[i].amount > 1)
                {
                    string text = inventoryItems[i].amount + "x";
                    Vector2 pos = new Vector2((xpos+1) * entriesWidth, (ypos+1) * entriesHeight) 
                        - ContentSupplier.Instance.fonts["testfont"].MeasureString(text); // bottom right corner of cell
                    spriteBatch.DrawString(ContentSupplier.Instance.fonts["testfont"], 
                                           text, 
                                           pos, 
                                           Color.White);
                }

            }
            // draw selector
            spriteBatch.Draw(ContentSupplier.Instance.textures["hplow"],
                             new Rectangle((curSelection % numEntriesX) * entriesWidth,
                                           (curSelection / numEntriesX) * entriesHeight,
                                           entriesWidth, entriesHeight),
                             Color.White);


            // draw equipped marker
            var wieldedPrimary = inventoryItems.FindIndex(x => x == player.get<WieldingComponent>().primary);
            if(wieldedPrimary != -1)
                spriteBatch.Draw(ContentSupplier.Instance.textures["hplow"],
                                 new Rectangle((wieldedPrimary % numEntriesX) * entriesWidth,
                                               (wieldedPrimary / numEntriesX) * entriesHeight,
                                                entriesWidth, entriesHeight),
                                 Color.Green);
            var wieldedSecondary = inventoryItems.FindIndex(x => x == player.get<WieldingComponent>().secondary);
            if(wieldedSecondary != -1)
                spriteBatch.Draw(ContentSupplier.Instance.textures["hplow"],
                                 new Rectangle((wieldedSecondary % numEntriesX) * entriesWidth,
                                               (wieldedSecondary / numEntriesX) * entriesHeight,
                                                entriesWidth, entriesHeight),
                                 Color.Red);

            //todo: draw controls
            Vector2 drawpos = new Vector2(GameSingleton.Instance.graphics.Viewport.Width * 2 / 3f, 0);
            string legendtext = "ARROWS = choose\nENTER = equip/unequip\ng = drop\ne = use\ni/ESC = back to game";
            spriteBatch.DrawString(ContentSupplier.Instance.fonts["testfont"], legendtext, drawpos, Color.White);
            spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}
