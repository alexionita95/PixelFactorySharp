using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PixelFactory.Entities;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PixelFactory.Items
{
    public class InventoryEntity : DrawableEntity
    {
        public string Name { get; set; }
        public int MaximumQuantity { get; set; } = 10;
        public InventoryEntity(SpriteBatch spriteBatch, Vector2 size) : base(spriteBatch, size)
        {
        }
        public InventoryEntity(InventoryEntity item): base(item)
        {
            Name = item.Name;
            MaximumQuantity = item.MaximumQuantity;
        }
        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
        }
        public bool HasSameId(InventoryEntity item)
        {
            if (item == null)
            {
                return false;
            }
            return item.Id == Id;
        }

        public override bool Equals(object obj)
        {
            if (obj is null || !(obj is InventoryEntity)) return false;
            InventoryEntity item = (InventoryEntity)obj;
            return this.Id == item.Id;
            
        }

        public override int GetHashCode()
        {
           return Id.GetHashCode() + MaximumQuantity.GetHashCode() + base.GetHashCode();
        }
    }
}
