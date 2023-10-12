using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PixelFactory.Entities;
using PixelFactory.Serialization;
using System;
using System.Collections.Generic;

namespace PixelFactory
{
    public class MapTile : DrawableEntity
    {
        public string Type { get; set; }
        public MapTile(string type)
            : base(Vector2.One)
        {
            Type = type;
            Layer = DrawLayer.Map;
        }
        public MapTile() 
        { 
        }
        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            base.Draw(gameTime, spriteBatch);
        }
        public override List<byte> GetData()
        {
            List<byte> data = base.GetData();
            Serializer.WriteString(Type, data);
            return data;
        }
        public override void Deserialize(List<byte> data, ContentManager contentManager)
        {
            base.Deserialize(data, contentManager);
            string type = Serializer.ReadString(data);
            Texture = new Graphics.Texture(contentManager.TileTextures[type]);
        }
    }
    public class MapChunk
    {
        public Camera Camera { get; set; }
        public List<MapTile> Tiles { get; set; }
        public Vector2 Position { get; set; }
        public int Size { get; set; } = 32;
        public MapChunk()
        {
            Tiles = new List<MapTile>();
        }
        public void AddTile(MapTile tile)
        {
            Tiles.Add(tile);
        }
        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            foreach (MapTile tile in Tiles)
            {
                if (Camera.IsInviewport(tile.Position, tile.Size))
                {
                    tile.Zoom = Camera.Zoom;
                    tile.Draw(gameTime, spriteBatch);
                }
            }
        }
        public List<byte> GetData()
        {
            List<byte> data = new List<byte>();
            Serializer.WriteFloat(Position.X, data);
            Serializer.WriteFloat(Position.Y, data);
            Serializer.WriteInt(Tiles.Count, data);
            for (int i = 0; i < Tiles.Count; ++i)
            {
                data.AddRange(Tiles[i].GetData());
            }
            return data;
        }
        public void Deserialize(List<byte> data, ContentManager contentManager)
        {
            float x = Serializer.ReadFloat(data);
            float y = Serializer.ReadFloat(data);
            int tiles = Serializer.ReadInt(data);
            Position = new Vector2(x, y);
            Tiles.Clear();
            for(int i = 0; i < tiles; ++i) 
            {
                Tiles.Add(new MapTile());
                Tiles[Tiles.Count-1].Deserialize(data, contentManager);
            }
        }
    }
    public class Map : Entity
    {
        public static int TileSize { get; set; } = 64;
        public static int ChunkSize { get; set; } = 16;
        public int MapSize { get; set; } = 2;
        public long Seed { get; set; } = 0;
        public Vector2 MapOffset = Vector2.Zero;
        public Camera Camera { get; set; }

        public Dictionary<string, Texture2D> TileTextures { get; set; }
        public List<MapChunk> Chunks { get; set; }
        public Map()
        {
            Chunks = new List<MapChunk>();
        }
        public Map(Dictionary<string, Texture2D> tileTextures)
        {
            Chunks = new List<MapChunk>();
            TileTextures = tileTextures;
        }
        public static Vector2 MapToScreen(float mapX, float mapY)
        {
            return new Vector2(mapX * TileSize, mapY * TileSize);
        }
        public static Vector2 ScreenToMap(int screenX, int screenY, float zoom = 1)
        {
            return new Vector2(MathF.Floor(screenX / (TileSize * zoom)), MathF.Floor(screenY / (TileSize * zoom)));
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
                for (int tileX = 0; tileX < ChunkSize; ++tileX)
                {
                    MapTile tile = new MapTile("debug");

                    tile.Texture = new Graphics.Texture(TileTextures[tile.Type]);
                    tile.Position = new Vector2(chunkX * ChunkSize + tileX + MapOffset.X, chunkY * ChunkSize + tileY + MapOffset.Y);
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
                    Chunks.Add(GenerateChunk(chunkX, chunkY));
                }
            }
        }
        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            foreach (MapChunk chunk in Chunks)
            {
                chunk.Camera = Camera;
                chunk.Draw(gameTime, spriteBatch);
            }
        }
        public override List<byte> GetData()
        {
            List<byte> data = new List<byte>();
            Serializer.WriteLong(Seed, data);
            Serializer.WriteInt(Chunks.Count, data);
            for(int i=0; i<Chunks.Count; ++i)
            {
                data.AddRange(Chunks[i].GetData());
            }
            return data;
        }
        public override void Deserialize(List<byte> data, ContentManager contentManager)
        {
            Seed = Serializer.ReadLong(data);
            int chunks = Serializer.ReadInt(data);
            Chunks.Clear();
            for(int i=0; i < chunks; ++i) 
            {
                Chunks.Add(new MapChunk());
                Chunks[Chunks.Count-1].Deserialize(data, contentManager);
            }
        }

    }
}
