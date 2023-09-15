﻿using Microsoft.Xna.Framework;
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
        Map map;
        EntityManager entityManager;


       

        public MainGame()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            _graphics.PreferredBackBufferWidth = 800;  // set this value to the desired width of your window
            _graphics.PreferredBackBufferHeight = 800;
            entityManager = new EntityManager();
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
            building.Position = new Vector2(10,2) + map.MapOffset;
            building.Id = "debugBuilding2x2";
            belt = new Belt(_spriteBatch);
            belt.Position = new Vector2(10, 4) + map.MapOffset;
            belt.Id = "debugBelt";
            belt.ProcessingTime = 1500;
            belt.AddItemToInput(new Items.Item() { Id = "debugItem" }, ItemLogisticsComponentPort.PortDirection.N, belt.Position);
            entityManager.Add(building);
            entityManager.Add(belt);
            Vector2 lastPos = belt.Position;
            int i = 1;
            for(i= 1; i < 5; ++i)
            {
                Belt dynBelt = new Belt(_spriteBatch);
                dynBelt.Position = new Vector2(lastPos.X, lastPos.Y + i);
                dynBelt.Id = belt.Id;
                dynBelt.ProcessingTime = belt.ProcessingTime;
                if (i == 4)
                {
                    dynBelt.Rotate(DrawableEntity.EntityRotation.Rot90);
                    dynBelt.AddItemToInput(new Items.Item() { Id = "debugItem" }, ItemLogisticsComponentPort.PortDirection.W, belt.Position);
                }
                entityManager.Add(dynBelt);
            }
            lastPos=new Vector2(lastPos.X,lastPos.Y + i-1);
            for(i= 1;i < 4; ++i)
            {
                Belt dynBelt = new Belt(_spriteBatch);
                dynBelt.Position = new Vector2(lastPos.X - i, lastPos.Y);
                dynBelt.Id = belt.Id;
                dynBelt.ProcessingTime = belt.ProcessingTime;
                dynBelt.Rotate(DrawableEntity.EntityRotation.Rot90);
                entityManager.Add(dynBelt);
            }
            lastPos = new Vector2(lastPos.X - i , lastPos.Y+1);
            for (i = 1; i < 5; ++i)
            {
                Belt dynBelt = new Belt(_spriteBatch);
                dynBelt.Position = new Vector2(lastPos.X, lastPos.Y - i);
                dynBelt.Id = belt.Id;
                dynBelt.ProcessingTime = belt.ProcessingTime;
                dynBelt.Rotate(DrawableEntity.EntityRotation.Rot180);
                entityManager.Add(dynBelt);
            }

            lastPos = new Vector2(lastPos.X - 1, lastPos.Y - i);
            for (i = 1; i < 5; ++i)
            {
                Belt dynBelt = new Belt(_spriteBatch);
                dynBelt.Position = new Vector2(lastPos.X + i, lastPos.Y);
                dynBelt.Id = belt.Id;
                dynBelt.ProcessingTime = belt.ProcessingTime;
                dynBelt.Rotate(DrawableEntity.EntityRotation.Rot270);
                entityManager.Add(dynBelt);
            }
            // TODO: use this.Content to load your game content here
        }
        DrawableEntity.EntityRotation entityRotation = DrawableEntity.EntityRotation.None;
        void Rotate()
        {
            int length = Enum.GetValues(typeof(DrawableEntity.EntityRotation)).Length;
            entityRotation = (DrawableEntity.EntityRotation)(((uint)(entityRotation) + 1)%length);
        }
        GameTime lastAction = null;
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();
            currentMousePos = Mouse.GetState().Position;
            Vector2 mousePos = Map.ScreenToMap(currentMousePos.X, currentMousePos.Y);
            /*if(lastAction == null)
            {
                lastAction = new GameTime(gameTime.TotalGameTime,gameTime.ElapsedGameTime, gameTime.IsRunningSlowly);
            }
            else
            {
                if(gameTime.TotalGameTime.TotalSeconds - lastAction.TotalGameTime.TotalSeconds > 5)
                {
                    Rotate();
                    belt.Rotate(entityRotation);
                    if (belt.CanAcceptItemsFrom(ItemLogisticsComponentPort.PortDirection.N, belt.Position))
                    {
                        belt.AddItemToInput(new Items.Item() { Id = "debugItem" }, ItemLogisticsComponentPort.PortDirection.N, belt.Position);
                    }
                    lastAction = new GameTime(gameTime.TotalGameTime, gameTime.ElapsedGameTime, gameTime.IsRunningSlowly);
                }
            }*/
            entityManager.Update(gameTime);
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black) ;
            // TODO: Add your drawing code here
            _spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend);
            map.Draw(gameTime);
            entityManager.Draw(gameTime);
            Vector2 mousePos = Map.ScreenToMap(currentMousePos.X, currentMousePos.Y);
            //DrawChunk(ScreenToMap(_graphics.PreferredBackBufferWidth/2,_graphics.PreferredBackBufferHeight/2));
            _spriteBatch.Draw(gridTexture, Map.MapToScreen((int)mousePos.X, (int)mousePos.Y), Color.White);
            Entity entity = entityManager.GetFromPosition(mousePos - map.MapOffset);
            string text = $"Coords:({mousePos.X - map.MapOffset.X}, {mousePos.Y - map.MapOffset.Y})";
            if (entity != null) 
            {
                text = $"{text} Entity: #{entity.Id}";
            }
           _spriteBatch.DrawString(debugFont,text, new Vector2(0,0),Color.White);
            _spriteBatch.End();
            base.Draw(gameTime);

        }
    }
}