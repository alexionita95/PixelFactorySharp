using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using PixelFactory.Animations;
using PixelFactory.Buildings;
using PixelFactory.Entities;
using PixelFactory.Items;
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
        Point currentMousePos;
        Map map;
        EntityManager entityManager;
        Camera camera;
        ContentManager contentManager;
        AnimationManager animationManager;

        Player player = new Player();



        public MainGame()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            _graphics.PreferredBackBufferWidth = 800;  // set this value to the desired width of your window
            _graphics.PreferredBackBufferHeight = 800;
            camera = new Camera(Vector2.Zero, new Vector2(800, 800));
            entityManager = new EntityManager();
            entityManager.Camera = camera;
            camera.Zoom = 1;

            contentManager = new ContentManager();
            animationManager = new AnimationManager();

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
            // map.MapOffset = Map.ScreenToMap(_graphics.PreferredBackBufferWidth/2, _graphics.PreferredBackBufferHeight/4);
            contentManager.AddTileTexture("debug", Content.Load<Texture2D>("debugGrid"));
            contentManager.AddTexture("debugItem", Content.Load<Texture2D>("debugItem"));
            contentManager.AddTexture("debugBelt", Content.Load<Texture2D>("debugBelt"));
            contentManager.AddTexture("debugBuilding2x2", Content.Load<Texture2D>("debugBuilding2x2"));
            debugFont = Content.Load<SpriteFont>("debug");
            map = new Map(_spriteBatch, contentManager.TileTextures);
            map.Camera = camera;
            map.Generate();
            building = new Building(_spriteBatch, new Vector2(2, 2));
            building.Position = new Vector2(10, 2) + map.MapOffset;
            building.Id = "debugBuilding2x2";
            building.Texture = contentManager.GetTexture(building.Id);
            belt = new Belt(_spriteBatch);
            belt.Position = new Vector2(10, 4) + map.MapOffset;
            belt.Id = "debugBelt";
            belt.Texture = contentManager.GetTexture(belt.Id);
            belt.ProcessingTime = 1000f;
            belt.AddItemToInput(new Items.Item(_spriteBatch, Vector2.One) { Id = "debugItem", Texture = contentManager.GetTexture("debugItem") }, Direction.N, belt.Position);

            Animation animation = new Animation(belt.Texture, new Vector2(Map.TileSize, Map.TileSize), 250, true);
            animationManager.AddAnimation(animation);
            belt.Animation = animation;
            entityManager.Add(building);
            entityManager.Add(belt);

            entityManager.Add(player);
            Item debugItem1 = new Item(_spriteBatch, Vector2.One) { Id = "debugItem_1", Texture = contentManager.GetTexture("debugItem") };
            Item debugItem2 = new Item(_spriteBatch, Vector2.One) { Id = "debugItem_2", Texture = contentManager.GetTexture("debugItem") };
            Item debugItem3 = new Item(_spriteBatch, Vector2.One) { Id = "debugItem_3", Texture = contentManager.GetTexture("debugItem") };
            Recipe recipe = new Recipe();
            recipe.Duration = 15000;
            recipe.AddInput(new RecipeItem(debugItem1, 2));
            recipe.AddInput(new RecipeItem(debugItem2, 2));
            recipe.AddOutput(new RecipeItem(debugItem3, 1));
            player.AddItemsToInventory(debugItem1, 12);
            player.AddItemsToInventory(debugItem2, 6);
            player.DisplayDebugInventory();
            player.Craft(recipe, 7);

            Vector2 lastPos = belt.Position;
            int i = 1;
            for (i = 1; i < 5; ++i)
            {
                Belt dynBelt = new Belt(_spriteBatch);
                dynBelt.Position = new Vector2(lastPos.X, lastPos.Y + i);
                dynBelt.Id = belt.Id;
                dynBelt.ProcessingTime = belt.ProcessingTime;
                dynBelt.Texture = belt.Texture;
                dynBelt.Animation = belt.Animation;
                if (i == 4)
                {
                    dynBelt.Rotation = DrawableEntity.EntityRotation.Rot90;
                    dynBelt.AddItemToInput(new Items.Item(_spriteBatch, Vector2.One) { Id = "debugItem", Texture = contentManager.GetTexture("debugItem") }, Direction.W, belt.Position);
                }
                entityManager.Add(dynBelt);
            }
            lastPos = new Vector2(lastPos.X, lastPos.Y + i - 1);
            for (i = 1; i < 4; ++i)
            {
                Belt dynBelt = new Belt(_spriteBatch);
                dynBelt.Position = new Vector2(lastPos.X - i, lastPos.Y);
                dynBelt.Id = belt.Id;
                dynBelt.ProcessingTime = belt.ProcessingTime;
                dynBelt.Rotation = DrawableEntity.EntityRotation.Rot90;
                dynBelt.Texture = belt.Texture;
                dynBelt.Animation = belt.Animation;
                entityManager.Add(dynBelt);
            }
            lastPos = new Vector2(lastPos.X - i, lastPos.Y + 1);
            for (i = 1; i < 5; ++i)
            {
                Belt dynBelt = new Belt(_spriteBatch);
                dynBelt.Position = new Vector2(lastPos.X, lastPos.Y - i);
                dynBelt.Id = belt.Id;
                dynBelt.ProcessingTime = belt.ProcessingTime;
                dynBelt.Rotation = DrawableEntity.EntityRotation.Rot180;
                dynBelt.Texture = belt.Texture;
                dynBelt.Animation = belt.Animation;
                entityManager.Add(dynBelt);
            }

            lastPos = new Vector2(lastPos.X - 1, lastPos.Y - i);
            for (i = 1; i < 5; ++i)
            {
                Belt dynBelt = new Belt(_spriteBatch);
                dynBelt.Position = new Vector2(lastPos.X + i, lastPos.Y);
                dynBelt.Id = belt.Id;
                dynBelt.ProcessingTime = belt.ProcessingTime;
                dynBelt.Rotation = DrawableEntity.EntityRotation.Rot270;
                dynBelt.Texture = belt.Texture;
                dynBelt.Animation = belt.Animation;
                entityManager.Add(dynBelt);
            }
            // TODO: use this.Content to load your game content here
        }
        DrawableEntity.EntityRotation entityRotation = DrawableEntity.EntityRotation.None;
        void Rotate()
        {
            int length = Enum.GetValues(typeof(DrawableEntity.EntityRotation)).Length;
            entityRotation = (DrawableEntity.EntityRotation)(((uint)(entityRotation) + 1) % length);
        }
        GameTime lastAction = null;

        int previousScrollWheelValue = 0;

        protected override void Update(GameTime gameTime)
        {
            if (IsActive)
            {
                if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                    Exit();


                if (Keyboard.GetState().IsKeyDown(Keys.Right))
                {
                    camera.Position = new Vector2(camera.Position.X + 1, camera.Position.Y);
                }


                if (Keyboard.GetState().IsKeyDown(Keys.Left))
                {
                    camera.Position = new Vector2(camera.Position.X - 1, camera.Position.Y);
                }
                if (Keyboard.GetState().IsKeyDown(Keys.Right))
                {
                    camera.Position = new Vector2(camera.Position.X + 1, camera.Position.Y);
                }


                if (Keyboard.GetState().IsKeyDown(Keys.Up))
                {
                    camera.Position = new Vector2(camera.Position.X, camera.Position.Y - 1);
                }
                if (Keyboard.GetState().IsKeyDown(Keys.Down))
                {
                    camera.Position = new Vector2(camera.Position.X, camera.Position.Y + 1);
                }
                float scrollDelta = Mouse.GetState().ScrollWheelValue - previousScrollWheelValue;

                if (scrollDelta > 0)
                {
                    camera.Zoom += 0.1f;
                }
                else if (scrollDelta < 0)
                {
                    camera.Zoom -= 0.1f;
                }

                previousScrollWheelValue = Mouse.GetState().ScrollWheelValue;
                if (camera.IsInviewport(Mouse.GetState().Position.ToVector2()))
                {
                    currentMousePos = Mouse.GetState().Position;

                    Point worldPos = camera.ScreenToWorld(currentMousePos.ToVector2()).ToPoint();
                    Vector2 mousePos = Map.ScreenToMap(worldPos.X, worldPos.Y, camera.Zoom);
                    if (Mouse.GetState().LeftButton == ButtonState.Pressed)
                    {
                        Vector2 pos = mousePos;

                        Entity entity = entityManager.GetFromPosition(pos);
                        if (entity == null)
                        {
                            Belt dynBelt = new Belt(_spriteBatch);
                            dynBelt.Position = pos;
                            dynBelt.Id = belt.Id;
                            dynBelt.ProcessingTime = belt.ProcessingTime;
                            dynBelt.Rotation = DrawableEntity.EntityRotation.Rot180;
                            dynBelt.Texture = belt.Texture;
                            dynBelt.Animation = belt.Animation;
                            entityManager.Add(dynBelt);
                        }
                    }
                }
            }
            if (lastAction == null)
            {
                lastAction = new GameTime(gameTime.TotalGameTime, gameTime.ElapsedGameTime, gameTime.IsRunningSlowly);
            }
            else
            {
                if (gameTime.TotalGameTime.TotalSeconds - lastAction.TotalGameTime.TotalSeconds > 2)
                {

                    if (belt.CanAcceptItemsFrom(Direction.N, belt.Position))
                    {
                        belt.AddItemToInput(new Items.Item(_spriteBatch, Vector2.One) { Id = "debugItem", Texture = contentManager.GetTexture("debugItem") }, Direction.N, belt.Position);
                    }
                    lastAction = new GameTime(gameTime.TotalGameTime, gameTime.ElapsedGameTime, gameTime.IsRunningSlowly);
                }
            }
            animationManager.Update(gameTime);
            entityManager.Update(gameTime);
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);
            // TODO: Add your drawing code here
            _spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend, transformMatrix: camera.TransformMatrix);
            map.Draw(gameTime);
            entityManager.Draw(gameTime);
            Point worldPos = camera.ScreenToWorld(currentMousePos.ToVector2()).ToPoint();
            Vector2 mousePos = Map.ScreenToMap(worldPos.X, worldPos.Y, camera.Zoom);
            //DrawChunk(ScreenToMap(_graphics.PreferredBackBufferWidth/2,_graphics.PreferredBackBufferHeight/2));
            _spriteBatch.Draw(gridTexture, new Rectangle(((Map.MapToScreen((int)mousePos.X, (int)mousePos.Y)) * camera.Zoom).ToPoint(), new Vector2(gridTexture.Width * camera.Zoom, gridTexture.Height * camera.Zoom).ToPoint()), Color.White);
            Vector2 pos = mousePos;
            Entity entity = entityManager.GetFromPosition(pos);
            string text = $" FPS:{Math.Ceiling(1 / gameTime.ElapsedGameTime.TotalSeconds)} Coords:({pos.X}, {pos.Y})";
            if (entity != null)
            {
                text = $"{text} Entity: #{entity.Id}";
            }
            _spriteBatch.DrawString(debugFont, text, camera.ScreenToWorld(new Vector2(0, 0)), Color.White);
            _spriteBatch.End();
            base.Draw(gameTime);

        }
    }
}