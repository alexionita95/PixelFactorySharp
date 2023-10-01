using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PixelFactory.Entities;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PixelFactory.Items
{
    public class Item : DrawableEntity
    {
        public string Name { get; set; }
        public int StackSize { get; set; } = 10;
        public Item(SpriteBatch spriteBatch, Vector2 size) : base(spriteBatch, size)
        {
        }
        public Item(Item item): base(item)
        {
            Name = item.Name;
            StackSize = item.StackSize;
        }
        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
        }



    }
}
