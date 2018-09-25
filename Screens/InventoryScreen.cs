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
        List<EquipmentComponent> inventoryItems;
        EntityManager entityManager;

        bool firstframe = true;

        public InventoryScreen()
        {
            inventoryItems = new List<EquipmentComponent>();
            //this.entityManager = entityManager;

        }

        public void init(EntityManager entityManager)
        {
            this.entityManager = entityManager;
            numEntriesX = GameSingleton.Instance.graphics.Viewport.Width / entriesWidth;
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
            inventoryItems = player.getAll<EquipmentComponent>();

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
            if(Keyboard.GetState().IsKeyDown(Keys.Enter) && prevKB.IsKeyUp(Keys.Enter))
            {
                // equip/unequip
                if(curSelection < inventoryItems.Count)
                {   // don't equip both

                    // unequip primary
                    if(player.get<WieldingComponent>().primary == inventoryItems[curSelection])
                        player.get<WieldingComponent>().primary = null;
                    // unequip secondary
                    else if(player.get<WieldingComponent>().secondary == inventoryItems[curSelection])
                        player.get<WieldingComponent>().secondary = null;
                    // equip primary
                    else if(player.get<WieldingComponent>().primary == null)
                        player.get<WieldingComponent>().primary = inventoryItems[curSelection];
                    // equip secondary
                    else if(player.get<WieldingComponent>().secondary == null)
                        player.get<WieldingComponent>().secondary = inventoryItems[curSelection];
                }
            }
            if(Keyboard.GetState().IsKeyDown(Keys.G) && prevKB.IsKeyUp(Keys.G))
            {
                // dropping items
                if(curSelection < inventoryItems.Count)
                {
                    EquipmentComponent item = inventoryItems[curSelection];
                    // unequip
                    if(player.get<WieldingComponent>().primary == inventoryItems[curSelection])
                        player.get<WieldingComponent>().primary = null;
                    // unequip secondary
                    else if(player.get<WieldingComponent>().secondary == inventoryItems[curSelection])
                        player.get<WieldingComponent>().secondary = null;

                    player.detach(item); // remove from player
                    Entity droppedItem = EntityFactory.createItem(player.get<TransformComponent>().position,
                                                                  item.type,
                                                                  item.name,
                                                                  item.effect);
                    droppedItem.get<ItemComponent>().amount = item.amount; // might be a stack
                    entityManager.attachEntity(droppedItem); // place in world
                }
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

                spriteBatch.Draw(ContentSupplier.Instance.textures[inventoryItems[i].type],
                                 new Rectangle(xpos * entriesWidth, ypos * entriesHeight, entriesWidth, entriesHeight),
                                 Color.White);
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

            spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}
