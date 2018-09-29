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
            this.map = ContentSupplier.Instance.maps[GameSingleton.Instance.currentmap]; // get level from gamestate
        }
        
        public void update(GameTime gameTime)
        {
            this.cam = GameSingleton.Instance.entityManager.GetEntities<CameraComponent>()[0].get<CameraComponent>().camera;
            handleDamage();
            foreach(var projectile in entityManager.GetEntities<ProjectileComponent>()) // todo: put this somewhere else
            {
                var transform = projectile.get<TransformComponent>();
                transform.position += projectile.get<ProjectileComponent>().velocity;
            }
        }

        public void handleDamage()
        {
            foreach(Entity damagingEffect in entityManager.GetEntities<DamagingOnceComponent>())
            {   // damage handling
                var c = damagingEffect.get<DamagingOnceComponent>();

                foreach(Entity affectedEntity in entityManager.GetEntities<HealthComponent>())
                {
                    if(affectedEntity == damagingEffect || c.alreadyDamaged.Contains(affectedEntity))
                        continue;

                    // check for proximity
                    if(affectedEntity.get<TransformComponent>().distance(damagingEffect.get<TransformComponent>())> 30) // magic_number get from weapon reach? sprite collision?
                        continue;
                    c.alreadyDamaged.Add(affectedEntity); // don't damage again
                                                          // don't do damage when blocking
                    var shield = affectedEntity.get<WieldingComponent>()?.wielded(ItemType.BLOCK);
                    if(shield != null && (affectedEntity.get<CharacterComponent>()?.isBlocking ?? false))
                    {
                        shield.damage(c.damage);  // damage shield
                        continue;   // don't do hp damage
                    }

                    affectedEntity.get<HealthComponent>().doDamage(shield != null ? (c.damage - shield.effect) : c.damage);

                    // pull aggro
                    if(!affectedEntity.has<PlayerControlComponent>()) // todo: only for NPCs, could there be other entities with healthcomponent?
                    {
                        // follow and attack if not doing already
                        if(!affectedEntity.has<AIAttackComponent>())
                        {
                            affectedEntity.attach(new AIAttackComponent(c.alreadyDamaged[0])); // target owner of attack
                        }
                        if(!affectedEntity.has<AIFollowComponent>())
                        {
                            affectedEntity.attach(new AIFollowComponent(c.alreadyDamaged[0])); // target owner of attack
                        }
                    }
                }
            }
        }

        public Vector2 tryMove(Vector2 origin, Vector2 target)
        {
            if(origin == target)
                return target;
            var screentarget = cam.WorldToScreen(target);
            if(inScreen(target) && inScreen(origin))
            {
                // use collision key
                if(getCollisionKey(screentarget) == CollisionType.NONE)
                    return target;
                else
                {
                    // can't get there
                    Vector2 onlyX = target;
                    onlyX.Y = origin.Y;
                    if(getCollisionKey(cam.WorldToScreen(onlyX)) == CollisionType.NONE)
                        return onlyX;
                    Vector2 onlyY = target;
                    onlyY.X = origin.X;
                    if(getCollisionKey(cam.WorldToScreen(onlyY)) == CollisionType.NONE)
                        return onlyY;
                    Console.WriteLine(getCollisionKey(screentarget));
                    return origin;
                }
            } else
            {
                // use tile collision
                if(getPassable(target))
                    return target;
                else
                {
                    // can't get there
                    return origin;
                }
            }
        }

        bool inScreen(Vector2 point)
        {
            //Rectangle vp = GameSingleton.Instance.graphics.Viewport.Bounds;
            return cam.BoundingRectangle.Contains(point);
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
            if(GameSingleton.Instance.entityManager.GetEntities<CameraComponent>().Count > 0)  // camera not still in addlist
                cam = GameSingleton.Instance.entityManager.GetEntities<CameraComponent>()[0].get<CameraComponent>().camera;
            map = GameSingleton.Instance.map;
            var collisionlayer = map.GetLayer("collision");
            if(collisionlayer == null)
                return;
            maprenderer.Draw(collisionlayer, cam.GetViewMatrix());

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
            Vector2 isoPos = worldToIso(worldPos.X, worldPos.Y);
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

        private Vector2 worldToIso(float worldX, float worldY)
        {
            float tileW = map.TileWidth;
            float tileH = map.TileHeight;
            if(map.Orientation == TiledMapOrientation.Isometric)
                tileW = tileH;

            var isoX = worldY / tileH + worldX / (2 * tileW);
            var isoY = worldY / tileH - worldX / (2 * tileW);

            return new Vector2(isoX, isoY);
        }
#endregion
    }
}
