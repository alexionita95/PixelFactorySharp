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
            :base(Vector2.One)
        {
            Type = type;
            Layer = DrawLayer.Map;
        }
        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            base.Draw(gameTime, spriteBatch);
        }
    }
    public class MapChunk
    {
        SpriteBatch spriteBatch;
        public Camera Camera { get; set; }
        List<MapTile> tiles;
        public Vector2 Position { get; set; }
        public Vector2 Offset { get; set; } = Vector2.Zero;
        public int Size { get; set; } = 32;
        public MapChunk()
        {
            tiles = new List<MapTile>();
        }
        public void AddTile(MapTile tile)
        {
            tiles.Add(tile);
        }
        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            foreach (MapTile tile in tiles)
            {
                if (Camera.IsInviewport(tile.Position, tile.Size))
                {
                    tile.Zoom = Camera.Zoom;
                    tile.Draw(gameTime, spriteBatch);
                }
            }
        }
    }
    public class Map
    {
        public static int TileSize { get; set; } = 64;
        public static int ChunkSize { get; set; } = 16;
        public int MapSize { get; set; } = 2;
        public Vector2 MapOffset = Vector2.Zero;
        public Camera Camera { get; set; }

        private Dictionary<string, Texture2D> TileTextures { get; set; }


        SpriteBatch spriteBatch;
        List<MapChunk> chunks;
        public Map(SpriteBatch _spriteBatch, Dictionary<string, Texture2D> tileTextures)
        {
            spriteBatch = _spriteBatch;
            chunks = new List<MapChunk>();
            TileTextures = tileTextures;
        }
        public static Vector2 MapToScreen(float mapX, float mapY)
        {
            return new Vector2(mapX*TileSize,mapY*TileSize);
        }
        public static Vector2 ScreenToMap(int screenX, int screenY, float zoom = 1)
        {
            return new Vector2(MathF.Floor(screenX/(TileSize*zoom)), MathF.Floor(screenY/(TileSize*zoom)));
        }
        public static Vector2 ScreenToMap(Vector2 position, float zoom = 1)
        {
            return new Vector2(MathF.Floor(position.X / (TileSize * zoom)), MathF.Floor(position.Y / (TileSize * zoom)));
        }

        public MapChunk GenerateChunk(int chunkX, int chunkY)
        {
            MapChunk chunk = new MapChunk();
            for (int tileY = 0; tileY < ChunkSize; ++tileY)
            {
                for(int tileX = 0; tileX < ChunkSize;++tileX)
                {
                    MapTile tile = new MapTile(spriteBatch,"debug");
                    tile.Texture = TileTextures[tile.Type];
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
        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            foreach (MapChunk chunk in chunks)
            {
                chunk.Camera = Camera;
                chunk.Draw(gameTime, spriteBatch);
            }
        }

    }
}
