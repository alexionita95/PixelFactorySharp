using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using PixelFactory.Animations;
using PixelFactory.Buildings;
using PixelFactory.Crafting;
using PixelFactory.Entities;
using PixelFactory.Inventory;
using PixelFactory.Logistics;
using PixelFactory.Logistics.Fluids;
using PixelFactory.Logistics.Items;
using PixelFactory.Serialization;
using PixelFactory.UI;
using System;
using System.Diagnostics;
using System.Diagnostics.Tracing;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Nodes;
using static System.Net.Mime.MediaTypeNames;

namespace PixelFactory
{
    public class MainGame : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private SpriteFont debugFont;
        Map map;
        Camera camera;
        ContentManager contentManager;

        Player player = new Player();



        public MainGame()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            _graphics.PreferredBackBufferWidth = 800;  // set this value to the desired width of your window
            _graphics.PreferredBackBufferHeight = 800;
            camera = new Camera(Vector2.Zero, new Vector2(800, 800));
            camera.Zoom = 1;

            contentManager = new ContentManager();

        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            contentManager.AddTileTexture("debug", Content.Load<Texture2D>("debugGrid"));
            debugFont = Content.Load<SpriteFont>("debug");
           /* map = new Map(contentManager.TileTextures);
            map.Camera = camera;
            map.Generate();
            var save = map.Serialize().ToArray();
             System.IO.File.WriteAllBytes("map.dat", save);*/
            var data = System.IO.File.ReadAllBytes("map.dat").ToList();
            string type = Serializer.ReadString(data);
            Trace.WriteLine(type);
            Type objType = Type.GetType(type);
            if(objType != null ) 
            {
                map = Activator.CreateInstance(objType) as Map;
                map.TileTextures = contentManager.TileTextures;
                map.Camera = camera;
                map.Deserialize(data, contentManager);
            }

        }
        protected override void Update(GameTime gameTime)
        {
            if (IsActive)
            {
                //Handle Input
            }
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);
            _spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend, transformMatrix: camera.TransformMatrix);
            map.Draw(gameTime, _spriteBatch);
            string text = $" FPS:{Math.Ceiling(1 / gameTime.ElapsedGameTime.TotalSeconds)}";
            _spriteBatch.DrawString(debugFont, text, camera.ScreenToWorld(new Vector2(0, 0)), Color.White);
            _spriteBatch.End();
            base.Draw(gameTime);

        }
    }
}