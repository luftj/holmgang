using System;
using System.Collections.Generic;
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

    public class CollisionSystem
    {
        //private Effect fillTexEffect;
        private RenderTarget2D objectMap;

        private Color curColour;
        private byte colourStep = 1;

        private Dictionary<Color, object> lookup;
        private Dictionary<Color, CollisionType> collisionMap;


        private TiledMap map;
        private TiledMapRenderer maprenderer;
        private Camera2D cam;

        SpriteBatch spriteBatch;

        public CollisionSystem(int width, int height, TiledMap map, TiledMapRenderer maprenderer, Camera2D cam)
        {
            lookup = new Dictionary<Color, object>();

            objectMap = new RenderTarget2D(maprenderer.GraphicsDevice, width, height);
            collisionMap = new Dictionary<Color, CollisionType>();
            collisionMap.Add(Color.Blue, CollisionType.WATER);
            collisionMap.Add(Color.Black, CollisionType.NONE);
            collisionMap.Add(new Color(0,0,0,0), CollisionType.NONE);

            this.map = map;
            this.maprenderer = maprenderer;
            this.cam = cam;
            spriteBatch = new SpriteBatch(maprenderer.GraphicsDevice);
        }

        /// <summary>
        /// Begin handling of selectable objects.
        /// </summary>
        public void Begin()
        {
            lookup.Clear();
            curColour = new Color(0, 0, 1); // set start color, can not be black

            maprenderer.GraphicsDevice.SetRenderTarget(objectMap);
            maprenderer.GraphicsDevice.Clear(Color.Black);

            //game.spriteBatch.Begin(SpriteSortMode.Immediate, null, null, null, null, fillTexEffect);
            spriteBatch.Begin(transformMatrix: cam.GetViewMatrix(), samplerState: SamplerState.PointClamp, sortMode: SpriteSortMode.Immediate);
            // todo allow sprites as well -> fillTexEffect
        }

        /// <summary>
        /// End and clean up for this frame
        /// </summary>
        public void End()
        {
            spriteBatch.End();

            // switch back to default render target
            maprenderer.GraphicsDevice.SetRenderTarget(null);
        }

        /// <summary>
        /// Adds an object, to handle clicks on it. Use inbetween Begin() and End() calls!
        /// </summary>
        /// <param name="worldObject">World object.</param>
        /// <param name="tex">Sprite of the object, which descirbes it's boundaries.</param>
        /// <param name="pos">Position, where to blit the sprite on the viewport</param>
        public void AddObject(object worldObject, Texture2D tex, Vector2 pos)
        {
            // call to begin
            throw new NotImplementedException("sprite picking not implemented");

            nextColour();   // use a new color key for this object

            // draw this sprite on the backbuffer
            spriteBatch.Draw(tex, pos, curColour);

            //add object to color-lookup-table
            lookup.Add(curColour, worldObject);

            // call to end
        }

        public void handleMap()
        {
            // call to begin

            maprenderer.Draw(map.GetLayer("collision"), cam.GetViewMatrix());

            // call to end
        }

        /// <summary>
        /// Gets the selection at given screen coordinates.
        /// </summary>
        /// <returns>The selection. Or NULL if there was no object found.</returns>
        /// <param name="x">The x coordinate.</param>
        /// <param name="y">The y coordinate.</param>
        public object getSelection(int x, int y)
        {
            throw new NotImplementedException("sprite picking not implemented");

            int i = x + y * objectMap.Width;
            Rectangle r = new Rectangle(x, y, 1, 1);
            Color[] dat = new Color[1];
            objectMap.GetData<Color>(0, r, dat, 0, 1);
            if(dat[0] != Color.Black)
                return lookup[dat[0]]; // todo: handle unknown colours -> trygetvalue or catch exception
            else return null;
        }

        /// <summary>
        /// Gets the collision key at specified map position .
        /// </summary>
        /// <returns>The collision key.</returns>
        /// <param name="x">The x coordinate in screen coordinates</param>
        /// <param name="y">The y coordinate in screen coordinates.</param>
        public CollisionType getCollisionKey(int x, int y)
        {
            int i = x + y * objectMap.Width;
            Rectangle r = new Rectangle(x, y, 1, 1);
            Color[] dat = new Color[1];
            objectMap.GetData<Color>(0, r, dat, 0, 1);

            try { 
                return collisionMap[dat[0]];} 
            catch(KeyNotFoundException) {
                return CollisionType.UNKNOWN; // debug: switch to NONE, maybe use TryGetValue
            }
        }

        public CollisionType getCollisionKey(Vector2 pos)
        {
            return getCollisionKey((int)pos.X, (int)pos.Y);
        }

        /// <summary>
        /// generates a new unique color
        /// </summary>
        private void nextColour()
        {
            if(curColour.R + colourStep < 255)
                curColour.R += colourStep;
            else if(curColour.G + colourStep < 255)
                curColour.G += colourStep;
            else if(curColour.B + colourStep < 255)
                curColour.B += colourStep;
            else
                throw new ArgumentOutOfRangeException("curColour", "Too many objects in scene for current colourkey settings");
        }
    }
}