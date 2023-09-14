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
        private static ContentManager _instance;
        public static ContentManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new ContentManager();
                }
                return _instance;
            }
        }

        private Dictionary<string, Texture2D> TileTextures;
        private Dictionary<string, Texture2D> ItemTextures;
        private Dictionary<string, Texture2D> BuildingTextures;
        private Dictionary<string, SpriteFont> Fonts;
        private ContentManager()
        {
            TileTextures = new Dictionary<string, Texture2D>();
            ItemTextures = new Dictionary<string, Texture2D>();
            BuildingTextures = new Dictionary<string, Texture2D>();
            Fonts = new Dictionary<string, SpriteFont>();

        }

        public void AddTileTexture(string name, Texture2D texture)
        {
            TileTextures.Add(name, texture);
        }
        public Texture2D GetTileTexture(string type)
        {
            if (TileTextures.ContainsKey(type))
            {
                return TileTextures[type];
            }
            return null;
        }
        public void AddItemTexture(string name, Texture2D texture)
        {
            ItemTextures.Add(name, texture);
        }
        public Texture2D GetItemTexture(string type)
        {
            if (ItemTextures.ContainsKey(type))
            {
                return ItemTextures[type];
            }
            return null;
        }

        public void AddBuildingTexture(string name, Texture2D texture)
        {
            BuildingTextures.Add(name, texture);
        }
        public Texture2D GetBuildingTexture(string type)
        {
            if (BuildingTextures.ContainsKey(type))
            {
                return BuildingTextures[type];
            }
            return null;
        }

        public void AddBuildingTexture(string name, SpriteFont font)
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
