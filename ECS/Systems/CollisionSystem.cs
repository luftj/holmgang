using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.Tiled;
using MonoGame.Extended.Tiled.Graphics;

namespace holmgang.Desktop
{
    public enum CollisionType
    {
        WATER,
        NONE,
        UNKNOWN,    // debug
    }

    public class CollisionSystem : System
    {

        private RenderTarget2D objectMap;
        private Dictionary<Color, CollisionType> collisionMap;
        private SpriteBatch spriteBatch;


        private TiledMap map;
        private TiledMapRenderer maprenderer;
        private Camera2D cam;

        public CollisionSystem(EntityManager entityManager) : base(entityManager)
        {

            this.maprenderer = new TiledMapRenderer(GameSingleton.Instance.graphics);


            objectMap = new RenderTarget2D(maprenderer.GraphicsDevice,
                                           maprenderer.GraphicsDevice.Viewport.Width,
                                           maprenderer.GraphicsDevice.Viewport.Height);
            collisionMap = new Dictionary<Color, CollisionType>();
            collisionMap.Add(Color.Blue, CollisionType.WATER);
            collisionMap.Add(Color.Black, CollisionType.NONE);
            collisionMap.Add(new Color(0, 255, 0, 255), CollisionType.NONE);
            collisionMap.Add(new Color(0, 0, 0, 0), CollisionType.NONE);

            spriteBatch = new SpriteBatch(maprenderer.GraphicsDevice);
        }

        public void LoadContent()
        {
            this.map = ContentSupplier.Instance.maps["map"]; // todo: get level from gamestate -> gamesingleton
        }
        
        public void update(GameTime gameTime)
        {
            this.cam = GameSingleton.Instance.entityManager.GetEntities<CameraComponent>()[0].get<CameraComponent>().camera;
            foreach(Entity e in entityManager.GetEntities<DamagingOnceComponent>())
            {
                var c = e.get<DamagingOnceComponent>();

                foreach(Entity other in entityManager.entities)
                {
                    if(other == e || !other.has<HealthComponent>())
                        continue;
                    if(c.alreadyDamaged.Contains(other))
                        continue;
                    c.alreadyDamaged.Add(other); // don't damage again

                    // check for proximity
                    if((other.get<TransformComponent>().position - e.get<TransformComponent>().position).Length() < 30) // todo magic number
                    {
                        if(other.get<PlayerControlComponent>()?.isBlocking ?? false) // don't do damage when blocking
                        {
                            if(other.get<WieldingComponent>().secondary == null)
                                continue;
                            other.get<WieldingComponent>().secondary.damage(c.damage);
                            if(other.get<WieldingComponent>().secondary.durability <= 0)
                            {
                                other.get<WieldingComponent>().unequip(other.get<WieldingComponent>().secondary);
                                other.detach(other.getAll<SpriteComponent>().Find(x => x.spriteName == "shield"));
                                other.get<PlayerControlComponent>().isBlocking = false;
                            }
                            continue;
                        } else if(other.get<CharacterComponent>()?.isBlocking ?? false)
                        {
                            other.get<WieldingComponent>().secondary.durability -= c.damage;
                            continue;
                        }

                        var shield = other.get<WieldingComponent>()?.wielding("BLOCK"); // item reduces damage
                        other.get<HealthComponent>().doDamage(shield != null ? (c.damage - shield.effect) : c.damage);

                        if(other.get<HealthComponent>().HP <= 0)
                        {
                            other.get<HealthComponent>().regPerS = 0;
                            var camc = other.get<CameraComponent>();
                            if(camc != null)
                                entityManager.attachEntity(EntityFactory.createCamera(camc.camera));
                            entityManager.destroyEntity(other); // u ded :(
                        }
                        
                        // pull aggro
                        if(!other.has<PlayerControlComponent>())
                        {
                            // follow and attack if not doing already
                            if(!other.has<AIAttackComponent>())
                            {
                                other.attach(new AIAttackComponent(c.alreadyDamaged[0])); // target owner of attack
                            }
                            if(!other.has<AIFollowComponent>())
                            {
                                other.attach(new AIFollowComponent(c.alreadyDamaged[0])); // target owner of attack
                            }
                        }
                    }
                }
            }
        }

        #region mapcollision
        /// <summary>
        /// Begin drawing of backbuffer and handling of selectable objects.
        /// </summary>
        public void Begin()
        {
            maprenderer.GraphicsDevice.SetRenderTarget(objectMap);
            maprenderer.GraphicsDevice.Clear(Color.Black);

            spriteBatch.Begin(transformMatrix: cam.GetViewMatrix(), samplerState: SamplerState.PointClamp, sortMode: SpriteSortMode.Immediate);
        }

        /// <summary>
        /// End drawing to backbuffer and clean up for this frame
        /// </summary>
        public void End()
        {
            spriteBatch.End();

            // switch back to default render target
            maprenderer.GraphicsDevice.SetRenderTarget(null);
        }

        public void handleMap()
        {
            // call to begin
            var c = GameSingleton.Instance.entityManager.GetEntities<CameraComponent>()[0].get<CameraComponent>().camera;
            var collisionlayer = map.GetLayer("collision");
            if(collisionlayer == null)
                return;
            maprenderer.Draw(collisionlayer, c.GetViewMatrix());

            //foreach(var i in map.TileLayers)
            //{
            //    if(i.Name=="collision")
            //    foreach(var it in i.Tiles)
            //        Console.WriteLine(it.ToString());
            //}

            // call to end
        }

        /// <summary>
        /// Gets the collision key at specified map position .
        /// </summary>
        /// <returns>The collision key.</returns>
        /// <param name="x">The x coordinate in screen coordinates</param>
        /// <param name="y">The y coordinate in screen coordinates.</param>
        public CollisionType getCollisionKey(int x, int y)
        {
            //todo this allows collisin only in visible area
            int i = x + y * objectMap.Width;
            Rectangle r = new Rectangle(x, y, 1, 1);
            Color[] dat = new Color[1];
            objectMap.GetData<Color>(0, r, dat, 0, 1);

            try
            {
                return collisionMap[dat[0]];
            } catch(KeyNotFoundException)
            {
                return CollisionType.UNKNOWN; // debug: switch to NONE, maybe use TryGetValue
            }
        }

        public CollisionType getCollisionKey(Vector2 pos)
        {
            return getCollisionKey((int)pos.X, (int)pos.Y);
        }
        #endregion

        #region tilecollision
        public bool getPassable(Vector2 worldPos)
        {
            Vector2 isoPos = screenToIso(worldPos.X, worldPos.Y);
            //Console.WriteLine("map " + isoPos.ToPoint().ToString());

            TiledMapTileLayer layer = map.GetLayer<TiledMapTileLayer>("collision");
            if(layer == null)
                return true;
            TiledMapTile? tile;
            layer.TryGetTile((int)isoPos.X, (int)isoPos.Y, out tile);
            if(tile == null)
                return false;
            //Console.WriteLine("id " + tile.Value.GlobalIdentifier);
            int[] ids = { 0, 73, 78, 80, 95 };  // todo: oh hell no
            return ids.Contains(tile.Value.GlobalIdentifier); 
        }

        private Vector2 screenToIso(float screenX, float screenY)
        {
            float tileW = map.TileWidth;
            float tileH = map.TileHeight;
            if(map.Orientation == TiledMapOrientation.Isometric)
                tileW = tileH;

            var isoX = screenY / tileH + screenX / (2 * tileW);
            var isoY = screenY / tileH - screenX / (2 * tileW);

            return new Vector2(isoX, isoY);
        }
#endregion
    }
}
