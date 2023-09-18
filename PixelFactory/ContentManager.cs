using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PixelFactory
{
    public class ContentManager
    {
        public Dictionary<string, Texture2D> TileTextures { get; private set; }
        private Dictionary<string, Texture2D> Textures;
        private Dictionary<string, SpriteFont> Fonts;
        public ContentManager()
        {
            Textures = new Dictionary<string, Texture2D>();
            TileTextures = new Dictionary<string, Texture2D>();
            Fonts = new Dictionary<string, SpriteFont>();

        }

        public void AddTexture(string name, Texture2D texture)
        {
            Textures.Add(name, texture);
        }
        public void AddTileTexture(string name, Texture2D texture)
        {
            TileTextures.Add(name, texture);
        }
        public Texture2D GetTexture(string type)
        {
            if (Textures.ContainsKey(type))
            {
                return Textures[type];
            }
            return null;
        }
        public void AddFont(string name, SpriteFont font)
        {
            Fonts.Add(name, font);
        }
        public SpriteFont GetFont(string name)
        {
            if (Fonts.ContainsKey(name))
            {
                return Fonts[name];
            }
            return null;
        }

    }
}
