using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PixelFactory
{
    public class MapTile
    {
        public Vector2 Position { get; set; }
        private SpriteBatch spriteBatch;
        public string Type { get; set; }
        public MapTile(SpriteBatch spriteBatch, string type)
        {
            this.spriteBatch = spriteBatch;
            Type = type;
        }
        public void Draw(GameTime gameTime)
        {
            int mapX = (int)Position.X;
            int mapY = (int)Position.Y;
            Vector2 screenPosition = Map.MapToScreen(mapX, mapY);
            spriteBatch.Draw(ContentManager.Instance.GetTileTexture(Type), screenPosition, Color.White);
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
            /*float x = (mapX - mapY) * TileWidth / 2;
            float y = (mapX + mapY) * TileHeight / 2;*/

            return new Vector2(mapX*TileSize,mapY*TileSize);
        }
        public static Vector2 ScreenToMap(int screenX, int screenY)
        {
            /*int halfTileWidth = TileWidth / 2;
            int halfTileHeight = TileHeight / 2;

            int tileX = (screenX / halfTileWidth + screenY / halfTileHeight) / 2;
            int tileY = (screenY / halfTileHeight - screenX / halfTileWidth) / 2;*/

            // int x = (int)Math.Floor(Math.Floor(screenX/(tileWidth/2) + screenY/(tileHeight/2))/2);
            //int y = (int)Math.Floor(Math.Floor(screenY/(tileHeight/2) - screenX/(tileWidth/2))/2)+1;

            return new Vector2(MathF.Floor(screenX/TileSize), MathF.Floor(screenY/TileSize));
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
            /* for (int chunkY = 0; chunkY < MapSize; ++chunkY)
             {
                 for(int chunkX = 0; chunkX <MapSize; ++chunkX)
                 {
                     DrawChunk(new Vector2(chunkX*ChunkSize,chunkY*ChunkSize));                   
                 }
             }*/
        }

    }
}
