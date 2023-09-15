using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using PixelFactory.Buildings;
using PixelFactory.Logistics;
using System;
using System.Diagnostics;
using System.Diagnostics.Tracing;

namespace PixelFactory
{
    public class MainGame : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private SpriteFont debugFont;
        private Texture2D gridTexture;
        Building building;
        Belt belt;
        private Texture2D groundTexture;
        Point currentMousePos;
        Vector2 selectedTile;
        Map map;


       

        public MainGame()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            _graphics.PreferredBackBufferWidth = 800;  // set this value to the desired width of your window
            _graphics.PreferredBackBufferHeight = 800;
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            
            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            gridTexture = Content.Load<Texture2D>("gridSelector");
            map = new Map(_spriteBatch);
          //  map.MapOffset = Map.ScreenToMap(_graphics.PreferredBackBufferWidth/2, _graphics.PreferredBackBufferHeight/4);
            ContentManager.Instance.AddTileTexture("debug", Content.Load<Texture2D>("debugGrid"));
            ContentManager.Instance.AddItemTexture("debugItem", Content.Load<Texture2D>("debugItem"));
            ContentManager.Instance.AddBuildingTexture("debugBelt",Content.Load<Texture2D>("debugBelt"));
            ContentManager.Instance.AddBuildingTexture("debugBuilding2x2", Content.Load<Texture2D>("debugBuilding2x2"));
            debugFont = Content.Load<SpriteFont>("debug");
            map.Generate();
            building = new Building(_spriteBatch, new Vector2(2, 2));
            building.Position = new Vector2(10,5) + map.MapOffset;
            building.Id = "debugBuilding2x2";
            belt = new Belt(_spriteBatch);
            belt.Position = new Vector2(10, 7) + map.MapOffset;
            belt.Id = "debugBelt";
            belt.ProcessingTime = 15000;

            // TODO: use this.Content to load your game content here
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();
            if(Mouse.GetState().LeftButton == ButtonState.Pressed)
            {
                selectedTile = Map.ScreenToMap(Mouse.GetState().Position.X, Mouse.GetState().Position.Y);
            }
            currentMousePos = Mouse.GetState().Position;
            Vector2 mousePos = Map.ScreenToMap(currentMousePos.X, currentMousePos.Y);
            // TODO: Add your update logic here
            belt.Update(gameTime);
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black) ;
            // TODO: Add your drawing code here
            _spriteBatch.Begin();
            map.Draw(gameTime);
            building.Draw(gameTime);
            belt.Draw(gameTime);
            Vector2 mousePos = Map.ScreenToMap(currentMousePos.X, currentMousePos.Y);
            //DrawChunk(ScreenToMap(_graphics.PreferredBackBufferWidth/2,_graphics.PreferredBackBufferHeight/2));
            _spriteBatch.Draw(gridTexture, Map.MapToScreen((int)mousePos.X, (int)mousePos.Y), Color.White);
           _spriteBatch.DrawString(debugFont,$"Coords:({mousePos.X-map.MapOffset.X}, {mousePos.Y-map.MapOffset.Y})", new Vector2(0,0),Color.White);
            _spriteBatch.End();
            base.Draw(gameTime);

        }
    }
}