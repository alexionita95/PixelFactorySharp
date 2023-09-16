using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PixelFactory.Entities;
using System;
using System.Collections.Generic;

namespace PixelFactory
{
    public class MapTile : DrawableEntity
    {
        public string Type { get; set; }
        public MapTile(SpriteBatch spriteBatch, string type)
            :base(spriteBatch, Vector2.One)
        {
            this.spriteBatch = spriteBatch;
            Type = type;
            Layer = DrawLayer.Map;
        }
        public override void Draw(GameTime gameTime)
        {
            Texture = ContentManager.Instance.GetTileTexture(Type);
            base.Draw(gameTime);
        }
    }
    public class MapChunk
    {
        SpriteBatch spriteBatch;
        List<MapTile> tiles;
        public Vector2 Position { get; set; }
        public Vector2 Offset { get; set; } = Vector2.Zero;
        public int Size { get; set; } = 32;
        public MapChunk(SpriteBatch spriteBatch)
        {
            tiles = new List<MapTile>();
            this.spriteBatch = spriteBatch;
        }
        public void AddTile(MapTile tile)
        {
            tiles.Add(tile);
        }
        public void Draw(GameTime gameTime)
        {
            foreach (MapTile tile in tiles)
            {
                tile.Draw(gameTime);
            }
        }
    }
    public class Map
    {
        public static int TileSize { get; set; } = 64;
        public static int ChunkSize { get; set; } = 16;
        public int MapSize { get; set; } = 2;
        public Vector2 MapOffset = Vector2.Zero;


        SpriteBatch spriteBatch;
        List<MapChunk> chunks;
        public Map(SpriteBatch _spriteBatch)
        {
            spriteBatch = _spriteBatch;
            chunks = new List<MapChunk>();
        }
        public static Vector2 MapToScreen(float mapX, float mapY)
        {
            return new Vector2(mapX*TileSize,mapY*TileSize);
        }
        public static Vector2 ScreenToMap(int screenX, int screenY, float zoom = 1)
        {
            return new Vector2(MathF.Floor(screenX/(TileSize*zoom)), MathF.Floor(screenY/(TileSize*zoom)));
        }
        public MapChunk GenerateChunk(int chunkX, int chunkY)
        {
            MapChunk chunk = new MapChunk(spriteBatch);
            for (int tileY = 0; tileY < ChunkSize; ++tileY)
            {
                for(int tileX = 0; tileX < ChunkSize;++tileX)
                {
                    MapTile tile = new MapTile(spriteBatch,"debug");
                    tile.Position = new Vector2(chunkX*ChunkSize + tileX + MapOffset.X, chunkY*ChunkSize + tileY + MapOffset.Y);
                    chunk.AddTile(tile);
                }
            }
            return chunk;
        }
        public void Generate()
        {
            for (int chunkY = 0; chunkY < MapSize; ++chunkY)
            {
                for (int chunkX = 0; chunkX < MapSize; ++chunkX)
                {
                    chunks.Add(GenerateChunk(chunkX, chunkY));
                }
            }
        }
        public void Draw(GameTime gameTime)
        {
            foreach (MapChunk chunk in chunks)
            {
                chunk.Draw(gameTime);
            }
        }

    }
}
